using UnityEngine;

namespace LUX {
    public enum SpellTargetType { NoTarget, TargetUnit, TargetTile }
    [CreateAssetMenu(menuName = "LOH/Spell", fileName = "New Spell")]
    public class Spell : ScriptableObject {
        public Sprite Image;
        public AudioClip SFX;
        public SpellTargetType TargetType;
        public int Amount;
        public int Duration;
        public bool LastsTheEntireBattle;
        public EffectType EffectType;
        public DamageType DamageType;

        // public event EventHandler<Unit> OnBeginEffect;
        // public event EventHandler<Unit> OnEndEffect;
        
        public virtual void Start() {}

        public void Cast() {
            
        }

    }
}