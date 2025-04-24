using System.Collections.Generic;

using SQLite4Unity3d;

namespace Assets.Scripts.Repositories.Models
{
    [Table("Heroes")]
    public class Hero
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int Attack { get; set; }
        public int Defence { get; set; }
        public int MinDamage { get; set; }
        public int MaxDamage { get; set; }
        public double Initiative { get; set; }
        public int Morale { get; set; }
        public int Luck { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        [Ignore]
        public List<Ability> Abilities { get; set; } = new();
    }
}
