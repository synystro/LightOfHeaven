using System;

namespace LUX {    
    public enum EffectType { Debuff, Hp, Mp, Ap, Damage }
    public class EffectData {
        public Unit Source { get; private set; }
        public int Amount;
        public int Duration;
        public bool LastsTheEntireBattle;
        public EffectType EffectType;
        public DamageType DamageType;

        public event EventHandler<Unit> OnBeginEffect;
        public event EventHandler<Unit> OnEndEffect;

        public EffectData(Unit source, EffectType effectType, int amount, int duration, bool lastsTheEntireBattle) {
            this.Source = source;
            this.EffectType = effectType;
            this.Amount = amount;
            this.Duration = duration;
            this.LastsTheEntireBattle = lastsTheEntireBattle;
        }
        public EffectData(Unit source, DamageType damageType, int amount, int duration, bool lastsTheEntireBattle) {
            this.Source = source;
            this.EffectType = EffectType.Debuff;
            this.DamageType = damageType;
            this.Amount = amount;
            this.Duration = duration;
            this.LastsTheEntireBattle = lastsTheEntireBattle;
        }   
        
    }
}
