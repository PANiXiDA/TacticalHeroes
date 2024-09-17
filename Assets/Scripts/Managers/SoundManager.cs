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

        public void PlayRoryBattleSound()
        {
            audioSource.clip = Resources.Load<AudioClip>("Sound/MyEnemy");
            audioSource.loop = false;
            audioSource.Play();
        }

        public async UniTaskVoid PLayFireSound()
        {
            GameObject fireSound = new GameObject("FireSound");
            AudioSource audio = fireSound.AddComponent<AudioSource>();
            audio.clip = Resources.Load<AudioClip>("Sound/Fire");
            audio.Play();
            await UniTask.Delay(3750);
            Destroy(fireSound);
        }

        public async UniTaskVoid PLayLaughSound()
        {
            GameObject laughSound = new GameObject("LaughSound");
            AudioSource audio = laughSound.AddComponent<AudioSource>();
            audio.clip = Resources.Load<AudioClip>("Sound/Laugh");
            audio.Play();
            await UniTask.Delay(2500);
            Destroy(laughSound);
        }
    }
}
