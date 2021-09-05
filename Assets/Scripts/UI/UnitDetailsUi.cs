using UnityEngine;
using TMPro;

namespace LUX.LightOfHeaven {
    public class UnitDetailsUi : MonoBehaviour {
        [SerializeField] private GameObject detailsCanvasGO;        
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI currentHpText;
        [SerializeField] private TextMeshProUGUI currentSpText;
        [SerializeField] private TextMeshProUGUI stunText;

        public void SetDisplayState(bool state) {
            detailsCanvasGO.SetActive(state);
        }
        public void Refresh(UnitController unit) {
            nameText.text = unit.name;
            currentHpText.text = unit.CurrentHp.ToString();
            currentSpText.text = unit.CurrentSp.ToString(); 
            stunText.text = unit.IsStunned.ToString();
        }
    }
}
