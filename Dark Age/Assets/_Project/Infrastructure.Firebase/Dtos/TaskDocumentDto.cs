using System;

namespace DarkAge.Infrastructure.Firebase.Dtos
{
    [Serializable]
    public sealed class TaskDocumentDto
    {
        public string taskId;
        public string taskType;
        public int currentProgress;
        public int requiredProgress;
        public bool rewardGranted;
    }
}
