using System.Collections.Generic;
using UnityEngine;

namespace LUX {
    public class MapManager : MonoBehaviour {
        [SerializeField] LayerMask tileLayer;
        [SerializeField] TileData[] tiles;
        public TileData[] Tiles => tiles;
        private TileController[] tileControllers;
        private const int mapWidth = 10;
        public int MapWidth => mapWidth;
        public float GetDistanceBetweenTiles() { return Vector2.Distance(tiles[0].GetPosition(), tiles[1].GetPosition() ); }

        private void Awake() {
            GatherTiles();
        }
        private void GatherTiles() {
            tiles = GetComponentsInChildren<TileData>();
            tileControllers = GetComponentsInChildren<TileController>();
        }
        public int GetTileCount() {
            return tiles.Length;
        }
        public TileData GetTileByIndex(int index) {
            return tiles[index];
        }
        public void ResetTiles() {
            foreach(TileController tileController in tileControllers) {
                tileController.Reset();
            }
        }
    }
}