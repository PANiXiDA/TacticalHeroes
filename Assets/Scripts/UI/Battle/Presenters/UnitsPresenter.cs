using System.Collections.Generic;

using Assets.Scripts.Services.Interfaces.Battle;

using Cysharp.Threading.Tasks;

using R3;

using UnityEngine;

using Zenject;

using Unit = Assets.Scripts.GameEngine.Domain.Unit;

namespace Assets.Scripts.UI.Battle.Presenters
{
    public sealed class UnitsPresenter : MonoBehaviour
    {
        private readonly CompositeDisposable _disposables = new();

        [Inject] private readonly IUnitsService _unitsService;

        private void OnEnable()
        {
            _unitsService.OnUnitsSpawned
                .Subscribe(units => SpawnUnits(units).Forget())
                .AddTo(_disposables);
        }

        private void OnDestroy() => _disposables.Dispose();

        private async UniTask SpawnUnits(IReadOnlyList<Unit> units)
        {
            Debug.Log("test");
        }
    }
}
