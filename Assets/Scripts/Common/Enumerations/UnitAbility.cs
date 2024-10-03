using System.ComponentModel;

public enum UnitAbility
{
    [Description("Летающее существо")]
    Fly = 0,
    [Description("Стрелок")]
    Archer = 1,
    [Description("Двойной удар")]
    DoubleMeleeAttack = 2,
    [Description("Огненное дыхание")]
    FieryBreath = 3,
    [Description("Нежить")]
    Undead = 4,
    [Description("Площадный выстрел")]
    AreaRangeAttack = 5,
    [Description("Нет штрафа в ближнем бою")]
    FullMeleeDamage = 6,
    [Description("Отбрасывающий удар")]
    DiscardMeleeAttack = 7,
    [Description("Большой щит")]
    BigShield = 8,
    [Description("Враг не сопротивляется")]
    NoResponseAttack = 9,
    [Description("Бдительность")]
    AttackFirst = 10,
    [Description("Рыцарский разбег")]
    KnightRunUp = 11,
    [Description("Атака по линии")]
    LineAttack = 12,
    [Description("Снайпер")]
    Sniper = 13,
    [Description("Двойной выстрел")]
    DoubleRangeAttack = 14,
    [Description("Божественная защита")]
    GodDefence = 15,
    [Description("Бесконечный отпор")]
    UnlimitResponce = 16,
    [Description("Игнорирование защиты")]
    IgnoreDefence = 17,
}
