using System;

namespace DarkAge.Core.Domain
{
    [Serializable]
    public sealed class WorldBounds
    {
        public WorldBounds(int minX, int maxX, int minZ, int maxZ)
        {
            if (minX > maxX)
            {
                throw new ArgumentOutOfRangeException(nameof(minX), "minX cannot be greater than maxX.");
            }

            if (minZ > maxZ)
            {
                throw new ArgumentOutOfRangeException(nameof(minZ), "minZ cannot be greater than maxZ.");
            }

            MinX = minX;
            MaxX = maxX;
            MinZ = minZ;
            MaxZ = maxZ;
        }

        public int MinX { get; }

        public int MaxX { get; }

        public int MinZ { get; }

        public int MaxZ { get; }

        public bool Contains(GridPosition position)
        {
            return position.X >= MinX && position.X <= MaxX &&
                   position.Z >= MinZ && position.Z <= MaxZ;
        }
    }
}
