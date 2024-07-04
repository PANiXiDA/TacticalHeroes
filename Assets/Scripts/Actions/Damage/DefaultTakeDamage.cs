using Assets.Scripts.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Actions.Damage
{
    public class DefaultTakeDamage : ITakeDamage
    {
        public virtual void TakeMeleeDamage(BaseUnit attacker, BaseUnit defender)
        {
            int damage = CalculateDamage(attacker, defender);

            defender.UnitHealth -= damage;
            defender.animator.Play("TakeDamage");
        }
        public virtual void TakeRangeDamage(BaseUnit attacker, BaseUnit defender)
        {
            int damage = CalculateDamage(attacker, defender);

            defender.UnitHealth -= damage;
            defender.animator.Play("TakeDamage");
        }

        public int CalculateDamage(BaseUnit attacker, BaseUnit defender)
        {
            double baseDamage = UnityEngine.Random.Range(attacker.UnitMinDamage, attacker.UnitMaxDamage);
            double damageModifier = attacker.UnitAttack > defender.UnitDefence ?
                (1 + 0.05 * (attacker.UnitAttack - defender.UnitDefence)) :
                (1 / (1 + 0.05 * (defender.UnitDefence - attacker.UnitAttack)));

            int damage = (int)(baseDamage * damageModifier);

            MenuManager.Instance.ShowDamage(attacker, defender, damage);

            return damage;
        }
    }
}
