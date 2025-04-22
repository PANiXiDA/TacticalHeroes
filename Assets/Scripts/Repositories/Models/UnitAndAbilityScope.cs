using SQLite4Unity3d;

namespace Assets.Scripts.Repositories.Models
{
    [Table("UnitAndAbilityScopes")]
    public class UnitAndAbilityScope
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int UnitId { get; set; }
        public int AbilityId { get; set; }
    }
}
