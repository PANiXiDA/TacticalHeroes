using SQLite4Unity3d;

using Assets.Scripts.GameEngine.Domain.Enums;

namespace Assets.Scripts.Repositories.Models
{
    [Table("Effects")]
    public class Effect
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public EffectType Type { get; set; }
        public double Value { get; set; }
        public double Duration { get; set; }
        public string Parameters { get; set; }
    }
}
