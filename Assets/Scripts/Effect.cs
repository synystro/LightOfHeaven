
using UnityEngine;

namespace LUX.LightOfHeaven {
    public class Effect : MonoBehaviour {
        public void ApplyEffects(UnitController unitController) {
            for (int i = unitController.ActiveEffects.Count - 1; i >= 0; i--) {
                EffectData e = unitController.ActiveEffects[i];
                ApplyEffect(e, unitController);
                if (e.LastsTheEntireBattle) { continue; } // return if the effect isn't be removed until the end of battle
                // after each turn, reduce its duration value
                e.Duration -= 1;
                // remove the effect if it has reached the end of its duration
                if (e.Duration <= 0) {
                    unitController.RemoveEffect(e);
                }
            }
        }
        private void ApplyEffect(EffectData effectData, UnitController unitController) {
            bool tookHit = false;
            switch (effectData.EffectType) {
                case EffectType.Hp: unitController.UnitStats.Hp.AddModifier(new StatModifier(effectData.AmountOverTurns, effectData.StatModType, effectData.Source)); unitController.CurrentHp += effectData.AmountOverTurns; break;
                case EffectType.Heal: unitController.Heal(effectData.AmountOverTurns); break;
                case EffectType.Damage:
                    if (effectData.OverTurnsDamageData.Amount > 0) {
                        unitController.Damage(effectData.OverTurnsDamageData);
                        tookHit = true;
                    }
                    break;
                case EffectType.Stun: unitController.Stun(); unitController.Damage(effectData.OverTurnsDamageData); break;
                default: break;
            }
            if (tookHit) {
                AudioManager.PlaySFX(effectData.TickSFX);
            }
            if (unitController.CurrentHp <= 0) {
                unitController.Die();
            }
        }
    }
}
