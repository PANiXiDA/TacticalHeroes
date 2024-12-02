using Assets.Scripts.Common.Constants;
using Assets.Scripts.Common.WebRequest.JWT;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Scenes : MonoBehaviour
{
    public void ChangeScenes(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void OpenMultiPlayerLobby()
    {
        if (!string.IsNullOrEmpty(JwtTokenManager.AccessToken) || JwtTokenManager.LoadRefreshToken() != null)
        {
            SceneManager.LoadScene(SceneConstants.MultiPlayerLobbyScene);
        }
        else
        {
            SceneManager.LoadScene(SceneConstants.AuthScene);
        }
    }

    public void Quit()
    {
        Application.Quit();
    }
}
