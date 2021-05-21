using UnityEngine;
using System.Collections.Generic;

namespace LUX {
    [RequireComponent(typeof(UnitController))]
    public class PathFinder : MonoBehaviour {
        [Header("Tile Setup")]
        [SerializeField] private LayerMask tileLayer;
        [SerializeField] private TileController currentTile;
        [SerializeField] private int distanceBetweenTiles = 2;

        private List<GameObject> reachableTiles;
        private List<GameObject> reachableEnemies;
        private int apsLeft;

        private UnitController unitController;

        private void Awake() {
            unitController = this.GetComponent<UnitController>();
            reachableTiles = new List<GameObject>();
            reachableEnemies = new List<GameObject>();                        
        }
        public List<GameObject> GetReachableTiles() {
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
                if(t.HasObstacle() == false || unitController.IsFlying) {                                     
                    t.SetAsReachable();
                    t.SetMovesLeft(tile.MovesLeft - 1);             
                    reachableTiles.Add(t.gameObject); 
                    if(t.CurrentUnit == null) {  
                        t.Highlight();                        
                    }                                   
                }
            }
            reachableTiles.Remove(tile.gameObject);

            // if there are tiles left to search, recursive it
            if(reachableTiles.Count > 0) {
                TileController tiletoSearch = reachableTiles[0].GetComponent<TileController>();
                // only search the tile if the has no obstacle
                if(tiletoSearch.HasObstacle() == false || unitController.IsFlying)
                    Search(tiletoSearch); 
            }

        }
        public List<GameObject> GetEnemiesInRangeOf(int tilesDistance, bool ignoreObstacles) {
            reachableTiles.Clear();
            reachableEnemies.Clear();
            apsLeft = tilesDistance;

            currentTile = unitController.CurrentTile.GetComponent<TileController>(); 
            currentTile.SetRangeLeft(apsLeft);  
            ScanForEnemy(currentTile, ignoreObstacles);

            return reachableEnemies;          
        }
        private void ScanForEnemy(TileController tile, bool ignoreObstacles) { 
            // return if the tile is out of range
            if(tile.RangeLeft <= 0) { return; }   

            foreach(TileController t in tile.AdjacentTiles) {
                // if tile has already been checked, skip to thee next one
                if(t.IsInSpellRange) { continue; }
                // if tile is free
                if(t.HasObstacle() == false) {
                    t.SetRangeLeft(tile.RangeLeft - 1);
                    t.SetInSpellRange();              
                    reachableTiles.Add(t.gameObject);                 
                } else if(t.CurrentUnit != null) {
                    t.SetInSpellRange();
                    if(unitController.IsEnemy == false && t.CurrentUnit.IsEnemy == true) {
                        // player detecting enemy
                        reachableEnemies.Add(t.CurrentUnit.gameObject);                 
                    } else if(unitController.IsEnemy == true && t.CurrentUnit.IsEnemy == false) {                        
                        //enemy detecting player
                        reachableEnemies.Add(t.CurrentUnit.gameObject);
                    }
                }
            }
            reachableTiles.Remove(tile.gameObject);

            // if there are tiles left to search, recursive it
            if(reachableTiles.Count > 0) {
                TileController tiletoSearch = reachableTiles[0].GetComponent<TileController>();
                // only search the tile if the has no obstacle
                if(tiletoSearch.HasObstacle() == false  || ignoreObstacles)
                    ScanForEnemy(tiletoSearch, ignoreObstacles); 
            }

        }
    }
}
