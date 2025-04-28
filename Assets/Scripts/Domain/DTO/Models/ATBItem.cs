namespace Assets.Scripts.Domain.DTO.Models
{
    public class ATBItem
    {
        public string Name { get; }
        public int PlayerId { get; }
        public int? Count { get; }

        public ATBItem(
            string name,
            int playerId,
            int? count) 
        {
            Name = name;
            PlayerId = playerId;
            Count = count;
        }
    }
}
