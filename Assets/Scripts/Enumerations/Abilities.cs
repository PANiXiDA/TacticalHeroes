using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Enumeration
{
    public enum Abilities
    {
        [Description("Летающее существо")]
        Fly = 0,
        [Description("Стрелок")]
        Archer = 1,
        [Description("Двойной удар")]
        DoubleAttack = 2,


    }
}
