using System.Collections.Generic;
using UnityEngine;

namespace LUX {
    public class MapManager : MonoBehaviour {
        [SerializeField] private TilePack tilePack;
        [SerializeField] private LayerMask tileLayer;
        [SerializeField] private TileData[] tiles;
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

            ApplyTilePack();
        }
        private void ApplyTilePack() {
            foreach(TileController t in tileControllers) {
                SpriteRenderer tSR = t.GetComponent<SpriteRenderer>();
                int randomTilePackIndex = Random.Range(0, tilePack.groundTiles.Count);
                tSR.sprite = tilePack.groundTiles[randomTilePackIndex];
            }
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