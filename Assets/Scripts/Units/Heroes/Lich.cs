using Assets.Scripts.Actions.Attack.MeleeAttack;
using Assets.Scripts.Actions.Attack.RangeAttack;
using Assets.Scripts.Actions.Damage;
using Assets.Scripts.Actions.Move;
using Assets.Scripts.IActions;
using Assets.Scripts.Interfaces;
using Assets.Scripts.Managers;
using Cysharp.Threading.Tasks;

namespace Assets.Scripts.Units.Heroes
{
    public class Lich : BaseUnit
    {
        private IDamage _damageMeleeCalculator;
        private IDamage _damageRangeCalculator;
        private IMove _move;
        private IMeleeAttack _meleeAttack;
        private IRangeAttack _rangeAttack;

        protected override void Start()
        {
            _damageRangeCalculator = new DefaultDamage();
            _damageMeleeCalculator = new HalfDamage(_damageRangeCalculator);
            _move = new DefaultMove();
            _meleeAttack = new DefaultMeleeAttack(_damageMeleeCalculator);
            _rangeAttack = new DefaultRangeAttack(_damageRangeCalculator);
            UnitMorale = 0;
            UnitRange = 7;
            UnitArrows = 10;
        }

        public override async UniTask MeleeAttack(BaseUnit attacker, BaseUnit defender, Tile targetTile)
        {
            await _move.Move(attacker, targetTile);
            await _meleeAttack.MeleeAttack(attacker, defender);
            if (attacker.Side == GameManager.Instance.CurrentSide)
            {
                TurnManager.Instance.EndTurn(this);
            }
        }

        public override async UniTask RangeAttack(BaseUnit attacker, BaseUnit defender)
        {
            await _rangeAttack.RangeAttack(attacker, defender);
            if (attacker.Side == GameManager.Instance.CurrentSide)
            {
                TurnManager.Instance.EndTurn(this);
            }
        }
    }
}
