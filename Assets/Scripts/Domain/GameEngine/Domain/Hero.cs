using System;

using Assets.Scripts.GameEngine.Domain.Core;

namespace Assets.Scripts.GameEngine.Domain
{
    public class Hero : GameObject
    {
        public Hero(
            Guid id,
            int attack,
            int defence,
            int minDamage,
            int maxDamage,
            double initiative,
            int morale,
            int luck) : base(id, attack, defence, minDamage, maxDamage, initiative, morale, luck)
        {
        }
    }
}
