using UnityEngine;

namespace LUX.LightOfHeaven {
    public class Damage : MonoBehaviour {
        public int GetPhysicalDamageOnUnit(int damage, UnitController unit) {
            int finalDamage = damage;
            if (unit.CurrentPhyShield > 0) {
                int shieldValue = unit.CurrentPhyShield;
                finalDamage -= shieldValue;
            }
            if (finalDamage > 0) {
                if (unit.CurrentPhyArmor > 0) {
                    int armorValue = unit.CurrentPhyArmor;
                    finalDamage -= armorValue;
                }
            }
            return finalDamage;
        }

        public int DealPhysicalDamageOnUnit(int damage, UnitController unit) {
            int finalDamage = damage;
            if (unit.CurrentPhyShield > 0) {
                int shieldValue = unit.CurrentPhyShield;
                unit.CurrentPhyShield -= finalDamage;
                finalDamage -= shieldValue;
            }
            if (finalDamage > 0) {
                if (unit.CurrentPhyArmor > 0) {
                    int armorValue = unit.CurrentPhyArmor;
                    unit.CurrentPhyArmor -= finalDamage;
                    finalDamage -= armorValue;
                }
            }
            return finalDamage;
        }

        public int GetMagicalDamageOnUnit(int damage, UnitController unit) {
            int finalDamage = damage;
            if (unit.CurrentMagShield > 0) {
                int magicShieldValue = unit.CurrentMagShield;
                finalDamage -= magicShieldValue;
            }
            if (finalDamage > 0) {
                if (unit.CurrentMagArmor > 0) {
                    int magicResValue = unit.CurrentMagArmor;
                    finalDamage -= magicResValue;
                }
            }
            return finalDamage;
        }

        public int DealMagicalDamageOnUnit(int damage, UnitController unit) {
            int finalDamage = damage;
            if (unit.CurrentMagShield > 0) {
                int magicShieldValue = unit.CurrentMagShield;
                unit.CurrentMagShield -= finalDamage;
                finalDamage -= magicShieldValue;
            }
            if (finalDamage > 0) {
                if (unit.CurrentMagArmor > 0) {
                    int magicResValue = unit.CurrentMagArmor;
                    unit.CurrentMagArmor -= finalDamage;
                    finalDamage -= magicResValue;
                }
            }
            return finalDamage;
        }

        public int GetDamageTaken(DamageData damageData, UnitController unit) {
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
