using Assets.Scripts.IActions;
using Assets.Scripts.Interfaces;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Scripts.Actions.Attack.MeleeAttack
{
    public class LineMeleeAttack : DefaultMeleeAttack, IMeleeAttack
    {
        public LineMeleeAttack(IDamage damageCalculator) : base(damageCalculator) { }

        protected override async UniTask<bool> EnemyTakeDamage(BaseUnit attacker, BaseUnit defender)
        {
            List<BaseUnit> unitsForAttack = GetAroundUnits(attacker, defender);
            bool death = false;
            foreach (var unit in unitsForAttack)
            {
                if (unit == defender)
                {
                    await unit.TakeMeleeDamage(attacker, unit, _damageCalculator);
                    death = UnitManager.Instance.IsDead(unit);
                    if (death)
                    {
                        await unit.Death();
                    }
                }
                else
                {
                    _ = unit.TakeMeleeDamage(attacker, unit, _damageCalculator);
                    bool deathUnit = UnitManager.Instance.IsDead(unit);
                    if (deathUnit)
                    {
                        _ = unit.Death();
                    }
                }
            }
            return !death;
        }

        private List<BaseUnit> GetAroundUnits(BaseUnit attacker, BaseUnit defender)
        {
            List<BaseUnit> unitsForAttack = new List<BaseUnit>();

            var attackerTileCoord = GridManager.Instance.GetTileCoordinate(attacker.OccupiedTile);
            var defenderTileCoord = GridManager.Instance.GetTileCoordinate(defender.OccupiedTile);

            List<Vector2> offsets = new List<Vector2>();

            if (attackerTileCoord.y == defenderTileCoord.y)
            {
                offsets.Add(new Vector2(defenderTileCoord.x, defenderTileCoord.y + 1));
                offsets.Add(new Vector2(defenderTileCoord.x, defenderTileCoord.y - 1));
            }
            else if (attackerTileCoord.x == defenderTileCoord.x)
            {
                offsets.Add(new Vector2(defenderTileCoord.x + 1, defenderTileCoord.y));
                offsets.Add(new Vector2(defenderTileCoord.x - 1, defenderTileCoord.y));
            }
            else if ((attackerTileCoord.x < defenderTileCoord.x && attackerTileCoord.y < defenderTileCoord.y) ||
                     (attackerTileCoord.x > defenderTileCoord.x && attackerTileCoord.y > defenderTileCoord.y))
            {
                offsets.Add(new Vector2(defenderTileCoord.x - 1, defenderTileCoord.y + 1));
                offsets.Add(new Vector2(defenderTileCoord.x + 1, defenderTileCoord.y - 1));
            }
            else
            {
                offsets.Add(new Vector2(defenderTileCoord.x - 1, defenderTileCoord.y - 1));
                offsets.Add(new Vector2(defenderTileCoord.x + 1, defenderTileCoord.y + 1));
            }

            foreach (var offset in offsets)
            {
                Tile tile = GridManager.Instance.GetTileAtPosition(offset);
                if (tile != null && tile.OccupiedUnit != null)
                {
                    unitsForAttack.Add(tile.OccupiedUnit);
                }
            }

            unitsForAttack.Add(defender);

            return unitsForAttack;
        }

    }
}
