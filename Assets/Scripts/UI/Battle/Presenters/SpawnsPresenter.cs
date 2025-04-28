using System.Collections.Generic;

using Assets.Scripts.Domain.DTO.Wrappers;
using Assets.Scripts.GameEngine.DTO.Enums;
using Assets.Scripts.Services.Interfaces.Battle;
using Assets.Scripts.UI.Battle.Views;

using Cysharp.Threading.Tasks;
using R3;

using UnityEngine;
using UnityEngine.AddressableAssets;

using Zenject;

namespace Assets.Scripts.UI.Battle.Presenters
{
    public sealed class SpawnsPresenter : MonoBehaviour
    {
        private const string AddressablePrefix = "Prefabs/Units";

        [SerializeField] GridsPresenter _grid;

        private readonly CompositeDisposable _disposables = new();

        [Inject] private readonly IBattlePreparationsService _battlePreparationsService;

        private void OnEnable()
        {
            _battlePreparationsService.OnUnitsLoaded
                .Subscribe(units => SpawnUnits(units).Forget())
                .AddTo(_disposables);
        }

        private void OnDestroy() => _disposables.Dispose();

        private async UniTask SpawnUnits(IReadOnlyList<UnitWrapper> unitWrappers)
        {
            await _grid.WhenReady;

            foreach (var unitWrapper in unitWrappers)
            {
                var gameObject = await Addressables.LoadAssetAsync<GameObject>($"{AddressablePrefix}/{unitWrapper.Unit.Name}");
                var view = Instantiate(gameObject).GetComponent<UnitView>();
                view.Init(
                    side: unitWrapper.Side,
                    unitWrapper.Unit);

                var tileView = _grid.GetView(unitWrapper.TileX, unitWrapper.TileY);
                view.transform.SetParent(tileView.transform.parent, false);
                view.transform.position = tileView.transform.position;
                view.Flip(view.Side == PlayerSide.Right);
                tileView.Data.OccupiedUnitId = unitWrapper.Unit.Id;
            }
        }
    }
}
