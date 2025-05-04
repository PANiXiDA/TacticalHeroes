using System;
using System.Collections.Generic;

namespace Assets.Scripts.GameEngine.DTO.ATBCalculator
{
    public class ATBNextTurnResult
    {
        public Guid NextGameObjectId { get; set; }
        public List<GameEntity> UpdatedATBState { get; set; }
        public double DeltaTime { get; set; }
    }
}
