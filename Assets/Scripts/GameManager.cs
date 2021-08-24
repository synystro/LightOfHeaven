using UnityEngine;
using Zenject;

namespace LUX.LightOfHeaven {
    public class GameManager : MonoBehaviour {

        [Inject] WorldGenerator worldGenerator;
        [Inject] PlayerController playerController;
        [Inject] MapManager mapManager;
        [Inject] UnitManager unitManager;

        private void Awake() {
            AudioManager.Init();
        }
        private void Start() {
            worldGenerator.Init();            
        }
        public void ResetBattlefield() {
            // reposition player to starting point
            playerController.PlayerToStartingPosition();
            // reset tiles
            mapManager.ResetTiles();
            // reset player unit
            playerController.Reset();
            playerController.PlayerUnitController.OnTurnEnd();
            playerController.PlayerUnitController.OnTurnStart();
            // clear enemy data
            unitManager.WipeEnemies();
        }
        public void GenerateRoom(RoomType rt) {
            switch(rt) {
                case RoomType.Minions: unitManager.SpawnMinions(worldGenerator.Dimensions[(int)worldGenerator.CurrentDimension]); break;
                default: break;
            }
        }
    }
}