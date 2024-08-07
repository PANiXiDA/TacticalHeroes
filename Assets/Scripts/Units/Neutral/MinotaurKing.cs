using Assets.Scripts.Actions.Attack.MeleeAttack;
using Assets.Scripts.Actions.TakeDamage;
using Assets.Scripts.Managers;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
namespace Assets.Scripts.Units.Neutral
{
    public class MinotaurKing : BaseNeutralUnit
    {
        protected override void Start()
        {
            base.Start();
            _meleeAttack = new LineMeleeAttack(_damageCalculator);
        }
    }
}
