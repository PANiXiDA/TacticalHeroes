using System;

using SQLite4Unity3d;

namespace Assets.Scripts.Repositories.Models
{
    [Table("Units")]
    public class Unit
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
        public int Health { get; set; }
        public int Speed { get; set; }
        public int? Range { get; set; }
        public int? Arrows { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
