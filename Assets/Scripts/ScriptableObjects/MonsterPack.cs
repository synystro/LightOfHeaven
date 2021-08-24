using UnityEngine;

namespace LUX.LightOfHeaven {
    [CreateAssetMenu(menuName = "LOH/MonsterPack", fileName = "New MonsterPack")]
    public class MonsterPack : ScriptableObject {
        public Unit[] Monsters;
    }
}
