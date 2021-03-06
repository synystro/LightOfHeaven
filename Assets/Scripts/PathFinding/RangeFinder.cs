using UnityEngine;
using System.Collections.Generic;

namespace LUX.LightOfHeaven {
    [RequireComponent(typeof(UnitController))]
    public class RangeFinder : MonoBehaviour {
        [Header("Tile Setup")]
        [SerializeField] private LayerMask tileLayer;
        [SerializeField] private TileController currentTile;

        private List<GameObject> reachableTiles;
        private List<GameObject> reachableEnemies;
        private List<GameObject> reachableDestructibles;
        private int apsLeft;

        private UnitController unitController;

        private void Awake() {
            unitController = this.GetComponent<UnitController>();
            reachableTiles = new List<GameObject>();
            reachableEnemies = new List<GameObject>();  
            reachableDestructibles = new List<GameObject>();                      
        }
        public List<GameObject> GetReachableTiles() {
            reachableTiles.Clear();
            apsLeft = unitController.CurrentSp;

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
        public List<GameObject> GetEnemiesInRangeOf(TileController tile, int range, bool ignoreObstacles) {
            reachableTiles.Clear();
            reachableEnemies.Clear();
            apsLeft = range;

            currentTile = tile;
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
