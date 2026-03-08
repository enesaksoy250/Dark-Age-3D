using System.Linq;
using System.Text;
using DarkAge.Gameplay.Results;
using UnityEngine;
using UnityEngine.UI;

namespace DarkAge.Presentation.UI
{
    public sealed class TaskHudPresenter : MonoBehaviour
    {
        private Text _tasksText;

        public void Build(Transform parent)
        {
            var panel = new GameObject("TaskHud", typeof(RectTransform), typeof(Image));
            panel.transform.SetParent(parent, false);
            var rectTransform = panel.GetComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0f, 0f);
            rectTransform.anchorMax = new Vector2(0.32f, 0f);
            rectTransform.offsetMin = new Vector2(12f, 12f);
            rectTransform.offsetMax = new Vector2(-12f, 160f);

            var image = panel.GetComponent<Image>();
            image.color = new Color(0.11f, 0.08f, 0.08f, 0.84f);

            var textObject = new GameObject("TaskText", typeof(RectTransform), typeof(Text));
            textObject.transform.SetParent(panel.transform, false);
            _tasksText = textObject.GetComponent<Text>();
            _tasksText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf") ?? Resources.GetBuiltinResource<Font>("Arial.ttf");
            _tasksText.fontSize = 16;
            _tasksText.color = Color.white;
            _tasksText.alignment = TextAnchor.UpperLeft;

            var textRect = _tasksText.rectTransform;
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = new Vector2(12f, 12f);
            textRect.offsetMax = new Vector2(-12f, -12f);
        }

        public void Render(WorldStateSnapshot snapshot)
        {
            if (_tasksText == null)
            {
                return;
            }

            var builder = new StringBuilder("Tasks\n");
            foreach (var task in snapshot.PlayerProgress.Tasks.OrderBy(task => task.TaskId))
            {
                builder.AppendLine($"{task.TaskId}: {task.CurrentProgress}/{task.RequiredProgress} {(task.RewardGranted ? "[Rewarded]" : task.IsCompleted ? "[Complete]" : string.Empty)}");
            }

            _tasksText.text = builder.ToString();
        }
    }
}
