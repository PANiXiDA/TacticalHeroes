using SQLite4Unity3d;

namespace Assets.Scripts.Repositories.Models
{
    [Table("Effects")]
    public class Effect
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int EffectType { get; set; }
        public double Value { get; set; }
        public double Duration { get; set; }
        public string Parameters { get; set; }
    }
}
