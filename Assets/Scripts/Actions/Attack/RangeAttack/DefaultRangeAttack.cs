using Assets.Scripts.Interfaces;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Actions.Attack.RangeAttack
{
    public class DefaultRangeAttack : IRangeAttack
    {
        public async UniTask RangeAttack(BaseUnit attacker, BaseUnit defender)
        {
            Tile.Instance.DeleteHighlight();
            attacker.isBusy = true;

            attacker.animator.Play("RangeAttack");

            defender.TakeRangeDamage(attacker, defender);
            bool death = UnitManager.Instance.IsDead(defender);
            if (death)
            {
                defender.Death();
            }

            await UniTask.Delay(1000);
            attacker.isBusy = false;
        }
    }
}
