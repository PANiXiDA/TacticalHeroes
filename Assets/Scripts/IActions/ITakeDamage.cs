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
        public void TakeMeleeDamage(BaseUnit attacker, BaseUnit defender);
        public void TakeRangeDamage(BaseUnit attacker, BaseUnit defender);
        public int CalculateDamage(BaseUnit attacker, BaseUnit defender);
    }
}
