using UnityEngine;

namespace LUX {
    public static class AudioManager {
        private static GameObject audioManagerGO;
        private static GameObject sfxGO;
        private static AudioSource sfxAS;

        public static void Init() {
            audioManagerGO = GameObject.FindGameObjectWithTag("AudioManager");
        }

        public static void PlaySFX(AudioClip audio) {
            if(sfxGO == null) {
                sfxGO = new GameObject("SFX");
                sfxAS = sfxGO.AddComponent<AudioSource>();
            }
            sfxAS.volume = 0.5f;
            sfxAS.PlayOneShot(audio);
        }
    }
}
