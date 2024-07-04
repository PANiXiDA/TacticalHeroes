using Assets.Scripts.Interfaces;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Actions.Attack.MeleeAttack
{
    public class DefaultMeleeAttack : IMeleeAttack
    {
        public async UniTask MeleeAttack(BaseUnit attacker, BaseUnit defender)
        {
            Tile.Instance.DeleteHighlight();
            attacker.isBusy = true;

            attacker.animator.Play("MeleeAttack");

            bool responseAttack = UnitManager.Instance.IsResponseAttack(attacker);

            defender.TakeMeleeDamage(attacker, defender);
            bool death = UnitManager.Instance.IsDeath(defender);
            if (death)
            {
                defender.Death(responseAttack);
            }

            if (!death && !responseAttack && defender.UnitResponse)
            {
                await UniTask.Delay(1000);
                defender.UnitResponse = false;
                await defender.Attack(defender, attacker, defender.OccupiedTile);
            }

            attacker.isBusy = false;
        }
    }
}
