using Assets.Scripts.GameEngine.DTO.Enums;
using Assets.Scripts.GameEngine.DTO.PathFinderCalculator;
using Assets.Scripts.GameEngine.Interfaces;
using Assets.Scripts.Services.Interfaces.Battle;
using R3;

using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace Assets.Scripts.Services.Implementations.Battle
{
    public sealed class GridsService : IGridsService
    {
        private readonly IGridGenerator _generator;
        private readonly Subject<IReadOnlyList<Tile>> _gridGenerated = new();

        private List<Tile> _grid = new();

        public Observable<IReadOnlyList<Tile>> OnGridGenerated => _gridGenerated.AsObservable();

        public GridsService(IGridGenerator generator)
        {
            _generator = generator;
        }

        public void GenerateGrid(GameType type)
        {
            var tiles = _generator.GenerateGrid(type);
            _gridGenerated.OnNext(tiles);

            SetCache(tiles);
        }

        public List<Tile> GetGrid() => _grid;

        public Tile GetTile(Guid unitId) => _grid.FirstOrDefault(tile => tile.OccupiedUnitId == unitId);

        public int GetDistance(Tile a, Tile b)
        {
            int dx = Mathf.Abs(a.X - b.X);
            int dy = Mathf.Abs(a.Y - b.Y);

            return Mathf.CeilToInt(Mathf.Sqrt(dx * dx + dy * dy));
        }

        private void SetCache(List<Tile> grid)
        {
            _grid = grid;
        }
    }
}
