using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.IActions
{
    public interface IDamage
    {
        public (int damage, int deathCount) CalculateDamageAndDeathUnit(BaseUnit attacker, BaseUnit defender);
        public int CalculateDamage(BaseUnit attacker, BaseUnit defender);
        public int CalculateDeathCount(BaseUnit defender, int damage);
    }
}
