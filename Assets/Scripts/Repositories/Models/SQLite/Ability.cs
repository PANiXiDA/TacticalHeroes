using System.Collections.Generic;

using Assets.Scripts.GameEngine.Domain.Enums;

using SQLite4Unity3d;

namespace Assets.Scripts.Repositories.Models
{
    [Table("Abilities")]
    public class Ability
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public AbilityType Type { get; set; }

        [Ignore]
        public List<Effect> Effects { get; set; } = new();
    }
}
