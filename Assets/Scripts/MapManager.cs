using UnityEngine;

namespace LUX.LightOfHeaven {
    public class MapManager : MonoBehaviour {
        [SerializeField] private LayerMask tileLayer;
        [SerializeField] private TileData[] tiles;
        [SerializeField] private TileController[] startingTiles;
        public TileData[] Tiles => tiles;
        public TileController[] StartingTiles => startingTiles;
        private TileController[] tileControllers;
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
        public TileController GetTileByWorldPosition(Vector2 p) {
            Collider2D tileCollider = Physics2D.OverlapCircle(p, 0.2f, tileLayer);
            if (tileCollider) {
                return tileCollider.GetComponent<TileController>();
            }
            return null;
        }
        public void ResetTiles() {
            foreach(TileController tileController in tileControllers) {
                tileController.Reset();
            }
        }
    }
}