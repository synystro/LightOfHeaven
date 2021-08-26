using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace LUX.LightOfHeaven {
    public enum TerrainType { Start, Hell, Purgatory, Heaven } // dream as dlc maybe?
    public enum RoomType { Unknown, Shrine, Market, Minions, Champion, Unique, Boss }
    public class Portal : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler {
        // these are shown to the player
        [SerializeField] RoomType roomType;       

        // these are hidden
        [SerializeField] private MonsterPack monsterPack;
        private bool isActive;
        // treasure?

        [Inject] GameEventSystem gameEventSystem;

        private float hoverSize = 0.2f;

        [Inject] GameManager gameManager;

        private void OnEnable() {
            gameEventSystem.onBattleEnded += OnBattleEnded;
        }
        private void OnDisable() {
            gameEventSystem.onBattleEnded -= OnBattleEnded;
        }
        private void Start() {
            isActive = false;
        }
        private void OnBattleEnded() {
            isActive = true;
        }
        private void LoadNewRoom() {
            gameManager.ResetBattlefield();
            gameManager.GenerateRoom(roomType);

            gameEventSystem.OnNextRoomLoaded();
        }
        public void AssignRoom(RoomType rt) {
            roomType = rt;
        }
        public void OnPointerClick(PointerEventData eventData) {
            // on click
            if(isActive) {
                LoadNewRoom();
                isActive = false;
            }
        }
        public void OnPointerEnter(PointerEventData eventData) {
            // on enter
            this.transform.localScale += Vector3.one * hoverSize;
        }
        public void OnPointerExit(PointerEventData eventData) {
            // on exit
            this.transform.localScale -= Vector3.one * hoverSize;
        }
    }
}