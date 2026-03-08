using System;
using System.Text;
using DarkAge.Core.Domain;
using DarkAge.Gameplay.Results;
using UnityEngine;
using UnityEngine.UI;

namespace DarkAge.Presentation.UI
{
    public sealed class ResourceHudPresenter : MonoBehaviour
    {
        private Text _resourcesText;
        private Text _statusText;
        private Button _collectButton;
        private Button _placeButton;

        public event Action CollectClicked;
        public event Action PlaceClicked;

        public void Build(Transform parent)
        {
            var panel = CreatePanel(parent, "ResourceHud", new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(12f, -12f), new Vector2(-12f, -140f));

            _resourcesText = CreateText(panel.transform, "ResourcesText", 18, TextAnchor.UpperLeft);
            _resourcesText.rectTransform.anchorMin = new Vector2(0f, 0f);
            _resourcesText.rectTransform.anchorMax = new Vector2(0.68f, 1f);
            _resourcesText.rectTransform.offsetMin = new Vector2(12f, 12f);
            _resourcesText.rectTransform.offsetMax = new Vector2(-12f, -12f);

            _statusText = CreateText(panel.transform, "StatusText", 16, TextAnchor.MiddleRight);
            _statusText.rectTransform.anchorMin = new Vector2(0.68f, 0.5f);
            _statusText.rectTransform.anchorMax = new Vector2(1f, 1f);
            _statusText.rectTransform.offsetMin = new Vector2(0f, 12f);
            _statusText.rectTransform.offsetMax = new Vector2(-12f, -12f);
            _statusText.text = "Initializing...";

            _collectButton = CreateButton(panel.transform, "CollectButton", "Collect", new Vector2(1f, 0f), new Vector2(1f, 0f), new Vector2(-180f, 12f), new Vector2(-20f, 60f));
            _collectButton.onClick.AddListener(() => CollectClicked?.Invoke());

            _placeButton = CreateButton(panel.transform, "PlaceButton", "Place HQ", new Vector2(1f, 0f), new Vector2(1f, 0f), new Vector2(-380f, 12f), new Vector2(-200f, 60f));
            _placeButton.onClick.AddListener(() => PlaceClicked?.Invoke());
            _placeButton.interactable = false;
        }

        public void Render(WorldStateSnapshot snapshot, GridPosition? selectedGrid)
        {
            if (_resourcesText == null)
            {
                return;
            }

            var resources = snapshot.PlayerProgress.Resources;
            var builder = new StringBuilder();
            builder.AppendLine($"Food: {resources.Get(ResourceType.Food)}");
            builder.AppendLine($"Gold: {resources.Get(ResourceType.Gold)}");
            builder.AppendLine($"Iron: {resources.Get(ResourceType.Iron)}");
            builder.AppendLine($"Money: {resources.Get(ResourceType.Money)}");
            builder.AppendLine($"Petroleum: {resources.Get(ResourceType.Petroleum)}");
            builder.Append($"Power: {resources.Get(ResourceType.Power)}");
            _resourcesText.text = builder.ToString();

            if (snapshot.PlayerProgress.HasHeadquarters)
            {
                _statusText.text = $"HQ placed at {snapshot.PlayerProgress.Headquarters.GridPosition}";
                _placeButton.interactable = false;
            }
            else if (selectedGrid.HasValue)
            {
                _statusText.text = $"Selected grid: {selectedGrid.Value}";
                _placeButton.interactable = true;
            }
            else
            {
                _statusText.text = "Select a tile to place HQ";
                _placeButton.interactable = false;
            }
        }

        public void ShowMessage(string message)
        {
            if (_statusText != null)
            {
                _statusText.text = message;
            }
        }

        private static GameObject CreatePanel(Transform parent, string name, Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin, Vector2 offsetMax)
        {
            var panelObject = new GameObject(name, typeof(RectTransform), typeof(Image));
            panelObject.transform.SetParent(parent, false);
            var rectTransform = panelObject.GetComponent<RectTransform>();
            rectTransform.anchorMin = anchorMin;
            rectTransform.anchorMax = anchorMax;
            rectTransform.offsetMin = offsetMin;
            rectTransform.offsetMax = offsetMax;

            var image = panelObject.GetComponent<Image>();
            image.color = new Color(0.06f, 0.09f, 0.13f, 0.82f);
            return panelObject;
        }

        private static Text CreateText(Transform parent, string name, int fontSize, TextAnchor anchor)
        {
            var textObject = new GameObject(name, typeof(RectTransform), typeof(Text));
            textObject.transform.SetParent(parent, false);
            var text = textObject.GetComponent<Text>();
            text.font = FindRuntimeFont();
            text.fontSize = fontSize;
            text.alignment = anchor;
            text.color = Color.white;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Overflow;
            return text;
        }

        private static Button CreateButton(Transform parent, string name, string label, Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin, Vector2 offsetMax)
        {
            var buttonObject = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
            buttonObject.transform.SetParent(parent, false);
            var rectTransform = buttonObject.GetComponent<RectTransform>();
            rectTransform.anchorMin = anchorMin;
            rectTransform.anchorMax = anchorMax;
            rectTransform.offsetMin = offsetMin;
            rectTransform.offsetMax = offsetMax;

            var image = buttonObject.GetComponent<Image>();
            image.color = new Color(0.2f, 0.28f, 0.16f, 0.95f);

            var labelText = CreateText(buttonObject.transform, $"{name}Label", 18, TextAnchor.MiddleCenter);
            labelText.text = label;
            labelText.rectTransform.anchorMin = Vector2.zero;
            labelText.rectTransform.anchorMax = Vector2.one;
            labelText.rectTransform.offsetMin = Vector2.zero;
            labelText.rectTransform.offsetMax = Vector2.zero;

            return buttonObject.GetComponent<Button>();
        }

        private static Font FindRuntimeFont()
        {
            var font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            return font != null ? font : Resources.GetBuiltinResource<Font>("Arial.ttf");
        }
    }
}
