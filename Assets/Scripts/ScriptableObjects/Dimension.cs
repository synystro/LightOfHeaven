using UnityEngine;

namespace LUX.LightOfHeaven {
    [CreateAssetMenu(menuName = "LOH/Dimension", fileName = "New Dimension")]
    public class Dimension : ScriptableObject {
        public MonsterPack[] MinionPacks;
    }
}
