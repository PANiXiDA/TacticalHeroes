using System.Collections.Generic;

using Assets.Scripts.Domain.GameEngine.Domain.Core;
using Assets.Scripts.GameEngine.Domain.Enums;

namespace Assets.Scripts.GameEngine.Domain
{
    public class Effect
    {
        public EffectType Type { get; }
        public double Duration { get; private set; }
        public IReadOnlyList<StatModifier> Modifiers { get; }

        public Effect(
            EffectType type,
            double duration,
            IReadOnlyList<StatModifier> modifiers)
        {
            Type = type;
            Duration = duration;
            Modifiers = modifiers;
        }

        public void Tick(double delta) => Duration -= delta;
    }
}
