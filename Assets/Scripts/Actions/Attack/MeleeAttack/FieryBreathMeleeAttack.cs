using Assets.Scripts.IActions;
using Assets.Scripts.Interfaces;
using Cysharp.Threading.Tasks;

namespace Assets.Scripts.Actions.Attack.MeleeAttack
{
    public class FieryBreathMeleeAttack : IMeleeAttack
    {
        private IDamage _damageCalculator;
        public FieryBreathMeleeAttack(IDamage damageCalculator)
        {
            _damageCalculator = damageCalculator;
        }
        public async UniTask MeleeAttack(BaseUnit attacker, BaseUnit defender)
        {
            Tile.Instance.DeleteHighlight();
            attacker.isBusy = true;
            bool death;

            UnitManager.Instance.ChangeUnitFlip(attacker, defender.OccupiedTile);
            UnitManager.Instance.PlayAttackAnimation(attacker, defender);

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

            bool responseAttack = UnitManager.Instance.IsResponseAttack(attacker);

            await defender.TakeMeleeDamage(attacker, defender, _damageCalculator);
            death = UnitManager.Instance.IsDead(defender);
            if (death)
            {
                await defender.Death();
            }

            if (!death && !responseAttack && defender.UnitResponse)
            {
                defender.UnitResponse = false;
                await defender.MeleeAttack(defender, attacker, defender.OccupiedTile);
            }

            UnitManager.Instance.SetOriginalUnitFlip(attacker);

            attacker.isBusy = false;
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
