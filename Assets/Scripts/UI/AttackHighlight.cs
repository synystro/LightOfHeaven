using UnityEngine;
using TMPro;

namespace LUX {
    public class AttackHighlight : MonoBehaviour {
        [SerializeField] private TextMeshPro damageText;
        
        public void SetDamageValue(int damage) {
            damageText.text = damage.ToString();
        }
    }
}