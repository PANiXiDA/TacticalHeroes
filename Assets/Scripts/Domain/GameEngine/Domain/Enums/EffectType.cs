using System.ComponentModel.DataAnnotations;

namespace Assets.Scripts.GameEngine.Domain.Enums
{
    public enum EffectType
    {
        [Display(Name = "Оборона")]
        Defence = 0,

        [Display(Name = "Кровотечение")]
        Bleeding = 1,
    }
}
