using Assets.Scripts.Units.Citadel;
using Cysharp.Threading.Tasks;

namespace Assets.Scripts.Units.Enemies
{
    public class Bear : BaseCitadelUnit
    {
        protected double PercentIgnoreDefence { get; set; }

        protected override void Start()
        {
            base.Start();
            PercentIgnoreDefence = 0.4;
        }

        public override async UniTask MeleeAttack(BaseUnit attacker, BaseUnit defender, Tile targetTile)
        {
            var defence = defender.UnitDefence;
            defender.UnitDefence = (int)(defender.UnitDefence * (1 - PercentIgnoreDefence));
            await base.MeleeAttack(attacker, defender, targetTile);
            defender.UnitDefence = defence;
        }
    }
}
