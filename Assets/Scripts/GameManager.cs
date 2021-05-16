using UnityEngine;

namespace LUX {
    public class GameManager : MonoBehaviour {
        private void Awake() {
            AudioManager.Init();
        }
    }
}