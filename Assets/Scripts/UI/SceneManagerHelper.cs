using UnityEngine.SceneManagement;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public class SceneManagerHelper : MonoBehaviour
    {
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
            SceneManager.activeSceneChanged += OnSceneChanged;
            CurrentScene = SceneManager.GetActiveScene().name;
            PreviousScene = null;
        }

        private void OnSceneChanged(Scene oldScene, Scene newScene)
        {
            if (!string.IsNullOrEmpty(newScene.name))
            {
                PreviousScene = CurrentScene;
                CurrentScene = newScene.name;
            }
        }

        private void OnDestroy()
        {
            SceneManager.activeSceneChanged -= OnSceneChanged;
        }
    }
}
