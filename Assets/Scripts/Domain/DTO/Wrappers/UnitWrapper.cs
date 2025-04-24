using Assets.Scripts.GameEngine.Domain;
using Assets.Scripts.GameEngine.DTO.Enums;

namespace Assets.Scripts.Domain.DTO.Wrappers
{
    public class UnitWrapper
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public PlayerSide Side { get; set; }
        public int TeamNumber { get; set; }
        public int TileX { get; set; }
        public int TileY { get; set; }

        public Unit Unit { get; set; }

        public UnitWrapper(
            string name,
            string description,
            PlayerSide side,
            int teamNumber,
            int tileX,
            int tileY)
        {
            Name = name;
            Description = description;
            Side = side;
            TeamNumber = teamNumber;
            TileX = tileX;
            TileY = tileY;
        }
    }
}
