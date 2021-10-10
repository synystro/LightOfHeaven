using UnityEngine;
using System.Collections.Generic;

namespace LUX.LightOfHeaven {
    [System.Serializable]
    [CreateAssetMenu(menuName = "LOH/Unit", fileName = "New Unit")]
    public class Unit : ScriptableObject {
        public new string name;
        public Sprite icon;
        public GameObject charPrefabRight;
        public GameObject charPrefabLeft;
        [Header("EQUIPMENT")]
        public Equippable Weapon;
        public Equippable Shield;
        public Equippable Helm;
        public Equippable Armor;
        [Header("SPELLS")]
        public List<Spell> Spells;
        //[Header("EFFECTS")]
        //[SerializeField] public List<EffectData> ActiveEffects;
        [Header("FEATURES")]
        public bool Flight;
        [Header("ATTRIBUTES")]
        public int Strength;
        public int Stamina;
        public int Vitality;
        public int Dexterity;
        public int Intelligence;
        [Header("ESSENCE")]
        public int Hp;
        public int Ep;
        public int Sp;
        [Header("OFFENSE")]
        public int PhyDamage;
        public int MagDamage;
        public int AtkRange;
        public int AtkAccuracy;
        [Header("DEFENSE")]
        public int PhyShield;
        public int MagShield;
        public int PhyArmor;
        public int MagArmor;
        public int Poise;

        private void Awake() {
            //ActiveEffects = new List<EffectData>();
        }
        public void AddSpell(Spell s) {
            Spells.Add(s);
        }
        public void RemoveSpell(Spell s) {
            if(Spells.Contains(s)) {
                Spells.Remove(s);
            }
        }
        public bool Equip(Equippable equipment, UnitController owner) {
            switch(equipment.Type) {
                case EquipmentType.Weapon: return AddWeapon(equipment, owner);
                case EquipmentType.Shield: return AddShield(equipment, owner);
                case EquipmentType.Helm: return AddHelm(equipment, owner);
                case EquipmentType.Armor: return AddArmor(equipment, owner);
                default: Debug.LogError("Equipment type unkown?"); return false;
            }
        }
        private bool AddWeapon(Equippable weapon, UnitController owner) {
            if(this.Weapon)
                Weapon.Unequip(owner.UnitStats);
            this.Weapon = weapon;
            weapon.Equip(owner.UnitStats);
            return true;
        }
        private bool AddShield(Equippable shield, UnitController owner) {
            if(this.Shield)
                Shield.Unequip(owner.UnitStats);
            this.Shield = shield;
            shield.Equip(owner.UnitStats);
            return true;
        }
        private bool AddHelm(Equippable helm, UnitController owner) {
            if(this.Helm)
                Helm.Unequip(owner.UnitStats);
            this.Helm = helm;
            helm.Equip(owner.UnitStats);
            return true;
        }
        private bool AddArmor(Equippable armor, UnitController owner) {
            if(this.Armor)
                Armor.Unequip(owner.UnitStats);
            this.Armor = armor;
            armor.Equip(owner.UnitStats);
            return true;
        }
    }
}
