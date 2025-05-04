using System.ComponentModel.DataAnnotations;

namespace Assets.Scripts.Domain.GameEngine.Domain.Enums
{
    public enum StatType
    {
        [Display(Name = "Атака")]
        Attack = 0,

        [Display(Name = "Защита")]
        Defence = 1,

        [Display(Name = "Здоровье")]
        Health = 2,

        [Display(Name = "Минимальный урон")]
        MinDamage = 3,

        [Display(Name = "Максимальный урон")]
        MaxDamage = 4,

        [Display(Name = "Инициатива")]
        Initiative = 5,

        [Display(Name = "Скорость")]
        Speed = 6,

        [Display(Name = "Дальность")]
        Range = 7,

        [Display(Name = "Количество стрел")]
        Arrows = 8,

        [Display(Name = "Мораль")]
        Morale = 9,

        [Display(Name = "Удача")]
        Luck = 10,
    }
}
