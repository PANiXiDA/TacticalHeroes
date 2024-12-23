using Assets.Scripts.Common.Constants;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Scripts.UI.MultiPlayerLobby
{
    public class MultiPlayerLobbySceneManager : MonoBehaviour
    {
        [SerializeField]
        private GameObject _battle, _raiting, _chat;

        [SerializeField]
        private Button _profileBtn, _battleBtn, _raitingBtn, _chatBtn, _exitBtn;

        private void Awake()
        {
            if (_profileBtn != null)
            {
                _profileBtn.onClick.AddListener(OnClickProfile);
            }
            if (_battleBtn != null)
            {
                _battleBtn.onClick.AddListener(OnClickBattle);
            }
            if (_raitingBtn != null)
            {
                _raitingBtn.onClick.AddListener(OnClickRaiting);
            }
            if (_chatBtn != null)
            {
                _chatBtn.onClick.AddListener(OnClickChat);
            }
            if (_exitBtn != null)
            {
                _exitBtn.onClick.AddListener(OnClickExit);
            }
        }

        private void OnDestroy()
        {
            if (_profileBtn != null)
            {
                _profileBtn.onClick.RemoveAllListeners();
            }
            if (_battleBtn != null)
            {
                _battleBtn.onClick.RemoveAllListeners();
            }
            if (_raitingBtn != null)
            {
                _raitingBtn.onClick.RemoveAllListeners();
            }
            if (_chatBtn != null)
            {
                _chatBtn.onClick.RemoveAllListeners();
            }
            if (_exitBtn != null)
            {
                _exitBtn.onClick.RemoveAllListeners();
            }
        }

        public void OnClickProfile()
        {
            SceneManager.LoadScene(SceneConstants.ProfileScene);
        }

        public void OnClickBattle()
        {
            _battle.SetActive(true);
            _raiting.SetActive(false);
            _chat.SetActive(false);
        }

        public void OnClickRaiting()
        {
            _battle.SetActive(false);
            _raiting.SetActive(true);
            _chat.SetActive(false);
        }

        public void OnClickChat()
        {
            _battle.SetActive(false);
            _raiting.SetActive(false);
            _chat.SetActive(true);
        }

        public void OnClickExit()
        {
            SceneManager.LoadScene(SceneConstants.MenuScene);
        }
    }
}