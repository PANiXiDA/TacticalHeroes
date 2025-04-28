using System;
using System.Collections.Generic;
using System.Linq;

using DomainUnit = Assets.Scripts.GameEngine.Domain.Unit;
using DomainEffect = Assets.Scripts.GameEngine.Domain.Effect;

namespace Assets.Scripts.Domain.Entities.Models
{
    public class Unit
    {
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

        public List<Ability> Abilities { get; set; } = new();

        public Unit(
            int id,
            int attack,
            int defence,
            int minDamage,
            int maxDamage,
            double initiative,
            int morale,
            int luck,
            int health,
            int speed,
            int? range,
            int? arrows,
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
            Health = health;
            Speed = speed;
            Range = range;
            Arrows = arrows;
            Name = name;
            Description = description;
        }

        public static DomainUnit MapToDomain(Unit entity)
        {
            return new DomainUnit(
                id: Guid.NewGuid(),
                name: entity.Name,
                description: entity.Description,
                attack: entity.Attack,
                defence: entity.Defence,
                fullHealth: entity.Health,
                minDamage: entity.MinDamage,
                maxDamage: entity.MaxDamage,
                initiative: entity.Initiative,
                speed: entity.Speed,
                range: entity.Range,
                arrows: entity.Arrows,
                morale: entity.Morale,
                luck: entity.Luck,
                count: 1)
            {
                Abilities = Ability.MapToDomains(entity.Abilities),
                Effects = new List<DomainEffect>()
            };
        }

        public static List<DomainUnit> MapToDomains(List<Unit> entities)
        {
            return entities.Select(entity => MapToDomain(entity)).ToList();
        }
    }
}
