using SQLite4Unity3d;

namespace Assets.Scripts.Repositories.Models
{
    [Table("AbilityAndEffectScopes")]
    public class AbilityAndEffectScope
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int AbilityId { get; set; }
        public int EffectId { get; set; }
    }
}
