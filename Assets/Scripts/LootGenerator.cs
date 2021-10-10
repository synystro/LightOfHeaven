using UnityEngine;

namespace LUX.LightOfHeaven {
    public class LootGenerator : MonoBehaviour {
        [SerializeField] private Spell[] spells;
        [SerializeField] private EquipmentDatabase equipment;

        public Spell GetRandomSkill() {
            return spells[Random.Range(0, spells.Length)];
        }

        public Equippable GetRandomEquipment() {
            int nEquipmentTypes = System.Enum.GetNames(typeof(EquipmentType)).Length;
            EquipmentType randomEquipmentType = (EquipmentType)Random.Range(0, nEquipmentTypes);
            switch(randomEquipmentType) {
                case EquipmentType.Weapon: return GetRandomWeapon();
                case EquipmentType.Shield: return GetRandomShield();
                case EquipmentType.Helm: return GetRandomHelm();
                case EquipmentType.Armor: return GetRandomArmor();
                default: Debug.LogError("Unknown equipment type passed trying to get a random equipment!"); return null;
            }
        }

        public Equippable GetRandomWeapon() {
            return equipment.Weapons[Random.Range(0, equipment.Weapons.Length)];
        }

        public Equippable GetRandomShield() {
            return equipment.Shields[Random.Range(0, equipment.Shields.Length)];
        }

        public Equippable GetRandomHelm() {
            return equipment.Helmets[Random.Range(0, equipment.Helmets.Length)];
        }

        public Equippable GetRandomArmor() {
            return equipment.Armor[Random.Range(0, equipment.Armor.Length)];
        }
    }
}
