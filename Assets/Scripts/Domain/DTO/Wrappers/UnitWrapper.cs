using Assets.Scripts.GameEngine.Domain;
using Assets.Scripts.GameEngine.DTO.Enums;

namespace Assets.Scripts.Domain.DTO.Wrappers
{
    public class UnitWrapper
    {
        public PlayerSide Side { get; set; }
        public int TeamNumber { get; set; }
        public int TileX { get; set; }
        public int TileY { get; set; }

        public Unit Unit { get; set; }

        public UnitWrapper(
            PlayerSide side,
            int teamNumber,
            int tileX,
            int tileY)
        {
            Side = side;
            TeamNumber = teamNumber;
            TileX = tileX;
            TileY = tileY;
        }
    }
}
