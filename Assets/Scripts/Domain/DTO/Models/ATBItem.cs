using System;

namespace Assets.Scripts.Domain.DTO.Models
{
    public class ATBItem
    {
        public Guid Id { get; }
        public string Name { get; }
        public int PlayerId { get; }
        public int? Count { get; set; }

        public ATBItem(
            Guid id,
            string name,
            int playerId,
            int? count) 
        {
            Id = id;
            Name = name;
            PlayerId = playerId;
            Count = count;
        }
    }
}
