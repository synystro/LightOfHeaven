using UnityEngine;
using System.Collections.Generic;
using Zenject;

namespace LUX {
    public class UnitManager : MonoBehaviour {
        [Header("Selection")]
        [SerializeField] private UnitController selectedUnit;
        [Header("Units Data")]
        public List<Unit> PlayerUnitsSO;
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
            if(EnemyUnits.Contains(unitToDie)) {
                EnemyUnits.Remove(unitToDie);                
            } else if(PlayerUnits.Contains(unitToDie)) {
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
            int tileToSpawnIndex = (mapManager.GetTileCount() / 2);
            foreach(Unit playerUnit in PlayerUnitsSO) {
                GameObject tileToSpawnGO = mapManager.GetTileByIndex(tileToSpawnIndex).gameObject;
                Vector3 tileToSpawnPosition = tileToSpawnGO.GetComponent<TileData>().GetPosition();
                GameObject unitGO = Instantiate(unitPrefab, tileToSpawnPosition, Quaternion.identity);
                UnitController playerUnitController = unitGO.GetComponent<UnitController>();
                playerUnitController.Setup(playerUnit, tileToSpawnGO, false);                
                PlayerUnits.Add(unitGO); 

                // set player // in another another later?
                playerController.SetPlayerGO(unitGO);
                playerController.SetPlayerUnitController(playerUnitController);

                gameEventSystem.OnPlayerSpawn();              

                tileToSpawnIndex++;                                
            }
        }
        private void SpawnEnemyUnits() {
            int tileToSpawnIndex = ((mapManager.GetTileCount() / 2) - 1) + (mapManager.MapWidth -1) - EnemyUnitsSO.Count;
            foreach(Unit enemyUnit in EnemyUnitsSO) {
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
            if(selectedUnit == null) { return; }
            selectedUnit.SetSelection(false);
            selectedUnit = null;
        }
        public void UntargetEnemyUnits() {
            foreach(GameObject eGO in EnemyUnits) {
                UnitController e = eGO.GetComponent<UnitController>();
                e.DisplayDamagePreview(false);
                e.SetIsTarget(false);
                e.Highlight(false);
            }            
        }
    }
}