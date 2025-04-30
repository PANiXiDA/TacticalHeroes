using System.Collections.Generic;
using System.Linq;

using Assets.Scripts.Domain.DTO.Wrappers;
using Assets.Scripts.GameEngine.DTO.Enums;
using Assets.Scripts.Services.Interfaces.Battle;
using Assets.Scripts.UI.Battle.Views;

using Cysharp.Threading.Tasks;

using R3;

using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

using Zenject;

namespace Assets.Scripts.UI.Battle.Presenters
{
    public sealed class SpawnsPresenter : MonoBehaviour
    {
        private const string AddressablePrefix = "Prefabs/Units";

        [Inject] GridsPresenter _grid;

        [Inject] private readonly DiContainer _container;
        [Inject] private readonly IBattlePreparationsService _battlePreparationsService;

        private readonly CompositeDisposable _disposables = new();

        private void OnEnable()
        {
            BindStreams();
        }

        private void OnDestroy() => _disposables.Dispose();

        private void BindStreams()
        {
            _battlePreparationsService.OnUnitsLoaded
                .Subscribe(units => SpawnUnits(units).Forget())
                .AddTo(_disposables);
        }

        private async UniTask SpawnUnits(IReadOnlyList<UnitWrapper> unitWrappers)
        {
            await _grid.WhenReady;

            var handles = new List<AsyncOperationHandle<GameObject>>(unitWrappers.Count);
            {
                foreach (var unitWrapper in unitWrappers)
                {
                    var handle = Addressables.LoadAssetAsync<GameObject>(
                        $"{AddressablePrefix}/{unitWrapper.Unit.Name}"
                    );
                    handles.Add(handle);
                }

                await UniTask.WhenAll(handles.Select(handle => handle.ToUniTask()));

                for (int i = 0; i < unitWrappers.Count; i++)
                {
                    var wrapper = unitWrappers[i];
                    var handle = handles[i];
                    var prefab = handle.Result;

                    var tileView = _grid.GetTile(wrapper.TileX, wrapper.TileY);

                    var view = _container.InstantiatePrefabForComponent<UnitView>(
                        prefab,
                        tileView.transform,
                        new object[] { wrapper.Unit, wrapper.Side }
                    );

                    view.Flip(wrapper.Side == PlayerSide.Right);

                    Addressables.Release(handle);
                }
            }
        }
    }
}
