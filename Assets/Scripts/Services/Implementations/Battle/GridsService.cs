using Assets.Scripts.GameEngine.DTO.Enums;
using Assets.Scripts.GameEngine.DTO.PathFinderCalculator;
using Assets.Scripts.GameEngine.Interfaces;
using Assets.Scripts.Services.Interfaces.Battle;
using R3;

using System.Collections.Generic;

namespace Assets.Scripts.Services.Implementations.Battle
{
    public sealed class GridsService : IGridsService
    {
        private readonly IGridGenerator _generator;
        private readonly Subject<IReadOnlyList<Tile>> _gridGenerated = new();

        public Observable<IReadOnlyList<Tile>> OnGridGenerated => _gridGenerated.AsObservable();

        public GridsService(IGridGenerator generator)
        {
            _generator = generator;
        }

        public void GenerateGrid(GameType type)
        {
            var tiles = _generator.GenerateGrid(type);
            _gridGenerated.OnNext(tiles);
        }
    }
}
