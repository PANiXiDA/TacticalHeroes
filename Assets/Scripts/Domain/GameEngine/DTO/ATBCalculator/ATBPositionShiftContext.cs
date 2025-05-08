using System;
using System.Collections.Generic;

namespace Assets.Scripts.GameEngine.DTO.ATBCalculator
{
    public class ATBPositionShiftContext
    {
        public List<GameEntity> CurrentATBState { get; set; }
        public Guid GameObjectId { get; set; }
        public double ShiftFactor { get; set; } = 0.5;
        public bool IsWait { get; set; }
    }
}
