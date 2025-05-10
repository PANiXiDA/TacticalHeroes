using Assets.Scripts.Services.Implementations.Battle;
using Assets.Scripts.Services.Interfaces.Battle;

using Zenject;

namespace Assets.Scripts.Services.DependencyInjection
{
    public sealed class BattleActionsInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<IBattleActionsFacade>().To<BattleActionsFacade>().AsSingle();

            Container.Bind<IPlayerColorsService>().To<PlayerColorsService>().AsSingle();
            Container.Bind<IGridsService>().To<GridsService>().AsSingle();
            Container.Bind<IBattlePreparationsService>().To<BattlePreparationsService>().AsSingle();
            Container.Bind<IATBService>().To<ATBService>().AsSingle();
            Container.Bind<IAttacksService>().To<AttacksService>().AsSingle();
            Container.Bind<IMovementsService>().To<MovementsService>().AsSingle();
            Container.Bind<IBattleTurnsService>().To<BattleTurnsService>().AsSingle();
            Container.Bind<IButtonStatesService>().To<ButtonStatesService>().AsSingle();
            Container.Bind<IBuffsDebuffsService>().To<BuffsDebuffsService>().AsSingle();
            Container.Bind<IBattleEndService>().To<BattleEndService>().AsSingle();
            Container.Bind<ISurrenderService>().To<SurrenderService>().AsSingle();
        }
    }
}
