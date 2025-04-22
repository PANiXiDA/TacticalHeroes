using System.Collections.Generic;

using Assets.Scripts.GameEngine.Domain;

namespace Assets.Scripts.Domain.Entities.Models
{
    public class Hero
    {
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

        public List<Ability> Abilities { get; set; } = new();

        public Hero(
            int id,
            int attack,
            int defence,
            int minDamage,
            int maxDamage,
            double initiative,
            int morale,
            int luck,
            string name,
            string description)
        {
            Id = id;
            Attack = attack;
            Defence = defence;
            MinDamage = minDamage;
            MaxDamage = maxDamage;
            Initiative = initiative;
            Morale = morale;
            Luck = luck;
            Name = name;
            Description = description;
        }
    }
}
