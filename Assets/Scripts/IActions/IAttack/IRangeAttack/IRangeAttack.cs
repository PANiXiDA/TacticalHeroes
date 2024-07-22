using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Interfaces
{
    public interface IRangeAttack
    {
        public UniTask RangeAttack(BaseUnit attacker, BaseUnit defender);
    }
}
