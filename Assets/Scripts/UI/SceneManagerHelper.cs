using UnityEngine;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;

namespace Assets.Scripts.UI
{
    public class SceneManagerHelper : MonoBehaviour
    {
        [SerializeField]
        private Animator _anim;

        [SerializeField]
        private float fadeDuration = 1f;

        public static SceneManagerHelper Instance { get; private set; }

        public string CurrentScene { get; private set; }
        public string PreviousScene { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            CurrentScene = SceneManager.GetActiveScene().name;
            PreviousScene = null;
        }

        public void ChangeScene(string newSceneName)
        {
            if (newSceneName != CurrentScene)
            {
                ChangeSceneWithAnimationAsync(newSceneName).Forget();
            }
        }

        private async UniTask ChangeSceneWithAnimationAsync(string newSceneName)
        {
            if (_anim == null)
            {
                Debug.LogError("Animator not assigned!");
                return;
            }

            _anim.SetTrigger("FadeOut");

            await UniTask.Delay((int)(fadeDuration * 1000));

            PreviousScene = CurrentScene;

            await SceneManager.LoadSceneAsync(newSceneName);

            _anim.SetTrigger("FadeIn");

            CurrentScene = newSceneName;

            await UniTask.Delay((int)(fadeDuration * 1000));
        }
    }
}