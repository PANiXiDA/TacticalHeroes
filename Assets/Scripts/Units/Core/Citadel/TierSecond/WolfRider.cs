using Assets.Scripts.Managers;
using Assets.Scripts.Units.Citadel;
using Cysharp.Threading.Tasks;

namespace Assets.Scripts.Units.Enemies
{
    public class WolfRider : BaseCitadelUnit
    {
        public override async UniTask MeleeAttack(BaseUnit attacker, BaseUnit defender, Tile targetTile)
        {
            await _move.Move(attacker, targetTile);
            await _meleeAttack.MeleeAttack(attacker, defender);
            if (defender.UnitCount > 0 && attacker.UnitCount > 0 && attacker.Side == GameManager.Instance.CurrentSide) 
            {
                await _meleeAttack.MeleeAttack(attacker, defender);
            }
            if (attacker.Side == GameManager.Instance.CurrentSide)
            {
                TurnManager.Instance.EndTurn(this);
            }
        }
    }
}
