using Assets.Scripts.Actions.Attack.MeleeAttack;
using Assets.Scripts.Actions.Damage;
using Assets.Scripts.Actions.Move;
using Assets.Scripts.IActions;
using Assets.Scripts.Interfaces;
using Assets.Scripts.Managers;
using Cysharp.Threading.Tasks;

namespace Assets.Scripts.Units.Enemies
{
    public class WolfRider : BaseUnit
    {
        private IDamage _damageCalculator;
        private IMove _move;
        private IMeleeAttack _meleeAttack;

        protected override void Start()
        {
            _damageCalculator = new DefaultDamage();
            _move = new DefaultMove();
            _meleeAttack = new DefaultMeleeAttack(_damageCalculator);
        }
        public override async UniTask MeleeAttack(BaseUnit attacker, BaseUnit defender, Tile targetTile)
        {
            await _move.Move(attacker, targetTile);
            await _meleeAttack.MeleeAttack(attacker, defender);
            if (defender.UnitCount > 0 && attacker.UnitCount > 0 && attacker.Faction == GameManager.Instance.CurrentFaction) 
            {
                await _meleeAttack.MeleeAttack(attacker, defender);
            }
            if (attacker.Faction == GameManager.Instance.CurrentFaction)
            {
                TurnManager.Instance.EndTurn(this);
            }
        }
    }
}
