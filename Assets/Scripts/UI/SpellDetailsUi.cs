using UnityEngine;
using TMPro;

namespace LUX.LightOfHeaven {
    public class SpellDetailsUi : MonoBehaviour {        
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI effectTypeText;
        [SerializeField] private TextMeshProUGUI costText;
        [SerializeField] private TextMeshProUGUI rangeText;
        [SerializeField] private TextMeshProUGUI targetTypeText;
        [SerializeField] private TextMeshProUGUI ignoreObstaclesText;
        [SerializeField] private TextMeshProUGUI damageTypeText;
        [SerializeField] private TextMeshProUGUI amountInstantText;
        [SerializeField] private TextMeshProUGUI amountOverTurnsText;
        [SerializeField] private TextMeshProUGUI durationText;

        public void SetDisplayState(bool state) {
            this.gameObject.SetActive(state);
        }
        public void Refresh(Spell spell) {
            nameText.text = spell.name;
            effectTypeText.text = spell.EffectType.ToString();
            costText.text = spell.Cost.ToString();
            rangeText.text = spell.Range == 0 ? "Self" : spell.Range.ToString();
            targetTypeText.text = spell.TargetType.ToString();
            ignoreObstaclesText.text = spell.IgnoreObstacles.ToString();
            damageTypeText.text = spell.EffectType == EffectType.Heal ? "Heal" : damageTypeText.text = spell.DamageType.ToString();
            amountInstantText.text = spell.AmountInstant.ToString();
            amountOverTurnsText.text = spell.AmountOverTurns.ToString();
            durationText.text = spell.LastsTheEntireBattle ? "Combat" : spell.Duration.ToString();            
        }
    }
}
