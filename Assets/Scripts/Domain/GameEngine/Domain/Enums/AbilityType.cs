using System.ComponentModel.DataAnnotations;

namespace Assets.Scripts.GameEngine.Domain.Enums
{
    public enum AbilityType
    {
        [Display(Name = "Летающее существо")]
        Fly = 0,

        [Display(Name = "Двойная атака")]
        DoubleDamage = 1,

        [Display(Name = "Вампиризм")]
        Vampirisme = 2,

        [Display(Name = "Стрелок")]
        Archer = 3,

        [Display(Name = "Нет штрафа в ближнем бою")]
        NoPenaltyInMelee = 4,

        [Display(Name = "Отбрасывающий удар")]
        DiscardingBlow = 5,

        [Display(Name = "Игнорирование защиты")]
        IgnoringDefence = 6,

        [Display(Name = "Атакует первым")]
        AttackFirst = 7,

        [Display(Name = "Снайпер")]
        Sniper = 8,

        [Display(Name = "Огненное дыхание")]
        FieryBreath = 9,

        [Display(Name = "Нежить")]
        Undead = 10,

        [Display(Name = "Выстрел по площади")]
        AreaRangeAttack = 11,

        [Display(Name = "Большой щит")]
        BigShield = 12,

        [Display(Name = "Враг не отвечает")]
        NoResponseAttack = 13,

        [Display(Name = "Удар с разбега")]
        KnightRunUp = 14,

        [Display(Name = "Атака по линии")]
        LineAttack = 15,

        [Display(Name = "Двойной выстрел")]
        DoubleRangeAttack = 16,

        [Display(Name = "Защита богов")]
        GodDefence = 17,

        [Display(Name = "Бесконечный отпор")]
        UnlimitResponce = 18,
    }
}
