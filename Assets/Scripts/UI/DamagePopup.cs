using UnityEngine;
using TMPro;

namespace LUX.LightOfHeaven {
    public class DamagePopup : MonoBehaviour {
        [Header("Damage Type Sprites")]
        [SerializeField] Sprite physicalDamageSprite;
        [SerializeField] Sprite magicalDamageSprite;
        [SerializeField] Sprite piercingDamageSprite;
        [Header("Components")]
        [SerializeField] private TextMeshPro damageText;
        [SerializeField] private SpriteRenderer spriteRenderer;
        
        public void SetDamageValue(int damage) {
            // never display negative numbers //
            if(damage <= 0) {
                damage = 0;
            }
            damageText.text = damage.ToString();
        }
        public void SetDamageSpriteByDamageType(DamageType dt) {
            if(physicalDamageSprite == null || magicalDamageSprite == null || piercingDamageSprite == null) {
                Debug.LogError("Damage sprite reference is missing here!");
                return;
            }
            switch(dt) {
                case DamageType.Physical: spriteRenderer.sprite = physicalDamageSprite; break;
                case DamageType.Magical: spriteRenderer.sprite = magicalDamageSprite; break;
                case DamageType.Piercing: spriteRenderer.sprite = piercingDamageSprite; break;
                default: break;
            }            
        }
    }
}