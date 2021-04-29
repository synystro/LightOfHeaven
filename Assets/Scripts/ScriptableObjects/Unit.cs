using UnityEngine;
using UnityEngine.UI;

namespace LUX {
    [CreateAssetMenu(menuName = "LOH/Unit", fileName = "New Unit")]
    public class Unit : ScriptableObject {
        public new string name;
        public Image icon;
        public GameObject charPrefabRight;
        public GameObject charPrefabLeft;
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

        public void Reset() {
            // hp
            MaxHp = BaseHp + BonusHp;
            CurrentHp = MaxHp;
            // mp
            MaxMp = BaseMp + BonusMp;
            CurrentMp = MaxMp;
            // ap
            MaxAp = BaseAp + BonusAp;
            CurrentAp = MaxAp;
            // atk
            AtkDamage = BaseAtkDamage + BonusAtkDamage;
            AtkRange = BaseAtkRange + BonusAtkRange;
            // mgc
            MgcDamage = BaseMgcDamage + BonusMgcDamage;
        }
    }
}
