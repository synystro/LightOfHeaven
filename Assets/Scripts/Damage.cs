using UnityEngine;

namespace LUX.LightOfHeaven {
    public class Damage : MonoBehaviour {
        public int GetPhysicalDamageOnUnit(int damage, Unit unit) {
            int finalDamage = damage;
            if (unit.CurrentShield > 0) {
                int shieldValue = unit.CurrentShield;
                finalDamage -= shieldValue;
            }
            if (finalDamage > 0) {
                if (unit.CurrentArmor > 0) {
                    int armorValue = unit.CurrentArmor;
                    finalDamage -= armorValue;
                }
            }
            return finalDamage;
        }
        public int DealPhysicalDamageOnUnit(int damage, Unit unit) {
            int finalDamage = damage;
            if (unit.CurrentShield > 0) {
                int shieldValue = unit.CurrentShield;
                unit.CurrentShield -= finalDamage;
                finalDamage -= shieldValue;
            }
            if (finalDamage > 0) {
                if (unit.CurrentArmor > 0) {
                    int armorValue = unit.CurrentArmor;
                    unit.CurrentArmor -= finalDamage;
                    finalDamage -= armorValue;
                }
            }
            return finalDamage;
        }
        public int GetMagicalDamageOnUnit(int damage, Unit unit) {
            int finalDamage = damage;
            if (unit.CurrentMagicShield > 0) {
                int magicShieldValue = unit.CurrentMagicShield;
                finalDamage -= magicShieldValue;
            }
            if (finalDamage > 0) {
                if (unit.CurrentMagicArmor > 0) {
                    int magicResValue = unit.CurrentMagicArmor;
                    finalDamage -= magicResValue;
                }
            }
            return finalDamage;
        }
        public int DealMagicalDamageOnUnit(int damage, Unit unit) {
            int finalDamage = damage;
            if (unit.CurrentMagicShield > 0) {
                int magicShieldValue = unit.CurrentMagicShield;
                unit.CurrentMagicShield -= finalDamage;
                finalDamage -= magicShieldValue;
            }
            if (finalDamage > 0) {
                if (unit.CurrentMagicArmor > 0) {
                    int magicResValue = unit.CurrentMagicArmor;
                    unit.CurrentMagicArmor -= finalDamage;
                    finalDamage -= magicResValue;
                }
            }
            return finalDamage;
        }

        public int GetDamageTaken(DamageData damageData, Unit unit) {
            int lethalRandom = Random.Range(0, 100);
            if (lethalRandom < damageData.LethalChance) {
                unit.CurrentHp = 0;
                print($"{damageData.Source.name} just dealt a LETHAL attack to {this.name}! Instantly killing them!");
                return 9999;
            }
            int damageTaken;
            switch (damageData.Type) {
                case DamageType.Physical:
                    int physicalDamage = damageData.Amount;
                    int critRandom = Random.Range(0, 100);
                    if (critRandom < damageData.CritChance) {
                        physicalDamage = physicalDamage * 2;
                    }
                    damageTaken = DealPhysicalDamageOnUnit(physicalDamage, unit);
                    break;
                case DamageType.Magical:
                    int magicalDamage = damageData.Amount;
                    damageTaken = DealMagicalDamageOnUnit(magicalDamage, unit);
                    break;
                case DamageType.Piercing:
                    damageTaken = damageData.Amount;
                    break;
                default:
                    damageTaken = 0;
                    break;
            }
            return damageTaken;
        }
    }
}
