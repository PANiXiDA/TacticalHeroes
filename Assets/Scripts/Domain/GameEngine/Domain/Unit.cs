using System;
using System.Collections.Generic;

using Assets.Scripts.GameEngine.Domain.Core;

namespace Assets.Scripts.GameEngine.Domain
{
    public class Unit : GameObject
    {
        public int FullHealth { get; set; }
        public int CurrentHealth { get; set; }
        public double CurrentInitiative { get; set; }
        public int Speed { get; set; }
        public int? Range { get; set; }
        public int? Arrows { get; set; }
        public int Count { get; set; }

        public List<Ability> Abilities { get; set; }
        public List<Effect> Effects { get; set; }

        public Unit(
            Guid id,
            string name,
            string description,
            int attack,
            int defence,
            int fullHealth,
            int minDamage,
            int maxDamage,
            double initiative,
            int speed,
            int? range,
            int? arrows,
            int morale,
            int luck,
            int count,
            int? ownerId = null) : base(id, name, description, attack, defence, minDamage, maxDamage, initiative, morale, luck, ownerId)
        {
            CurrentInitiative = initiative;
            FullHealth = fullHealth;
            CurrentHealth = fullHealth;
            Speed = speed;
            Range = range;
            Arrows = arrows;
            Count = count;;
        }
    }
}
