using System;
using UnityEngine;

namespace LUX.LightOfHeaven {
    [Serializable]
    public class UnitStats : MonoBehaviour {
        [Header("ATTRIBUTES")]
        [Tooltip("Increases Physical damage and Stun Chance")]
        public Stat Strength;
        [Tooltip("Increases SP and Evasion")]
        public Stat Stamina;
        [Tooltip("Increases HP and Magic Armor")]
        public Stat Vitality;
        [Tooltip("Increases Accuracy and Critical Attack Chance")]
        public Stat Dexterity;
        [Tooltip("Increases Energy and Magic Damage")]
        public Stat Intelligence;
        [Header("ESSENCE")]
        public Stat Hp;
        public Stat Ep;
        public Stat Sp;
        public int MaxHp { get { return Hp.Value + Vitality.Value; } }
        public int MaxEp { get { return Ep.Value + Intelligence.Value; } }
        public int MaxSp { get { return Sp.Value + Stamina.Value; } }
        [Header("OFFENSE")]   
        public Stat PhyDamage;
        public Stat MagDamage;     
        public Stat AtkRange;
        public Stat AtkAccuracy;
        public int AtkDamage { get { return PhyDamage.Value + Strength.Value; } }
        [Header("DEFENSE")]
        public Stat PhyShield;
        public Stat MagShield;
        public Stat PhyArmor;
        public Stat MagArmor;
        public Stat Poise;
        [Header("CHANCES")]
        public Stat Evade;
        public Stat Crit;
        public Stat Bash;
        public int Evasion { get { return Stamina.Value; } }
        public int Critical { get { return Dexterity.Value; } }
        public int Stun { get { return Strength.Value; } }
        public Stat Lethal;
        [Header("HEXES")]
        public Stat Potent;
        public Stat Weak;
        public Stat Fortified;
        public Stat Vulnerable;
    }
}