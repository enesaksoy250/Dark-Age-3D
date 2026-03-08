using System.IO;
using UnityEngine;

namespace DarkAge.Infrastructure.Firebase.Persistence
{
    internal static class LocalPersistencePaths
    {
        public static string RootDirectory
        {
            get
            {
                var root = Path.Combine(Application.persistentDataPath, "DarkAge");
                Directory.CreateDirectory(root);
                return root;
            }
        }

        public static string PlayersDirectory
        {
            get
            {
                var directory = Path.Combine(RootDirectory, "Players");
                Directory.CreateDirectory(directory);
                return directory;
            }
        }

        public static string GetPlayerFilePath(string playerId)
        {
            return Path.Combine(PlayersDirectory, $"{playerId}.json");
        }

        public static string WorldFilePath => Path.Combine(RootDirectory, "world-bases.json");
    }
}
