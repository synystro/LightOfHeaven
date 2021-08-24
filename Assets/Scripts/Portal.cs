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
        // treasure?

        private float hoverSize = 0.2f;

        [Inject] WorldGenerator worldGenerator;
        [Inject] GameManager gameManager;

        private void LoadNewRoom() {
            gameManager.ResetBattlefield();
            gameManager.GenerateRoom(roomType);
            // onLoadNewRoom()?.Invoke();
        }

        public void AssignRoom(RoomType rt) {
            roomType = rt;
        }

        public void OnPointerClick(PointerEventData eventData) {
            // on click
            LoadNewRoom();
            worldGenerator.RaiseLevel();
            // gameEvent?.OnLoadNewRomm
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