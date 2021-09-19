using UnityEngine;
using Zenject;

namespace LUX.LightOfHeaven {
    public class GameManager : MonoBehaviour {
        public string Seed;
        [SerializeField] private int generatedSeed = 0;
        public int GeneratedSeed => generatedSeed;

        [Inject] GameEventSystem gameEventSystem;
        [Inject] WorldGenerator worldGenerator;
        [Inject] PlayerController playerController;
        [Inject] MapManager mapManager;
        [Inject] UnitManager unitManager;

        private void Awake() {
            AudioManager.Init();
        }
        private void OnEnable() {            
            gameEventSystem.onBattleEnded += ResetBattlefield;
        }
        private void OnDisable() {
            gameEventSystem.onBattleEnded -= ResetBattlefield;
        }
        private void Start() {
            generatedSeed = GenerateSeed();
            Random.InitState(generatedSeed);
            worldGenerator.Init();
        }
        private int GenerateSeed() {
            if (string.IsNullOrEmpty(Seed)) {
                var currentTime = System.DateTime.Now;
                return currentTime.GetHashCode();
            } else {
                return Seed.GetHashCode();
            }
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
            switch (rt) {
                case RoomType.Minions: unitManager.SpawnMinions(worldGenerator.Dimensions[(int)worldGenerator.CurrentDimension]); break;
                case RoomType.Market: gameEventSystem.OnPeacefulRoomLoaded(); break;
                case RoomType.Shrine: gameEventSystem.OnPeacefulRoomLoaded(); break;
                default: break;
            }
        }
    }
}