using SQLite4Unity3d;

namespace Assets.Scripts.Repositories.Models
{
    [Table("Abilities")]
    public class Ability
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int AbilityType { get; set; }
    }
}
