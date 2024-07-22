using Assets.Scripts.IActions;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Interfaces
{
    public interface ITakeDamage
    {
        public UniTask TakeMeleeDamage(BaseUnit attacker, BaseUnit defender, IDamage damageCalculator);
        public UniTask TakeRangeDamage(BaseUnit attacker, BaseUnit defender, IDamage damageCalculator);
    }
}
