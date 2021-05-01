using UnityEngine;
using UnityEngine.UI;

namespace LUX {
    [CreateAssetMenu(menuName = "LOH/Unit", fileName = "New Unit")]
    public class Unit : ScriptableObject {
        public new string name;
        public Image icon;
        public GameObject charPrefabRight;
        public GameObject charPrefabLeft;
        [Header("ATTRIBUTES")]
        public int strength; // damage, stun chance
        public int stamina; // aps, evade chance
        public int vitality; // hp, magic res
        public int dexterity; // accuracy, crit chance
        public int intelligence; // mana, magic damage
        [Header("HP")]
        public int MaxHp;
        public int BaseHp;
        public int BonusHp;
        public int CurrentHp;
        [Header("MP")]
        public int MaxMp;
        public int BaseMp;
        public int BonusMp;
        public int CurrentMp;
        [Header("AP")]
        public int MaxAp;
        public int BaseAp;
        public int BonusAp;
        public int CurrentAp;
        [Header("PHYSICAL ATK")]
        public int AtkDamage;
        public int BaseAtkDamage;
        public int BonusAtkDamage;
        public int AtkRange;
        public int BaseAtkRange;
        public int BonusAtkRange;
        public int Accuracy;
        public int BaseAccuracy;
        public int BonusAccuracy;
        [Header("MAGICAL ATK")]
        public int MgcDamage;
        public int BaseMgcDamage;
        public int BonusMgcDamage;
        [Header("DEFENSE")]
        public int Armor;
        public int BaseArmor;
        public int BonusArmor;
        public int MagicResistance;
        public int BaseMagicResistance;
        public int BonusMagicResistance;        
        public int Poise;
        public int BasePoise;
        public int BonusPoise;
        [Header("CHANCES")]
        public int Evasion;
        public int BaseEvasion;
        public int BonusEvasion;
        public int CritChance;
        public int BaseCritChance;
        public int BonusCritChance;
        public int StunChance;
        public int BaseStunChance;
        public int BonusStunChance;
        public int LethalChance;
        public int BaseLethalChance;
        public int BonusLethalChance;

        public void Reset() {
            // hp
            MaxHp = BaseHp + BonusHp + (vitality * 10);
            CurrentHp = MaxHp;
            // mp
            MaxMp = BaseMp + BonusMp + (intelligence * 10);
            CurrentMp = MaxMp;
            // ap
            MaxAp = BaseAp + BonusAp + Mathf.FloorToInt(stamina / 5);
            CurrentAp = MaxAp;
            // atk
            AtkDamage = BaseAtkDamage + BonusAtkDamage + strength;
            AtkRange = BaseAtkRange + BonusAtkRange;
            // mgc
            MgcDamage = BaseMgcDamage + BonusMgcDamage + intelligence;
            MagicResistance = BaseMagicResistance + BonusMagicResistance + vitality;
            // chances
            Evasion = BaseEvasion + BonusEvasion + stamina;
            CritChance = BaseCritChance + BonusCritChance + dexterity;
            StunChance = BaseStunChance + BonusStunChance + strength;
            LethalChance = BaseLethalChance + BonusLethalChance;
        }
    }
}
