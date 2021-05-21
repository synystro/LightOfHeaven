using UnityEngine;
using System.Collections.Generic;

namespace LUX {
    [CreateAssetMenu(menuName = "LOH/TilePack", fileName = "New TilePack")]
    public class TilePack : ScriptableObject {
        public Sprite[] groundTiles;
        public Sprite[] impassableTiles;
    }
}
