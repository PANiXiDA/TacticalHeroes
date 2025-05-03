using Assets.Scripts.Common.Enumerations;
using R3;
using System.Collections.Generic;
using Assets.Scripts.Services.Interfaces.Battle;

namespace Assets.Scripts.Services.Implementations.Battle
{
    public sealed class ButtonStatesService : IButtonStatesService
    {
        private readonly Dictionary<BattleButtonType, bool> _states = new Dictionary<BattleButtonType, bool>();
        private readonly Subject<(BattleButtonType, bool)> _changes = new Subject<(BattleButtonType, bool)>();

        public Observable<(BattleButtonType Type, bool IsActive)> OnStateChanged => _changes.AsObservable();

        public bool IsActive(BattleButtonType type)
        {
            return _states.TryGetValue(type, out var active) && active;
        }

        public void Toggle(BattleButtonType type)
        {
            Set(type, !IsActive(type));
        }

        public void Set(BattleButtonType type, bool isActive)
        {
            _states[type] = isActive;
            _changes.OnNext((type, isActive));
        }
    }
}
