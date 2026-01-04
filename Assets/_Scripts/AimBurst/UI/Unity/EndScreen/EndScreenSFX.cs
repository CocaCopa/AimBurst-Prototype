using UnityEngine;

namespace AimBurst.UI.Unity.EndScreen {
    [RequireComponent(typeof(AudioSource))]
    public class EndScreenSFX : MonoBehaviour {
        [SerializeField] private AudioClip winSound;
        [SerializeField] private AudioClip loseSound;

        private AudioSource audioSrc;

        private void Awake() {
            audioSrc = GetComponent<AudioSource>();
        }

        internal void PlayWinAudio() {
            audioSrc.clip = winSound;
            audioSrc.Play();
        }

        internal void PlayLoseAudio() {
            audioSrc.clip = loseSound;
            audioSrc.Play();
        }
    }
}
