using Assets.Scripts.Domain.GameEngine.Domain.Core;
using Assets.Scripts.Domain.GameEngine.Domain.Enums;
using System.Collections.Generic;

using Assets.Scripts.GameEngine.Domain;
using Assets.Scripts.GameEngine.Domain.Enums;
using Assets.Scripts.Services.Interfaces.Battle;

namespace Assets.Scripts.Services.Implementations.Battle
{
    public sealed class BuffsDebuffsService : IBuffsDebuffsService
    {
        private const double Epsilon = 1e-6;
        private const double DefenceDefaultPercentAdd = 0.3;

        public void AddDefenceEffect(Unit unit, double duration)
        {
            var defenceModifier = new StatModifier(
                type: StatType.Defence,
                kind: ModifierKind.PercentAdd,
                value: DefenceDefaultPercentAdd);

            var defenceEffect = new Effect(
                type: EffectType.Defence,
                duration: duration,
                modifiers: new List<StatModifier> { defenceModifier });

            unit.Effects.Add(defenceEffect);
        }

        public void TickAllEffects(List<Unit> units, double deltaTime)
        {
            foreach (var unit in units)
            {
                unit.Effects.RemoveAll(effect =>
                {
                    effect.Tick(deltaTime);
                    return effect.Duration <= Epsilon;
                });
            }
        }
    }
}
