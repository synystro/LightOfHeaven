using System;
using UnityEngine;

namespace LUX {
    public enum EffectType { Damage, Stun, Hp, Mp, Ap }
    [System.Serializable]
    public class EffectData {
        public Unit Source { get; private set; }
        public int Amount;
        public int Duration;
        public bool LastsTheEntireBattle;
        public EffectType EffectType;
        public DamageType DamageType;
        public AudioClip TickSFX;

        public event EventHandler<Unit> OnBeginEffect;
        public event EventHandler<Unit> OnEndEffect;

        public EffectData(Unit source, EffectType effectType, DamageType damageType, int amount, int duration, AudioClip tickSFX, bool lastsTheEntireBattle) {
            this.Source = source;
            this.EffectType = effectType;
            this.DamageType = damageType;
            this.Amount = amount;
            this.Duration = duration;
            this.LastsTheEntireBattle = lastsTheEntireBattle;
            this.TickSFX = tickSFX;
        }
    }
}
