using Assets.Scripts.IActions;
using Assets.Scripts.Interfaces;
using Cysharp.Threading.Tasks;

namespace Assets.Scripts.Actions.Attack.MeleeAttack
{
    public class DefaultMeleeAttack : IMeleeAttack
    {
        protected IDamage _damageCalculator;
        public DefaultMeleeAttack(IDamage damageCalculator) 
        {
            _damageCalculator = damageCalculator;
        }
        public async UniTask MeleeAttack(BaseUnit attacker, BaseUnit defender)
        {
            Tile.Instance.DeleteHighlight();
            attacker.isBusy = true;

            UnitManager.Instance.ChangeUnitFlip(attacker, defender.OccupiedTile);

            bool isLuck = UnitManager.Instance.Luck(attacker);
            _damageCalculator.isLuck = isLuck;

            var death = await EnemyTakeDamage(attacker, defender);

            await ResponseAttack(attacker, defender, death);

            UnitManager.Instance.SetOriginalUnitFlip(attacker);

            attacker.isBusy = false;
        }

        protected virtual async UniTask<bool> EnemyTakeDamage(BaseUnit attacker, BaseUnit defender)
        {
            await defender.TakeMeleeDamage(attacker, defender, _damageCalculator);
            bool death = UnitManager.Instance.IsDead(defender);
            if (death)
            {
                await defender.Death();
            }
            return !death;
        }

        protected virtual async UniTask ResponseAttack(BaseUnit attacker, BaseUnit defender, bool isCanAttack)
        {
            bool isResponseAttack = UnitManager.Instance.IsResponseAttack(attacker);

            if (isCanAttack && !isResponseAttack && defender.UnitResponse)
            {
                defender.UnitResponse = false;
                await defender.MeleeAttack(defender, attacker, defender.OccupiedTile);
            }
        }
    }
}
