using Assets.Scripts.Domain.GameEngine.Implementations;
using Assets.Scripts.Domain.GameEngine.Interfaces;
using Assets.Scripts.GameEngine.Implementations;
using Assets.Scripts.GameEngine.Interfaces;

using Zenject;

namespace Assets.Scripts.GameEngine.DI
{
    public sealed class GameEngineInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<IATBCalculator>().To<ATBCalculator>().AsSingle();
            Container.Bind<IDamageCalculator>().To<DamageCalculator>().AsSingle();
            Container.Bind<IGridGenerator>().To<GridGenerator>().AsSingle();
            Container.Bind<IPathFinderCalculator>().To<PathFinderCalculator>().AsSingle();
            Container.Bind<IAttackCalculator>().To<AttackCalculator>().AsSingle();
            Container.Bind<IGameObjectValidation>().To<GameObjectValidation>().AsSingle();
        }
    }
}
