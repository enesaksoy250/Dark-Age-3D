using System.Reflection;
using System.Threading.Tasks;

namespace DarkAge.Infrastructure.Firebase.Auth
{
    internal static class ReflectionTaskUtility
    {
        public static async Task AwaitTaskAsync(object taskInstance)
        {
            if (taskInstance is Task task)
            {
                await task.ConfigureAwait(false);
            }
        }

        public static object GetTaskResult(object taskInstance)
        {
            if (taskInstance == null)
            {
                return null;
            }

            var resultProperty = taskInstance.GetType().GetProperty("Result", BindingFlags.Public | BindingFlags.Instance);
            return resultProperty?.GetValue(taskInstance);
        }
    }
}
