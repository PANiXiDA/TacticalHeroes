using Assets.Scripts.Actions.Attack.MeleeAttack;
using Assets.Scripts.Actions.Damage;
using Cysharp.Threading.Tasks;

namespace Assets.Scripts.Units.Neutral
{
    public class Cavalry : BaseNeutralUnit
    {
        public override async UniTask MeleeAttack(BaseUnit attacker, BaseUnit defender, Tile targetTile)
        {
            var path = PathFinder.Instance.GetPath(attacker.OccupiedTile.Position, targetTile.Position, attacker);
            if (path.Count > 0)
            {
                path.RemoveAt(0);
            } 
            _damageCalculator = new PercentDefaultDamage(1 + 0.05 * path.Count);
            _meleeAttack = new DefaultMeleeAttack(_damageCalculator);
            await base.MeleeAttack(attacker, defender, targetTile);         
        }
    }
}
