using Assets.Scripts.Common.Enumerations;
using R3;

namespace Assets.Scripts.Services.Interfaces.Battle
{
    public interface IButtonStatesService
    {
        Observable<(BattleButtonType Type, bool IsActive)> OnStateChanged { get; }
        bool IsActive(BattleButtonType type);
        void Toggle(BattleButtonType type);
        void Set(BattleButtonType type, bool isActive);
    }
}
