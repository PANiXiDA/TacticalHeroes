using Assets.Scripts.Common.Enumerations;
using Assets.Scripts.Domain.StateMachine.Interfaces;

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
