using UnityEngine;

namespace LUX.LightOfHeaven {
    public enum EquipmentType {
        Weapon,
        Helm,
        Armor,
        Boots
    }
    [CreateAssetMenu(menuName = "LOH/Equipment", fileName = "New Equipment")]
    public class Equippable : Item {
        public EquipmentType Type;
        [Space]
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
        [Header("CHANCES")]
        public int Evasion;
        public int Critical;
        public int Bash;
        public int Lethal;

        public override Item GetCopy() {
            return Instantiate(this);
        }

        public override void Destroy() {
            Destroy(this);
        }

        public void Equip(UnitStats us) {
            if (Strength != 0)
                us.Strength.AddModifier(new StatModifier(Strength, StatModType.Flat, this));
            if (Stamina != 0)
                us.Stamina.AddModifier(new StatModifier(Stamina, StatModType.Flat, this));
            if (Vitality != 0)
                us.Vitality.AddModifier(new StatModifier(Vitality, StatModType.Flat, this));
            if (Dexterity != 0)
                us.Dexterity.AddModifier(new StatModifier(Dexterity, StatModType.Flat, this));
            if (Intelligence != 0)
                us.Intelligence.AddModifier(new StatModifier(Intelligence, StatModType.Flat, this));
        }

        public void Unequip(UnitStats us) {
            us.Strength.RemoveAllModifiers(this);
            us.Stamina.RemoveAllModifiers(this);
            us.Vitality.RemoveAllModifiers(this);
            us.Dexterity.RemoveAllModifiers(this);
            us.Intelligence.RemoveAllModifiers(this);            
        }

        public override string GetItemType() {
            return Type.ToString();
        }
    }
}
