using System.ComponentModel;

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
        [Description("Огненное дыхание")]
        FieryBreath = 3,
    }
}
