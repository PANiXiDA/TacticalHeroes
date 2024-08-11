using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Managers
{
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager Instance;
        private AudioSource audioSource;

        void Awake()
        {
            Instance = this;
            audioSource = GetComponent<AudioSource>();
            audioSource.loop = true;
            audioSource.Play();
        }
    }
}
