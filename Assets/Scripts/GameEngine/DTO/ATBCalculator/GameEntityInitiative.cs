using System;

namespace Assets.Scripts.GameEngine.DTO.ATBCalculator
{
    public class GameEntityInitiative
    {
        public Guid GameEntityId { get; set; }
        public double Initiative { get; set; }

        public GameEntityInitiative(
            Guid gameEntityId,
            double initiative)
        {
            GameEntityId = gameEntityId;
            Initiative = initiative;
        }
    }
}
