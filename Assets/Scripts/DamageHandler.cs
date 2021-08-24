namespace LUX.LightOfHeaven {
    public static class DamageHandler {
        public static int GetPhysicalDamageOnUnit(int damage, Unit unit) {
            int finalDamage = damage;
            if(unit.CurrentShield > 0) {
                int shieldValue = unit.CurrentShield;
                finalDamage -= shieldValue;
            }
            if(finalDamage > 0) {
                if(unit.CurrentArmor > 0) {
                    int armorValue = unit.CurrentArmor;
                    finalDamage -= armorValue;                    
                }
            }
            return finalDamage;
        }
        public static int DealPhysicalDamageOnUnit(int damage, Unit unit) {
            int finalDamage = damage;
            if(unit.CurrentShield > 0) {
                int shieldValue = unit.CurrentShield;         
                unit.CurrentShield -= finalDamage;
                finalDamage -= shieldValue;
            }
            if(finalDamage > 0) {
                if(unit.CurrentArmor > 0) {
                    int armorValue = unit.CurrentArmor;
                    unit.CurrentArmor -= finalDamage;
                    finalDamage -= armorValue;                    
                }
            }            
            return finalDamage;
        }
        public static int GetMagicalDamageOnUnit(int damage, Unit unit) {
            int finalDamage = damage;
            if(unit.CurrentMagicShield > 0) {
                int magicShieldValue = unit.CurrentMagicShield;
                finalDamage -= magicShieldValue;
            }
            if(finalDamage > 0) {
                if(unit.CurrentMagicArmor > 0) {
                    int magicResValue = unit.CurrentMagicArmor;
                    finalDamage -= magicResValue;                    
                }
            }
            return finalDamage;
        }
        public static int DealMagicalDamageOnUnit(int damage, Unit unit) {
            int finalDamage = damage;
            if(unit.CurrentMagicShield > 0) {
                int magicShieldValue = unit.CurrentMagicShield;                
                unit.CurrentMagicShield -= finalDamage;
                finalDamage -= magicShieldValue;
            }
            if(finalDamage > 0) {
                if(unit.CurrentMagicArmor > 0) {
                    int magicResValue = unit.CurrentMagicArmor;
                    unit.CurrentMagicArmor -= finalDamage;
                    finalDamage -= magicResValue;                    
                }
            }
            return finalDamage;
        }
    }
}