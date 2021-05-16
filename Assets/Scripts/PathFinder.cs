using UnityEngine;
using System.Collections.Generic;

namespace LUX {
    [RequireComponent(typeof(UnitController))]
    public class PathFinder : MonoBehaviour {
        [Header("Tile Setup")]
        [SerializeField] private LayerMask tileLayer;
        [SerializeField] private TileController currentTile;
        [SerializeField] private int distanceBetweenTiles = 2;

        private List<UnityEngine.GameObject> reachableTiles;
        private List<UnityEngine.GameObject> reachableEnemies;
        private int apsLeft;

        private UnitController unitController;

        private void Awake() {
            unitController = this.GetComponent<UnitController>();
            reachableTiles = new List<UnityEngine.GameObject>();
            reachableEnemies = new List<UnityEngine.GameObject>();                        
        }
        public List<UnityEngine.GameObject> GetReachableTiles() {
            reachableTiles.Clear();
            apsLeft = unitController.UnitData.CurrentAp;

            currentTile = unitController.CurrentTile.GetComponent<TileController>();
            //currentTile.SetAsReachable();
            currentTile.SetMovesLeft(apsLeft);
            Search(currentTile);  

            return reachableTiles;          
        }
        private void Search(TileController tile) { 
            // return if the tile is out of range
            if(tile.MovesLeft <= 0) { return; }     

            foreach(TileController t in tile.AdjacentTiles) {                
                // if tile has already been checked, skip to thee next one
                if(t.IsReachable) { continue; }
                // if tile is free
                if(t.HasObstacle == false || unitController.IsFlying) {                                                       
                    t.SetAsReachable();
                    t.SetMovesLeft(tile.MovesLeft - 1);             
                    reachableTiles.Add(t.gameObject);                 
                }
            }
            reachableTiles.Remove(tile.gameObject);

            // if there are tiles left to search, recursive it
            if(reachableTiles.Count > 0) {
                TileController tiletoSearch = reachableTiles[0].GetComponent<TileController>();
                // only search the tile if the has no obstacle
                if(tiletoSearch.HasObstacle == false || unitController.IsFlying)
                    Search(tiletoSearch); 
            }

        }
        public List<UnityEngine.GameObject> GetReachableEnemies() {
            reachableTiles.Clear();
            reachableEnemies.Clear();
            apsLeft = unitController.UnitData.AtkRange;

            currentTile = unitController.CurrentTile.GetComponent<TileController>(); 
            currentTile.SetRangeLeft(apsLeft);  
            ScanForEnemy(currentTile);

            return reachableEnemies;          
        }
        private void ScanForEnemy(TileController tile) { 
            // return if the tile is out of range
            if(tile.RangeLeft <= 0) { return; }   

            foreach(TileController t in tile.AdjacentTiles) {
                // if tile has already been checked, skip to thee next one
                if(t.IsInAtkRange) { continue; }
                // if tile is free
                if(t.HasObstacle == false) {
                    t.SetRangeLeft(tile.RangeLeft - 1);
                    t.SetInAtkRange();                   
                    reachableTiles.Add(t.gameObject);                 
                } else if(t.CurrentUnit != null) {
                    t.SetInAtkRange();
                    if(unitController.IsEnemy == false && t.CurrentUnit.IsEnemy == true) {
                        // player detecting enemy
                        //print($"{t.CurrentUnit.UnitData.name}'s enemy unit is in {unitController.UnitData.name}'s atk range!");
                        reachableEnemies.Add(t.CurrentUnit.gameObject);
                        t.CurrentUnit.SetAttackHighlightDamage(this.unitController.UnitData.AtkDamage);
                        //t.CurrentUnit.DisplayAttackHighlight(true);                        
                    } else if(unitController.IsEnemy == true && t.CurrentUnit.IsEnemy == false) {
                        //enemy detecting player
                        //print($"{t.CurrentUnit.UnitData.name}'s player unit is in {unitController.UnitData.name}'s atk range!");
                        reachableEnemies.Add(t.CurrentUnit.gameObject);
                        t.CurrentUnit.SetAttackHighlightDamage(this.unitController.UnitData.AtkDamage);
                        //t.CurrentUnit.DisplayAttackHighlight(true); 
                    }
                }
            }
            reachableTiles.Remove(tile.gameObject);

            // if there are tiles left to search, recursive it
            if(reachableTiles.Count > 0) {
                TileController tiletoSearch = reachableTiles[0].GetComponent<TileController>();
                // only search the tile if the has no obstacle
                if(tiletoSearch.HasObstacle == false  || unitController.IsFlying)
                    ScanForEnemy(tiletoSearch); 
            }

        }
    }
}
