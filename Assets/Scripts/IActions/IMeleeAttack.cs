using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Interfaces
{
    public interface IMeleeAttack
    {
        public UniTask MeleeAttack(BaseUnit attacker, BaseUnit defender);
    }
}
