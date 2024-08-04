using Assets.Scripts.IActions;
using Assets.Scripts.Interfaces;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Actions.Attack.RangeAttack
{
    public class ArealRangeAttack : IRangeAttack
    {
        private IDamage _damageCalculator;
        public ArealRangeAttack(IDamage damageCalculator)
        {
            _damageCalculator = damageCalculator;
        }
        public async UniTask RangeAttack(BaseUnit attacker, BaseUnit defender)
        {
            Tile.Instance.DeleteHighlight();
            attacker.isBusy = true;

            attacker.UnitArrows -= 1;
            attacker.animator.Play("RangeAttack");

            bool isLuck = UnitManager.Instance.Luck(attacker);
            _damageCalculator.isLuck = isLuck;

            List<BaseUnit> defenders = GetArealUnits(defender);
            foreach (var defenderForAttack in defenders)
            {
                _ = defenderForAttack.TakeRangeDamage(attacker, defenderForAttack, _damageCalculator);
                bool death = UnitManager.Instance.IsDead(defenderForAttack);
                if (death)
                {
                    _ = defenderForAttack.Death();
                }
            }

            await UniTask.Delay(1000);
            attacker.isBusy = false;
        }

        private List<BaseUnit> GetArealUnits(BaseUnit defender)
        {
            List<BaseUnit> defenders = new List<BaseUnit>();
            defenders.Add(defender);

            var directions = new Vector2Int[] {
                new Vector2Int(1, 0),
                new Vector2Int(0, 1),
                new Vector2Int(0, -1),
                new Vector2Int(-1, 0),
                new Vector2Int(1, 1),
                new Vector2Int(1, -1),
                new Vector2Int(-1, -1),
                new Vector2Int(-1, 1)
            };

            Vector2 defenderPosition = GridManager.Instance.GetTileCoordinate(defender.OccupiedTile);

            foreach (var direction in directions)
            {
                Tile tile = GridManager.Instance.GetTileAtPosition(defenderPosition + direction);
                if (tile != null)
                {
                    if (tile.OccupiedUnit != null)
                    {
                        defenders.Add(tile.OccupiedUnit);
                    }
                }
            }

            return defenders;
        }
    }
}
