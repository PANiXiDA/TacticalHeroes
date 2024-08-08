using Assets.Scripts.Actions.Damage;
using Assets.Scripts.IActions;
using Cysharp.Threading.Tasks;
using Assets.Scripts.Enumerations;
using Assets.Scripts.Managers;

namespace Assets.Scripts.Units.Neutral
{
    public class Paladin : BaseNeutralUnit
    {
        protected bool IsActiveGodShield { get; set; }

        protected double PercentAbsorptionDamage { get; set; }

        protected override void Start()
        {
            base.Start();
            IsActiveGodShield = true;
            PercentAbsorptionDamage = 0.7;
        }
        public override async UniTask Move(BaseUnit unit, Tile targetTile)
        {
            IsActiveGodShield = true;
            await base.Move(unit, targetTile);
        }

        public override async UniTask MeleeAttack(BaseUnit attacker, BaseUnit defender, Tile targetTile)
        {
            if (attacker.Side == GameManager.Instance.CurrentSide)
            {
                IsActiveGodShield = true;
            }
            await base.MeleeAttack(attacker, defender, targetTile);
        }

        public override async UniTask TakeMeleeDamage(BaseUnit attacker, BaseUnit defender, IDamage damageCalculator)
        {
            if (IsActiveGodShield)
            {
                damageCalculator = SetDamageByActiveShield(defender);
            }
            await base.TakeMeleeDamage(attacker, defender, damageCalculator);
        }

        public override async UniTask TakeRangeDamage(BaseUnit attacker, BaseUnit defender, IDamage damageCalculator)
        {
            if (IsActiveGodShield)
            {
                damageCalculator = SetDamageByActiveShield(defender);
            }
            await base.TakeRangeDamage(attacker, defender, damageCalculator);
        }

        private IDamage SetDamageByActiveShield(BaseUnit defender)
        {
            IsActiveGodShield = false;
            IDamage damageCalculator = new PercentDefaultDamage(1 - PercentAbsorptionDamage);

            var defenderColor = defender.Side == Side.Player ? "red" : "blue";
            string message = $"<color={defenderColor}>{defender.UnitName}</color> поглотил 70% урона божественным щитом. " +
                $"(Анимации пока нет, так как нет щита)";
            MenuManager.Instance.AddMessageToChat(message);

            return damageCalculator;
        }
    }
}
