using UnityEngine;
using TMPro;

namespace LUX {
    public class AttackHighlight : MonoBehaviour {
        [SerializeField] private TextMeshPro damageText;
        
        public void SetDamageValue(int damage) {
            // never display negative numbers //
            if(damage <= 0) {
                damage = 0;
            }
            damageText.text = damage.ToString();
        }
    }
}