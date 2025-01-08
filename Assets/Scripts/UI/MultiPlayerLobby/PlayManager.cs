using Assets.Scripts.Common.Constants;
using Assets.Scripts.Common.Helpers;
using Assets.Scripts.Infrastructure.Responses.MatchmakingeService;
using Assets.Scripts.Services.Interfaces;
using Assets.Scripts.UI;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

public class PlayManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _preloader;

    [SerializeField] 
    private MatchSettings _matchSettings;

    private IMatchmakingeService _matchmakingeService;

    private bool isSearching = false;

    [Inject]
    public void Constructor(IMatchmakingeService matchmakingeService)
    {
        _matchmakingeService = matchmakingeService;
    }

    private async void OnDestroy()
    {
        if (isSearching)
        {
            await EndSearch();
        }
    }

    public void ToggleSearch()
    {
        if (isSearching)
        {
            EndSearch().Forget();
        }
        else
        {
            StartSearch().Forget();
        }

        isSearching = !isSearching;
    }

    private async UniTask StartSearch()
    {
        _preloader.SetActive(true);

        await TaskRunner.RunWithGlobalErrorHandling(async () =>
        {
            _matchmakingeService.OnMatchFound += OnMatchFound;
            await _matchmakingeService.ConnectToMatchmaking();
        });
    }

    private async UniTask EndSearch()
    {
        _preloader.SetActive(false);

        await TaskRunner.RunWithGlobalErrorHandling(async () =>
        {
            _matchmakingeService.OnMatchFound -= OnMatchFound;
            await _matchmakingeService.DisconnectFromMatchmaking();
        });
    }

    private void OnMatchFound(MatchmakingResponse matchSettings)
    {
        _matchSettings.Initialize(matchSettings);
        SceneManagerHelper.Instance.ChangeScene(SceneConstants.LoadBattleScene);
    }
}
