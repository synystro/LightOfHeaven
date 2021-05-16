namespace LUX {
    public static class DamageCalculator {
        public static int DealPhysicalDamage(int damage, Unit unit) {
            int finalDamage = damage;
            if(unit.CurrentMagicShield > 0) {
                int shieldValue = unit.CurrentMagicShield;                
                unit.CurrentMagicShield -= finalDamage;
                finalDamage -= shieldValue;
            }
            if(finalDamage > 0) {
                if(unit.CurrentMagicArmor > 0) {
                    int armorValue = unit.CurrentMagicArmor;
                    unit.CurrentMagicArmor -= finalDamage;
                    finalDamage -= armorValue;                    
                }
            }
            if(finalDamage > 0) {
                unit.CurrentHp -= finalDamage;
            }
            return finalDamage;
        }
        public static int DealMagicalDamage(int damage, Unit unit) {
            int finalDamage = damage;
            if(unit.CurrentMagicShield > 0) {
                int magicShieldValue = unit.CurrentMagicShield;                
                unit.CurrentMagicShield -= finalDamage;
                finalDamage -= magicShieldValue;
            }
            if(finalDamage > 0) {
                if(unit.CurrentMagicArmor > 0) {
                    int magicArmorValue = unit.CurrentMagicArmor;
                    unit.CurrentMagicArmor -= finalDamage;
                    finalDamage -= magicArmorValue;                    
                }
            }
            if(finalDamage > 0) {
                unit.CurrentHp -= finalDamage;
            }
            return finalDamage;
        }
    }
}