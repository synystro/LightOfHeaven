﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace LUX {
    [CreateAssetMenu(menuName = "LOH/Unit", fileName = "New Unit")]
    public class Unit : ScriptableObject {
        public new string name;
        public Image icon;
        public GameObject charPrefabRight;
        public GameObject charPrefabLeft;
        [Header("SPELLS")]
        public List<Spell> Spells;
        [Header("FEATURES")]
        public bool Flight;
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
        public int MaxShield;
        public int BaseShield;
        public int BonusShield;
        public int CurrentMagicShield;
        public int MaxArmor;
        public int BaseArmor;
        public int BonusArmor;
        public int CurrentMagicArmor;
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

        public void ResetBonuses() {
            BonusHp = 0; CurrentHp -= BonusHp;
            BonusMp = 0;
            BonusAp = 0;
            BonusAtkDamage = 0;
            BonusAtkRange = 0;
            BonusMgcDamage = 0;
            BonusArmor = 0;
            BonusMagicResistance = 0;
            BonusPoise = 0;            
            BonusEvasion = 0;
            BonusCritChance = 0;
            BonusStunChance = 0;
            BonusLethalChance = 0;
        }
        public void Setup() {
            // hp
            MaxHp = BaseHp + BonusHp + (vitality * 10);            
            // mp
            MaxMp = BaseMp + BonusMp + (intelligence * 10);            
            // ap
            MaxAp = BaseAp + BonusAp + Mathf.FloorToInt(stamina / 5);            
            // atk
            AtkDamage = BaseAtkDamage + BonusAtkDamage + strength;
            // armor
            MaxShield = BaseShield + BonusShield;
            MaxArmor = BaseArmor + BonusArmor;
            // mgc
            MgcDamage = BaseMgcDamage + BonusMgcDamage + intelligence;
            MagicResistance = BaseMagicResistance + BonusMagicResistance + vitality;
            // chances
            Evasion = BaseEvasion + BonusEvasion + stamina;
            CritChance = BaseCritChance + BonusCritChance + dexterity;
            StunChance = BaseStunChance + BonusStunChance + strength;
            LethalChance = BaseLethalChance + BonusLethalChance;
        }
        public void RestoreStats() {
            CurrentHp = MaxHp;
            CurrentMp = MaxMp;
            CurrentAp = MaxAp;
            AtkRange = BaseAtkRange + BonusAtkRange; 
            CurrentMagicShield = MaxShield;
            CurrentMagicArmor = MaxArmor;           
        }
        public void RefreshBonuses() {
            // hp
            CurrentHp += BonusHp;
            // mp
            CurrentMp += BonusMp;
            // shield
            CurrentMagicShield += BonusShield;
            // ap
            // MaxAp = BaseAp + BonusAp + Mathf.FloorToInt(stamina / 5);
            // CurrentAp = MaxAp;
            // // atk
            // AtkDamage = BaseAtkDamage + BonusAtkDamage + strength;
            // AtkRange = BaseAtkRange + BonusAtkRange;
            // // mgc
            // MgcDamage = BaseMgcDamage + BonusMgcDamage + intelligence;
            // MagicResistance = BaseMagicResistance + BonusMagicResistance + vitality;
            // // chances
            // Evasion = BaseEvasion + BonusEvasion + stamina;
            // CritChance = BaseCritChance + BonusCritChance + dexterity;
            // StunChance = BaseStunChance + BonusStunChance + strength;
            // LethalChance = BaseLethalChance + BonusLethalChance;
        }
        public void RestoreAfterTurn() {
            CurrentAp = MaxAp;
            CurrentMagicShield = MaxShield;
        }
        public void AddSpell(Spell s) {
            Spells.Add(s);
        }
        public void RemoveSpell(Spell s) {
            if(Spells.Contains(s)) {
                Spells.Remove(s);
            }
        }
    }
}
