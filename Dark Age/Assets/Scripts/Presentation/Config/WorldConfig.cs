using DarkAge.Core.Domain;
using UnityEngine;

namespace DarkAge.Presentation.Config
{
    [CreateAssetMenu(fileName = "WorldConfig", menuName = "Dark Age/Config/World")]
    public sealed class WorldConfig : ScriptableObject
    {
        [SerializeField] private int minX = -10;
        [SerializeField] private int maxX = 10;
        [SerializeField] private int minZ = -10;
        [SerializeField] private int maxZ = 10;
        [SerializeField] private float cellSize = 2f;

        public WorldBounds CreateBounds()
        {
            return new WorldBounds(minX, maxX, minZ, maxZ);
        }

        public float CellSize => cellSize;

        public static WorldConfig CreateDefault()
        {
            return CreateInstance<WorldConfig>();
        }
    }
}
