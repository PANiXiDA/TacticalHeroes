using System;
using System.Collections.Generic;

using Assets.Scripts.GameEngine.Domain;
using Assets.Scripts.GameEngine.DTO.Enums;

namespace Assets.Scripts.Infrastructure.Models
{
    public class PlayerBattleData
    {
        public int? Id { get; set; }
        public Guid? SessionId { get; set; }
        public Guid BuildId { get; set; }
        public PlayerSide Side { get; set; }

        public List<Unit> Units { get; set; } = new();
    }
}
