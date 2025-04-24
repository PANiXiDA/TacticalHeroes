using System.Collections.Generic;

using Assets.Scripts.Common.Enumerations;
using Assets.Scripts.GameEngine.Domain;
using Assets.Scripts.GameEngine.DTO.ATBCalculator;
using Assets.Scripts.GameEngine.DTO.PathFinderCalculator;

namespace Assets.Scripts.Domain.DTO.Models
{
    public class RoundState
    {
        public List<Tile> Grid { get; set; } = new();
        public List<Hero> Heroes { get; set; } = new();
        public List<Unit> Units { get; set; } = new();
        public List<GameEntity> ATB { get; set; } = new();
        public CommandType? LastCommand { get; set; }
    }
}
