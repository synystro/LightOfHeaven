using UnityEngine;
using System.Collections.Generic;
using Zenject;

namespace LUX {
    public class UnitManager : MonoBehaviour {
        [Header("Selection")]
        [SerializeField] private UnitController selectedUnit;
        [Header("Units Data")]
        public Unit PlayerUnitSO;        
        public List<Unit> EnemyUnitsSO;
        [Header("Units Spawned")]
        public List<GameObject> PlayerUnits;
        public List<GameObject> EnemyUnits;
        [Header("Prefabs")]
        [SerializeField] private GameObject unitPrefab;

        [Inject] private GameEventSystem gameEventSystem;
        [Inject] private MapManager mapManager;
        [Inject] private PlayerController playerController;

        private void OnEnable() {
            gameEventSystem.onUnitMove += OnUnitMove;
            gameEventSystem.onUnitDie += OnUnitDie;
        }
        private void OnUnitMove(bool isEnemy) {
            // do something about unit move depending on wether its player/friendly or enemy
        }
        private void OnUnitDie(GameObject unitToDie) {
            if (EnemyUnits.Contains(unitToDie)) {
                EnemyUnits.Remove(unitToDie);
            } else if (PlayerUnits.Contains(unitToDie)) {
                PlayerUnits.Remove(unitToDie);
            }
            print($"{unitToDie.name} just died.");
            Destroy(unitToDie);
        }
        private void Start() {
            SpawnPlayerUnits();
            SpawnEnemyUnits();
        }
        private void SpawnPlayerUnits() {
            TileController tileToSpawn = mapManager.StartingTiles[0];

            Vector3 tileToSpawnPosition = tileToSpawn.TileData.GetPosition();
            GameObject playerUnitGO = Instantiate(unitPrefab, tileToSpawnPosition, Quaternion.identity);
            UnitController playerUnitController = playerUnitGO.GetComponent<UnitController>();
            playerUnitController.Setup(PlayerUnitSO, tileToSpawn.gameObject, false);
            PlayerUnits.Add(playerUnitGO);
            // set player // in another another later?
            playerController.SetPlayerGO(playerUnitGO);
            playerController.SetPlayerUnitController(playerUnitController);
            gameEventSystem.OnPlayerSpawn();
        }
        public void PlayerOnStartingPosition() {
            playerController.PlayerGO.transform.position = mapManager.StartingTiles[0].transform.position;
        }
        public void SpawnMinions(Dimension d) {
            EnemyUnitsSO.Clear();
            foreach(Unit m in d.MinionPacks[0].Monsters) {
                EnemyUnitsSO.Add(m);
                print($"Setting {m.name} as enemy.");
            }
            SpawnEnemyUnits();
        }
        public void SpawnEnemyUnits() {
            int tileToSpawnIndex = ((mapManager.GetTileCount() / 2)) + (24) - EnemyUnitsSO.Count;
            foreach (Unit enemyUnit in EnemyUnitsSO) {
                GameObject tileToSpawnGO = mapManager.GetTileByIndex(tileToSpawnIndex).gameObject;
                Vector3 tileToSpawnPosition = tileToSpawnGO.GetComponent<TileData>().GetPosition();
                GameObject unitGO = Instantiate(unitPrefab, tileToSpawnPosition, Quaternion.identity);
                unitGO.GetComponent<UnitController>().Setup(enemyUnit, tileToSpawnGO, true);
                EnemyUnits.Add(unitGO);

                tileToSpawnIndex++;
            }
        }
        public void SetSelectedUnit(UnitController unit) {
            selectedUnit = unit;
            unit.SetSelection(true);
        }
        public UnitController GetSelectedUnit() {
            return selectedUnit;
        }
        public void DeselectUnit() {
            if (selectedUnit == null) { return; }
            selectedUnit.SetSelection(false);
            selectedUnit = null;
        }
        public void UntargetEnemyUnits() {
            foreach (GameObject eGO in EnemyUnits) {
                UnitController e = eGO.GetComponent<UnitController>();
                e.DisplayDamagePreview(false);
                e.SetIsTarget(false);
                e.Highlight(false);
            }
        }
        public void WipeEnemies() {
            EnemyUnitsSO.Clear();
            foreach(GameObject e in EnemyUnits) {
                Destroy(e);
            }
            // clear current enemies
            EnemyUnits.Clear();
        }
    }
}