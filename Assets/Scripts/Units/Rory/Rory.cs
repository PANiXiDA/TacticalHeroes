using Assets.Scripts.Actions.Damage;
using Assets.Scripts.IActions;
using Assets.Scripts.Managers;
using Cysharp.Threading.Tasks;

namespace Assets.Scripts.Units.Rory
{
    public class Rory : BaseUnit
    {
        private IDamage _damageCalculator;
        protected override void Awake()
        {
            SoundManager.Instance.PlayRoryBattleSound();
        }

        protected override void Start()
        {
            _damageCalculator = new DefaultDamage();
        }
        public override async UniTask Move(BaseUnit unit, Tile targetTile)
        {
            await MenuManager.Instance.ShowRoryTalk(this, _damageCalculator);
            TurnManager.Instance.EndTurn(this, true);
        }

        public override async UniTask MeleeAttack(BaseUnit attacker, BaseUnit defender, Tile targetTile)
        {
            await MenuManager.Instance.ShowRoryTalk(this, _damageCalculator);
            TurnManager.Instance.EndTurn(this, true);
        }

        public override async UniTask RangeAttack(BaseUnit attacker, BaseUnit defender)
        {
            await MenuManager.Instance.ShowRoryTalk(this, _damageCalculator);
            TurnManager.Instance.EndTurn(this, true);
        }

        public override async UniTask TakeMeleeDamage(BaseUnit attacker, BaseUnit defender, IDamage damageCalculator)
        {
            await MenuManager.Instance.ShowRoryTalk(this, _damageCalculator);
            TurnManager.Instance.EndTurn(this, true);
        }
        public override async UniTask TakeRangeDamage(BaseUnit attacker, BaseUnit defender, IDamage damageCalculator)
        {
            await MenuManager.Instance.ShowRoryTalk(this, _damageCalculator);
            TurnManager.Instance.EndTurn(this, true);
        }

        public override async UniTask Death()
        {
            await MenuManager.Instance.ShowRoryTalk(this, _damageCalculator);
            TurnManager.Instance.EndTurn(this, true);
        }
    }
}
