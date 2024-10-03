using Assets.Scripts.Units.IActions;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Linq;
using UnityEngine;
using Assets.Scripts.Managers;

namespace Assets.Scripts.Units.Actions.Move
{
    public class DefaultMove : IMove
    {
        public virtual async UniTask Move(BaseUnit unit, Tile targetTile)
        {
            unit.isBusy = true;

            UnitManager.Instance.ChangeUnitFlip(unit, targetTile);

            Tile.Instance.DeleteHighlight();
            var path = PathFinder.Instance.GetPath(GridManager.Instance.GetTileCoordinate(unit.OccupiedTile),
                GridManager.Instance.GetTileCoordinate(targetTile), unit);

            if (path.Count > 0)
            {
                path.Reverse();
                path.RemoveAt(0);

                Vector3[] path_ = path.Select(p => new Vector3(p.x, p.y, 0)).ToArray();

                unit.animator.Play("Move");
                await unit.transform.DOPath(path_, 1, PathType.Linear, PathMode.TopDown2D).SetEase(Ease.Linear);

                unit.OccupiedTile.OccupiedUnit = null;
                targetTile.OccupiedUnit = unit;
                unit.OccupiedTile = targetTile;
            }

            UnitManager.Instance.SetOriginalUnitFlip(unit);

            unit.isBusy = false;
        }
    }
}
