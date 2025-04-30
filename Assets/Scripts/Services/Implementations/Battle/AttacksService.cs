using Assets.Scripts.Services.Interfaces.Battle;

using R3;

using Unit = Assets.Scripts.GameEngine.Domain.Unit;

namespace Assets.Scripts.Services.Implementations.Battle
{
    public sealed class AttacksService : IAttacksService
    {
        private readonly Subject<Unit> _attackDone = new();

        public Observable<Unit> OnAttackDone => _attackDone.AsObservable();
    }
}
