using UnityEngine;

namespace LUX.LightOfHeaven {
    public enum SpellTargetType { NoTarget, TargetSelf, TargetUnit, TargetTile }
    [CreateAssetMenu(menuName = "LOH/Spell", fileName = "New Spell")]
    public class Spell : ScriptableObject {
        public Sprite Image;
        public AudioClip SFX;
        public SpellTargetType TargetType;
        public int Cost;
        public int Range;
        public bool IgnoreObstacles;
        public int AmountInstant;
        public int AmountOverTurns;
        public int Duration;
        public bool LastsTheEntireBattle;
        public bool OncePerCombat;
        public EffectType EffectType;
        public DamageType DamageType;

        // public event EventHandler<Unit> OnBeginEffect;
        // public event EventHandler<Unit> OnEndEffect;
        
        public virtual void Start() {}

        public void Cast() {
            
        }

    }
}