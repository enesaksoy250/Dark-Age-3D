using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DarkAge.Core.Domain;
using DarkAge.Presentation.Runtime;
using DarkAge.Presentation.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace DarkAge.Presentation.World
{
    public sealed class WorldSceneController : MonoBehaviour
    {
        private const float DragThreshold = 10f;

        private readonly Dictionary<string, GameObject> _renderedBases = new Dictionary<string, GameObject>();
        private readonly List<GameObject> _spawnedObjects = new List<GameObject>();

        private GameApplication _application;
        private CancellationTokenSource _cancellationTokenSource;
        private Camera _camera;
        private ResourceHudPresenter _resourceHudPresenter;
        private TaskHudPresenter _taskHudPresenter;
        private WorldStateSnapshot _currentSnapshot;
        private GameObject _previewObject;
        private Vector2 _pressStartPosition;
        private bool _isDraggingCamera;
        private GridPosition? _selectedGrid;

        public void Initialize(GameApplication application)
        {
            _application = application;
            _cancellationTokenSource = new CancellationTokenSource();
            application.StateChanged += HandleStateChanged;
            application.ErrorOccurred += HandleError;
            BuildScene();
            _ = InitializeAsync();
        }

        private async Task InitializeAsync()
        {
            var initialized = await _application.InitializeAsync(_cancellationTokenSource.Token);
            if (!initialized)
            {
                return;
            }

            HandleStateChanged(_application.CurrentState);
        }

        private void Update()
        {
            if (_camera == null || _application?.CurrentState == null || Pointer.current == null)
            {
                return;
            }

            HandleCameraDrag();
            HandleGridSelection();
        }

        private void OnDestroy()
        {
            if (_application != null)
            {
                _application.StateChanged -= HandleStateChanged;
                _application.ErrorOccurred -= HandleError;
            }

            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
        }

        private void BuildScene()
        {
            Screen.autorotateToPortrait = false;
            Screen.autorotateToPortraitUpsideDown = false;
            Screen.autorotateToLandscapeLeft = true;
            Screen.autorotateToLandscapeRight = true;
            Screen.orientation = ScreenOrientation.AutoRotation;

            _camera = Camera.main;
            if (_camera == null)
            {
                var cameraObject = new GameObject("DarkAgeCamera", typeof(Camera), typeof(AudioListener));
                _camera = cameraObject.GetComponent<Camera>();
                cameraObject.tag = "MainCamera";
                _spawnedObjects.Add(cameraObject);
            }

            _camera.transform.position = new Vector3(0f, 28f, -18f);
            _camera.transform.rotation = Quaternion.Euler(55f, 0f, 0f);
            _camera.clearFlags = CameraClearFlags.SolidColor;
            _camera.backgroundColor = new Color(0.12f, 0.16f, 0.2f);

            var light = Object.FindAnyObjectByType<Light>();
            if (light == null)
            {
                var lightObject = new GameObject("Directional Light", typeof(Light));
                light = lightObject.GetComponent<Light>();
                light.type = LightType.Directional;
                light.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
                _spawnedObjects.Add(lightObject);
            }

            CreateGround();
            CreateUi();
            CreatePreviewObject();
        }

        private void CreateGround()
        {
            var rules = _application.Rules;
            var bounds = rules.WorldBounds;
            var worldWidth = (bounds.MaxX - bounds.MinX + 1) * rules.GridCellSize;
            var worldDepth = (bounds.MaxZ - bounds.MinZ + 1) * rules.GridCellSize;

            var ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "WorldGround";
            ground.transform.position = new Vector3(
                (bounds.MinX + bounds.MaxX) * rules.GridCellSize * 0.5f,
                0f,
                (bounds.MinZ + bounds.MaxZ) * rules.GridCellSize * 0.5f);
            ground.transform.localScale = new Vector3(worldWidth / 10f, 1f, worldDepth / 10f);
            var renderer = ground.GetComponent<Renderer>();
            renderer.material.color = new Color(0.26f, 0.35f, 0.22f);
            _spawnedObjects.Add(ground);
        }

        private void CreateUi()
        {
            var canvasObject = new GameObject("DarkAgeCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvasObject.transform.SetParent(transform, false);
            _spawnedObjects.Add(canvasObject);

            var canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            var scaler = canvasObject.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);

            if (EventSystem.current == null)
            {
                var eventSystem = new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));
                _spawnedObjects.Add(eventSystem);
            }

            _resourceHudPresenter = canvasObject.AddComponent<ResourceHudPresenter>();
            _resourceHudPresenter.Build(canvasObject.transform);
            _resourceHudPresenter.CollectClicked += async () => await HandleCollectClicked();
            _resourceHudPresenter.PlaceClicked += async () => await HandlePlaceClicked();

            _taskHudPresenter = canvasObject.AddComponent<TaskHudPresenter>();
            _taskHudPresenter.Build(canvasObject.transform);
        }

        private void CreatePreviewObject()
        {
            _previewObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            _previewObject.name = "HQPreview";
            _previewObject.transform.localScale = new Vector3(1.6f, 1f, 1.6f);
            _previewObject.GetComponent<Renderer>().material.color = new Color(0.92f, 0.82f, 0.2f);
            _previewObject.SetActive(false);
            _spawnedObjects.Add(_previewObject);
        }

        private void HandleStateChanged(WorldStateSnapshot snapshot)
        {
            _currentSnapshot = snapshot;
            RenderBases(snapshot);
            RefreshPreviewVisibility();
            _resourceHudPresenter?.Render(snapshot, _selectedGrid);
            _taskHudPresenter?.Render(snapshot);
        }

        private void HandleError(string message)
        {
            _resourceHudPresenter?.ShowMessage(message);
        }

        private void RenderBases(WorldStateSnapshot snapshot)
        {
            foreach (var renderedBase in _renderedBases.Values)
            {
                Destroy(renderedBase);
            }

            _renderedBases.Clear();
            foreach (var worldBase in snapshot.WorldBases)
            {
                var baseObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
                baseObject.name = $"HQ_{worldBase.OwnerId.Value}";
                baseObject.transform.position = GridToWorldPosition(worldBase.BaseState.GridPosition) + new Vector3(0f, 0.5f, 0f);
                baseObject.transform.localScale = new Vector3(1.8f, 1f, 1.8f);
                baseObject.GetComponent<Renderer>().material.color =
                    worldBase.OwnerId.Equals(snapshot.PlayerProgress.Profile.PlayerId)
                        ? new Color(0.26f, 0.67f, 0.94f)
                        : new Color(0.64f, 0.23f, 0.23f);

                _renderedBases[worldBase.OwnerId.Value] = baseObject;
            }
        }

        private void HandleCameraDrag()
        {
            var pointer = Pointer.current;
            if (pointer == null)
            {
                return;
            }

            if (pointer.press.wasPressedThisFrame)
            {
                _pressStartPosition = pointer.position.ReadValue();
                _isDraggingCamera = false;
            }

            if (pointer.press.isPressed)
            {
                var delta = pointer.position.ReadValue() - _pressStartPosition;
                if (delta.sqrMagnitude > DragThreshold * DragThreshold)
                {
                    _isDraggingCamera = true;
                    _camera.transform.position += new Vector3(-pointer.delta.ReadValue().x, 0f, -pointer.delta.ReadValue().y) * 0.02f;
                }
            }
        }

        private void HandleGridSelection()
        {
            var pointer = Pointer.current;
            if (pointer == null || !pointer.press.wasReleasedThisFrame || _isDraggingCamera)
            {
                return;
            }

            if (_currentSnapshot != null && _currentSnapshot.PlayerProgress.HasHeadquarters)
            {
                return;
            }

            var ray = _camera.ScreenPointToRay(pointer.position.ReadValue());
            if (!Physics.Raycast(ray, out var hitInfo, 500f))
            {
                return;
            }

            var selectedGrid = WorldToGridPosition(hitInfo.point);
            if (!_application.Rules.WorldBounds.Contains(selectedGrid))
            {
                return;
            }

            _selectedGrid = selectedGrid;
            _previewObject.transform.position = GridToWorldPosition(selectedGrid) + new Vector3(0f, 0.5f, 0f);
            _previewObject.SetActive(true);
            _resourceHudPresenter?.Render(_currentSnapshot, _selectedGrid);
        }

        private async Task HandleCollectClicked()
        {
            await _application.CollectResourcesAsync(_cancellationTokenSource.Token);
        }

        private async Task HandlePlaceClicked()
        {
            if (!_selectedGrid.HasValue)
            {
                _resourceHudPresenter?.ShowMessage("Select a valid tile first.");
                return;
            }

            await _application.PlaceHeadquartersAsync(_selectedGrid.Value, _cancellationTokenSource.Token);
            _selectedGrid = null;
            RefreshPreviewVisibility();
        }

        private void RefreshPreviewVisibility()
        {
            if (_previewObject == null)
            {
                return;
            }

            var shouldShow = _currentSnapshot != null && !_currentSnapshot.PlayerProgress.HasHeadquarters && _selectedGrid.HasValue;
            _previewObject.SetActive(shouldShow);
            if (shouldShow)
            {
                _previewObject.transform.position = GridToWorldPosition(_selectedGrid.Value) + new Vector3(0f, 0.5f, 0f);
            }
        }

        private GridPosition WorldToGridPosition(Vector3 worldPosition)
        {
            var cellSize = _application.Rules.GridCellSize;
            var gridX = Mathf.RoundToInt(worldPosition.x / cellSize);
            var gridZ = Mathf.RoundToInt(worldPosition.z / cellSize);
            return new GridPosition(gridX, gridZ);
        }

        private Vector3 GridToWorldPosition(GridPosition gridPosition)
        {
            var cellSize = _application.Rules.GridCellSize;
            return new Vector3(gridPosition.X * cellSize, 0f, gridPosition.Z * cellSize);
        }
    }
}
