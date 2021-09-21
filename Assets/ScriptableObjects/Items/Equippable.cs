using UnityEngine;

namespace LUX.LightOfHeaven {
    public enum EquipmentType {
        Weapon,
        Shield,
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
        public int Stun;
        public int Lethal;

        public override Item GetCopy() {
            return Instantiate(this);
        }

        public override void Destroy() {
            Destroy(this);
        }

        public void Equip(UnitStats s) {
            if (Strength != 0)
                s.Strength.AddModifier(new StatModifier(Strength, StatModType.Flat, this));
            if (Stamina != 0)
                s.Stamina.AddModifier(new StatModifier(Stamina, StatModType.Flat, this));
            if (Vitality != 0)
                s.Vitality.AddModifier(new StatModifier(Vitality, StatModType.Flat, this));
            if (Dexterity != 0)
                s.Dexterity.AddModifier(new StatModifier(Dexterity, StatModType.Flat, this));
            if (Intelligence != 0)
                s.Intelligence.AddModifier(new StatModifier(Intelligence, StatModType.Flat, this));

            if (Hp != 0)
                s.Hp.AddModifier(new StatModifier(Hp, StatModType.Flat, this));
            if (Ep != 0)
                s.Ep.AddModifier(new StatModifier(Ep, StatModType.Flat, this));
            if (Sp != 0)
                s.Sp.AddModifier(new StatModifier(Sp, StatModType.Flat, this));

            if (PhyDamage != 0)
                s.PhyDamage.AddModifier(new StatModifier(PhyDamage, StatModType.Flat, this));
            if (MagDamage != 0)
                s.MagDamage.AddModifier(new StatModifier(MagDamage, StatModType.Flat, this));
            if (AtkRange != 0)
                s.AtkRange.AddModifier(new StatModifier(AtkRange, StatModType.Flat, this));
            if (AtkAccuracy != 0)
                s.AtkAccuracy.AddModifier(new StatModifier(AtkAccuracy, StatModType.Flat, this));

            if (PhyShield != 0)
                s.PhyShield.AddModifier(new StatModifier(PhyShield, StatModType.Flat, this));
            if (MagShield != 0)
                s.MagShield.AddModifier(new StatModifier(MagShield, StatModType.Flat, this));
            if (PhyArmor != 0)
                s.PhyArmor.AddModifier(new StatModifier(PhyArmor, StatModType.Flat, this));
            if (MagArmor != 0)
                s.MagArmor.AddModifier(new StatModifier(MagArmor, StatModType.Flat, this));
            if (Poise != 0)
                s.Poise.AddModifier(new StatModifier(Poise, StatModType.Flat, this));
            
            if (Evasion != 0)
                s.Evade.AddModifier(new StatModifier(Evasion, StatModType.Flat, this));
            if (Critical != 0)
                s.Crit.AddModifier(new StatModifier(Critical, StatModType.Flat, this));
            if (Stun != 0)
                s.Bash.AddModifier(new StatModifier(Stun, StatModType.Flat, this));
            if (Lethal != 0)
                s.Lethal.AddModifier(new StatModifier(Lethal, StatModType.Flat, this));
            
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
