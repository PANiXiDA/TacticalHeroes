using Assets.Scripts.Services.Interfaces.Battle.States.Core;

using UnityEngine;

using Zenject;

namespace Assets.Scripts.UI.Battle.Core
{
    public sealed class EntryPoint : MonoBehaviour
    {
        [Inject] private readonly IBattleStateMachine _battleStateMachine;

        private async void Start()
        {
            await _battleStateMachine.ChangeStateAsync(GameState.GenerateGrid);
        }
    }
}
