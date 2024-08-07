using Assets.Scripts.IActions;
using Assets.Scripts.Interfaces;
using Assets.Scripts.Managers;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Assets.Scripts.Actions.Attack.MeleeAttack
{
    public class DiscardMeleeAttack : DefaultMeleeAttack, IMeleeAttack
    {
        public DiscardMeleeAttack(IDamage damageCalculator) : base(damageCalculator) { }

        protected override async UniTask<bool> EnemyTakeDamage(BaseUnit attacker, BaseUnit defender)
        {
            await defender.TakeMeleeDamage(attacker, defender, _damageCalculator);
            bool discard = false;
            bool death = UnitManager.Instance.IsDead(defender);
            if (death)
            {
                await defender.Death();
            }
            else
            {
                discard = DiscardAttack(attacker, defender);
            }
            return discard & !death;
        }

        private bool DiscardAttack(BaseUnit attacker, BaseUnit defender)
        {
            if (Random.Range(0f, 100f) < 20f && attacker.Side == GameManager.Instance.CurrentSide)
            {
                var diffCoord = GridManager.Instance.GetTileCoordinate(attacker.OccupiedTile) - GridManager.Instance.GetTileCoordinate(defender.OccupiedTile);
                var tile = GridManager.Instance.GetTileAtPosition(GridManager.Instance.GetTileCoordinate(defender.OccupiedTile) - diffCoord);

                if (tile != null)
                {
                    if (tile.OccupiedUnit == null && tile.Walkable)
                    {
                        Vector3[] path = new Vector3[1];
                        path[0] = new Vector3(tile.Position.x, tile.Position.y, 0);
                        defender.transform.DOPath(path, 1, PathType.Linear, PathMode.TopDown2D).SetEase(Ease.Linear);

                        defender.OccupiedTile.OccupiedUnit = null;
                        tile.OccupiedUnit = defender;
                        defender.OccupiedTile = tile;

                        MenuManager.Instance.DeletePortrait(defender);
                        int index = TurnManager.Instance.ATB.FindIndex(pair => pair.Value == defender);
                        TurnManager.Instance.ATB.RemoveAt(index);

                        return true;
                    }
                }
            }
            return false;
        }
    }
}
