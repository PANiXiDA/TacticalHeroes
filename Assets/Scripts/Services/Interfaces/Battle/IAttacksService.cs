using R3;

using Unit = Assets.Scripts.GameEngine.Domain.Unit;

namespace Assets.Scripts.Services.Interfaces.Battle
{
    public interface IAttacksService
    {
        Observable<Unit> OnAttackDone { get; }
    }
}
