using System;

namespace DarkAge.Core.Domain
{
    [Serializable]
    public sealed class BaseState
    {
        public BaseState(BuildingType buildingType, GridPosition gridPosition, DateTime placedAtUtc)
        {
            BuildingType = buildingType;
            GridPosition = gridPosition;
            PlacedAtUtc = placedAtUtc;
        }

        public BuildingType BuildingType { get; }

        public GridPosition GridPosition { get; }

        public DateTime PlacedAtUtc { get; }
    }
}
