using Assets.Scripts.Interfaces;
using Cysharp.Threading.Tasks;

namespace Assets.Scripts.Actions.Attack.MeleeAttack
{
    public class DefaultMeleeAttack : IMeleeAttack
    {
        public async UniTask MeleeAttack(BaseUnit attacker, BaseUnit defender)
        {
            Tile.Instance.DeleteHighlight();
            attacker.isBusy = true;

            UnitManager.Instance.ChangeUnitFlip(attacker, defender.OccupiedTile);

            attacker.animator.Play("MeleeAttack");

            bool responseAttack = UnitManager.Instance.IsResponseAttack(attacker);

            await defender.TakeMeleeDamage(attacker, defender);
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
