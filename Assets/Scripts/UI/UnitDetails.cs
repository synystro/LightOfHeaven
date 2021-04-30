using UnityEngine;
using TMPro;

namespace LUX {
    public class UnitDetails : MonoBehaviour {
        [SerializeField] private GameObject detailsCanvasGO;        
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI currentHpText;

        public void SetDetailsCanvasState(bool state) {
            detailsCanvasGO.SetActive(state);
        }

        public void Refresh(Unit unit) {
            nameText.text = unit.name;
            currentHpText.text = unit.CurrentHp.ToString();         
        }
    }
}
