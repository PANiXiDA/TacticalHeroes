using System.Collections.Generic;

using Assets.Scripts.GameEngine.Domain;

namespace Assets.Scripts.Services.Interfaces.Battle
{
    public interface IBuffsDebuffsService
    {
        void AddDefenceEffect(Unit unit, double duration);
        void TickAllEffects(List<Unit> units, double deltaTime);
    }
}
