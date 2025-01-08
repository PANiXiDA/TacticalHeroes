using Assets.Scripts.Common.Constants;
using Assets.Scripts.Common.WebRequest.JWT;
using Assets.Scripts.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Scenes : MonoBehaviour
{
    public void ChangeScenes(string sceneName)
    {
        SceneManagerHelper.Instance.ChangeScene(sceneName);
    }

    public void OpenMultiPlayerLobby()
    {
        if (!string.IsNullOrEmpty(JwtTokenManager.AccessToken) || JwtTokenManager.LoadRefreshToken() != null)
        {
            SceneManagerHelper.Instance.ChangeScene(SceneConstants.MultiPlayerLobbyScene);
        }
        else
        {
            SceneManagerHelper.Instance.ChangeScene(SceneConstants.AuthScene);
        }
    }

    public void Quit()
    {
        Application.Quit();
    }
}
