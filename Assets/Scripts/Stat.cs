using System;
using System.Collections.Generic;
using UnityEngine;

namespace LUX.LightOfHeaven {
    [Serializable]
    public class Stat {
        public List<StatModifier> StatModifiers => statModifiers;
        protected readonly List<StatModifier> statModifiers;

        public int BaseValue;
        public int Value {
            get {
                if (changed || BaseValue != lastBaseValue) {
                    lastBaseValue = BaseValue;
                    value = CalculateFinalValue();
                    changed = false;
                }
                return value;
            }
        }

        protected bool changed = true;
        protected int value;
        protected int lastBaseValue = int.MinValue;        

        public Stat() {
            statModifiers = new List<StatModifier>();
        }

        public Stat(int baseValue) {
            BaseValue = baseValue;
            statModifiers = new List<StatModifier>();
        }

        public virtual void AddModifier(StatModifier mod) {
            changed = true;
            statModifiers.Add(mod);
            statModifiers.Sort();
        }

        public virtual bool RemoveModifier(StatModifier mod) {            
            if(statModifiers.Remove(mod))
                return changed = true;
            return false;
        }

        public virtual bool RemoveAllModifiers(object source) {
            bool removed = false;
            for (int i = statModifiers.Count - 1; i >= 0; i--) {
                if (statModifiers[i].Source == source) {
                    changed = true;
                    removed = true;
                    statModifiers.RemoveAt(i);
                }
            }
            return removed;
        }

        protected virtual int CompareModifierOrder(StatModifier a, StatModifier b) {
            if (a.Order < b.Order)
                return -1;
            else if (a.Order > b.Order)
                return 1;
            return 0;
        }

        protected virtual int CalculateFinalValue() {
            int finalValue = BaseValue;
            int sumPercent = 0;

            for (int i = 0; i < statModifiers.Count; i++) {
                StatModifier mod = statModifiers[i];
                switch (mod.Type) {
                    case StatModType.Flat: finalValue += mod.Value; break;
                    case StatModType.Percent:
                        sumPercent += mod.Value;
                        if (i + 1 >= statModifiers.Count || statModifiers[i + 1].Type != StatModType.Percent) {
                            finalValue *= 1 + mod.Value;
                            sumPercent = 0;
                        }
                        break;
                    default: Debug.LogError("Unkown stat modifier type!"); break;
                }
            }
            return Mathf.RoundToInt(finalValue);
        }
    }
}