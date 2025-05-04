using System;
using System.Collections.Generic;
using System.Linq;

using Assets.Scripts.Domain.GameEngine.Domain.Core;
using Assets.Scripts.Domain.GameEngine.Domain.Enums;
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

        public StatValue GetStatValue(StatType statType)
        {
            int baseValue = statType switch
            {
                StatType.Attack => Attack,
                StatType.Defence => Defence,
                StatType.Health => FullHealth,
                StatType.MinDamage => MinDamage,
                StatType.MaxDamage => MaxDamage,
                StatType.Initiative => (int)Initiative,
                StatType.Speed => Speed,
                StatType.Range => Range ?? 0,
                StatType.Arrows => Arrows ?? 0,
                StatType.Morale => Morale,
                StatType.Luck => Luck,
                _ => throw new ArgumentOutOfRangeException(nameof(statType))
            };

            double raw = ComputeRawStat(statType);
            return new StatValue(statType, raw, baseValue);
        }

        private double ComputeRawStat(StatType statType)
        {
            double baseValue = statType switch
            {
                StatType.Attack => Attack,
                StatType.Defence => Defence,
                StatType.Health => FullHealth,
                StatType.MinDamage => MinDamage,
                StatType.MaxDamage => MaxDamage,
                StatType.Initiative => Initiative,
                StatType.Speed => Speed,
                StatType.Range => Range ?? 0,
                StatType.Arrows => Arrows ?? 0,
                StatType.Morale => Morale,
                StatType.Luck => Luck,
                _ => throw new ArgumentOutOfRangeException(nameof(statType))
            };

            double flat = 0, percent = 0, percentM = 1;
            foreach (var e in Effects)
            {
                foreach (var m in e.Modifiers.Where(mod => mod.Type == statType))
                {
                    switch (m.Kind)
                    {
                        case ModifierKind.Flat: flat += m.Value; break;
                        case ModifierKind.PercentAdd: percent += m.Value; break;
                        case ModifierKind.PercentMul: percentM *= m.Value; break;
                    }
                }
            }

            return (baseValue + flat) * (1 + percent) * percentM;
        }
    }
}
