using UnityEngine;
using System.Collections.Generic;

namespace LUX.LightOfHeaven {
    [RequireComponent(typeof(UnitController))]
    public class PathFinder : MonoBehaviour {
        [Header("Tile Setup")]
        [SerializeField] private LayerMask tileLayer;
        [SerializeField] private TileController currentTile;

        private List<TileController> mappedTiles;
        private List<TileController> tilesLeftToSearch;
        public List<TileController> Path => path;
        private List<TileController> path;
        private Dictionary<TileController, int> tileCostMap;

        private UnitController unitController;

        private void Awake() {
            unitController = this.GetComponent<UnitController>();
            mappedTiles = new List<TileController>();        
            tilesLeftToSearch = new List<TileController>();
            path = new List<TileController>();                 
            tileCostMap = new Dictionary<TileController, int>();              
        }
        public List<TileController> GetPathToTargetOnTile(TileController originTile, TileController targetsTile) {
            mappedTiles.Clear();
            tileCostMap.Clear();
            path.Clear();

            currentTile = originTile;
            currentTile.SetMoveCost(0);
            mappedTiles.Add(targetsTile);
            tileCostMap.Add(currentTile, currentTile.MoveCost);
            FindPossibleMovesFrom(currentTile);

            List<TileController> possibleTargetTiles =  targetsTile.AdjacentTiles;
            TileController chosenTile = null;

            foreach(TileController t in possibleTargetTiles)
                if(chosenTile == null || t.MoveCost < chosenTile.MoveCost && t.HasObstacle() == false)
                    chosenTile = t;

            mappedTiles.Clear();            

            if(chosenTile != null) {
                path.Add(chosenTile);
                GeneratePathFromTargetTile(chosenTile);
            }

            path.Reverse();

            return path;          
        }

        private void GeneratePathFromTargetTile(TileController tile) {
            if(mappedTiles.Contains(tile) || tile.MoveCost == 1) {
                return;
            }
            mappedTiles.Add(tile);
            TileController chosenTile = null;
            foreach(TileController t in tile.AdjacentTiles) {
                if(path.Contains(t)) continue;
                if((chosenTile == null || (t.MoveCost < chosenTile.MoveCost && t.MoveCost > 0)))
                    if(t.HasObstacle() == false)
                        chosenTile = t;
                    else if(t.CurrentUnit != null)
                        if(t.CurrentUnit.IsEnemy)
                            chosenTile = t;
            }
            if(chosenTile != null) {
                path.Add(chosenTile);
                GeneratePathFromTargetTile(chosenTile);
            }
        }
        
        private void FindPossibleMovesFrom(TileController tile) {
            foreach(TileController t in tile.AdjacentTiles) {                
                // if tile has already been mapped, skip to the next one
                if(mappedTiles.Contains(t)) { continue; }
                // if tile is free
                if(t.HasObstacle() == false || unitController.IsFlying) {
                    t.SetMoveCost(tile.MoveCost + 1);
                    tileCostMap.Add(t, tile.MoveCost + 1);                                 
                    mappedTiles.Add(t);
                    tilesLeftToSearch.Add(t);                               
                }
            }

            if(tilesLeftToSearch.Contains(tile))
                tilesLeftToSearch.Remove(tile);

            // if there are tiles left to search, recursive it
            if(tilesLeftToSearch.Count > 0) {
                TileController tiletoSearch = tilesLeftToSearch[0];
                // only search the tile if there's no obstacle or if the unit can fly
                if(tiletoSearch.HasObstacle() == false || unitController.IsFlying)
                    FindPossibleMovesFrom(tiletoSearch); 
            }

        }
    }
}
