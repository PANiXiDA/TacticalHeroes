using Assets.Scripts.IActions;
using Assets.Scripts.Interfaces;
using Cysharp.Threading.Tasks;

namespace Assets.Scripts.Actions.Attack.MeleeAttack
{
    public class FieryBreathMeleeAttack : DefaultMeleeAttack, IMeleeAttack
    {
        public FieryBreathMeleeAttack(IDamage damageCalculator) : base(damageCalculator) { }

        protected override async UniTask<bool> EnemyTakeDamage(BaseUnit attacker, BaseUnit defender)
        {
            bool death;

            BaseUnit secondDefender = GetUnitBehind(attacker, defender);
            if (secondDefender != null)
            {
                _ = secondDefender.TakeMeleeDamage(attacker, secondDefender, _damageCalculator);
                death = UnitManager.Instance.IsDead(secondDefender);
                if (death)
                {
                    _ = secondDefender.Death();
                }
            }

            await defender.TakeMeleeDamage(attacker, defender, _damageCalculator);
            death = UnitManager.Instance.IsDead(defender);
            if (death)
            {
                await defender.Death();
            }

            return !death;
        }

        private BaseUnit GetUnitBehind(BaseUnit attacker, BaseUnit defender)
        {
            var diffCoord = GridManager.Instance.GetTileCoordinate(attacker.OccupiedTile) - GridManager.Instance.GetTileCoordinate(defender.OccupiedTile);
            var tile = GridManager.Instance.GetTileAtPosition(defender.OccupiedTile.Position - diffCoord);

            if (tile != null)
            {
                if (tile.OccupiedUnit != null)
                {
                    return tile.OccupiedUnit;
                }
            }

            return null;
        }
    }
}
