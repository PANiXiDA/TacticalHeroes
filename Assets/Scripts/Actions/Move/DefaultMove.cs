using Assets.Scripts.Interfaces;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Actions.Move
{
    public class DefaultMove : IMove
    {
        public virtual async UniTask Move(BaseUnit unit, Tile targetTile)
        {
            unit.isBusy = true;

            Dictionary<Vector2, Tile> tilesForMove = UnitManager.Instance.GetTilesForMove(unit);
            if (tilesForMove.ContainsValue(targetTile))
            {
                Tile.Instance.DeleteHighlight();
                var path = PathFinder.Instance.GetPath(GridManager.Instance.GetTileCoordinate(unit.OccupiedTile),
                    GridManager.Instance.GetTileCoordinate(targetTile), unit);
                path.Reverse();
                path.RemoveAt(0);

                Vector3[] path_ = path.Select(p => new Vector3(p.x, p.y, 0)).ToArray();

                unit.animator.Play("Move");
                await unit.transform.DOPath(path_, 1, PathType.Linear, PathMode.TopDown2D).SetEase(Ease.Linear);

                unit.OccupiedTile.OccupiedUnit = null;
                targetTile.OccupiedUnit = unit;
                unit.OccupiedTile = targetTile;

            }
            unit.isBusy = false;
        }
    }
}
