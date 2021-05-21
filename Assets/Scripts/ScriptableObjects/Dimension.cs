using UnityEngine;

namespace LUX {
    [CreateAssetMenu(menuName = "LOH/Dimension", fileName = "New Dimension")]
    public class Dimension : ScriptableObject {
        public MonsterPack[] MinionPacks;
    }
}
