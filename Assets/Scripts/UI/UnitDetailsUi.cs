using UnityEngine;
using TMPro;

namespace LUX {
    public class UnitDetailsUi : MonoBehaviour {
        [SerializeField] private GameObject detailsCanvasGO;        
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI currentHpText;
        [SerializeField] private TextMeshProUGUI stunText;

        public void SetDisplayState(bool state) {
            detailsCanvasGO.SetActive(state);
        }
        public void Refresh(Unit unit) {
            nameText.text = unit.name;
            currentHpText.text = unit.CurrentHp.ToString(); 
            stunText.text = this.GetComponent<UnitController>().IsStunned.ToString();
        }
    }
}
