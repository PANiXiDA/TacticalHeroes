using System;

using Assets.Scripts.GameEngine.Domain.Core;

namespace Assets.Scripts.GameEngine.Domain
{
    public class Hero : GameObject
    {
        public Hero(
            Guid id,
            string name,
            string description,
            int attack,
            int defence,
            int minDamage,
            int maxDamage,
            double initiative,
            int morale,
            int luck,
            int? ownerId = null) : base(id, name, description, attack, defence, minDamage, maxDamage, initiative, morale, luck, ownerId)
        {
        }
    }
}
