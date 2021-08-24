using System;
using UnityEngine;

namespace LUX.LightOfHeaven {
    public enum EffectType { Damage, Heal, Stun, Hp, Mp, Ap }
    [System.Serializable]
    public class EffectData {
        public Unit Source { get; private set; }
        public int AmountInstant;
        public int AmountOverTurns;
        public int Range;
        public bool IgnoreObstacles;
        public int Duration;
        public bool LastsTheEntireBattle;
        public EffectType EffectType;
        public DamageType DamageType;
        public AudioClip TickSFX;
        public DamageData InstantDamageData => instantDamageData;
        public DamageData OverTurnsDamageData => overTurnsDamageData;

        public event EventHandler<Unit> OnBeginEffect;
        public event EventHandler<Unit> OnEndEffect;

        private DamageData instantDamageData;
        private DamageData overTurnsDamageData;

        public EffectData(Unit source, EffectType effectType, DamageType damageType, int amountInstant, int amountOverTurns, int range, bool ignoreObstacles, int duration, AudioClip tickSFX, bool lastsTheEntireBattle) {
            this.Source = source;
            this.EffectType = effectType;
            this.DamageType = damageType;
            this.AmountInstant = amountInstant;
            this.AmountOverTurns = amountOverTurns;
            this.Range = range;
            this.IgnoreObstacles = ignoreObstacles;
            this.Duration = duration;
            this.LastsTheEntireBattle = lastsTheEntireBattle;
            this.TickSFX = tickSFX;

            CreateDamageData();
        }
        private void CreateDamageData() {
            instantDamageData = new DamageData(this.Source, this.AmountInstant, this.DamageType, this.Source.CritChance, this.Source.StunChance, this.Source.StunChance);
            overTurnsDamageData = new DamageData(this.Source, this.AmountOverTurns, this.DamageType, this.Source.CritChance, this.Source.StunChance, this.Source.StunChance);
        }
    }
}
