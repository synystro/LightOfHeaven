using UnityEngine;

namespace LUX.LightOfHeaven {
    [CreateAssetMenu(menuName = "LOH/EquipmentDatabase", fileName = "New EquipmentDatabase")]
    public class EquipmentDatabase : ScriptableObject {
        [Header("WEAPONS")]
        [Expandable]
        public Equippable[] Weapons;
        [Header("SHIELD")]
        [Expandable]
        public Equippable[] Shields;
        [Header("HELMETS")]
        [Expandable]
        public Equippable[] Helmets;
        [Header("ARMOR")]
        [Expandable]
        public Equippable[] Armor;
    }
}
