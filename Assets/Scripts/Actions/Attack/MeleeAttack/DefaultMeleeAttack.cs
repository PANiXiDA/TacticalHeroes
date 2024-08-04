using Assets.Scripts.IActions;
using Assets.Scripts.Interfaces;
using Cysharp.Threading.Tasks;

namespace Assets.Scripts.Actions.Attack.MeleeAttack
{
    public class DefaultMeleeAttack : IMeleeAttack
    {
        private IDamage _damageCalculator;
        public DefaultMeleeAttack(IDamage damageCalculator) 
        {
            _damageCalculator = damageCalculator;
        }
        public async UniTask MeleeAttack(BaseUnit attacker, BaseUnit defender)
        {
            Tile.Instance.DeleteHighlight();
            attacker.isBusy = true;

            UnitManager.Instance.ChangeUnitFlip(attacker, defender.OccupiedTile);

            UnitManager.Instance.PlayAttackAnimation(attacker, defender);

            bool responseAttack = UnitManager.Instance.IsResponseAttack(attacker);

            bool isLuck = UnitManager.Instance.Luck(attacker);
            _damageCalculator.isLuck = isLuck;

            await defender.TakeMeleeDamage(attacker, defender, _damageCalculator);
            bool death = UnitManager.Instance.IsDead(defender);
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
    }
}
