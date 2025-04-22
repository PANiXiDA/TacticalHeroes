using SQLite4Unity3d;

namespace Assets.Scripts.Repositories.Models
{
    [Table("HeroAndAbilityScopes")]
    public class HeroAndAbilityScope
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int HeroId { get; set; }
        public int AbilityId { get; set; }
    }
}
