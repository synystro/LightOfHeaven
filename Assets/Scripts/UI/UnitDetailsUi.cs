using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using System;
using System.Collections.Generic;

namespace LUX.LightOfHeaven {
    [Serializable]
    public struct SpritedIntent {
        public IntentType intent;
        public Sprite sprite;
    }
    public class UnitDetailsUi : MonoBehaviour {

        public Slider HpSlider => hpSlider;
        [SerializeField] private Slider hpSlider;
        public List<SpritedIntent> IntentSprites => intentSprites;
        [SerializeField] private List<SpritedIntent> intentSprites;
        [SerializeField] private GameObject detailsCanvasGO; 
        [SerializeField] private Transform unitDetails;       
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI currentHpText;
        [SerializeField] private TextMeshProUGUI currentSpText;
        [SerializeField] private TextMeshProUGUI stunText;

        public void SetDisplayState(bool state) {
            unitDetails.position = Mouse.current.position.ReadValue();
            //Vector3 mousePos = Mouse.current.position.ReadValue();
            //unitDetails.position = Utilities.KeepUiOnScreen(unitDetails, detailsCanvasGO, mousePos);
            detailsCanvasGO.SetActive(state);
        }
        public void Refresh(UnitController unit) {
            HpSlider.value = (float)unit.CurrentHp / unit.UnitStats.MaxHp;
            nameText.text = unit.name;
            currentHpText.text = unit.CurrentHp.ToString();
            currentSpText.text = unit.CurrentSp.ToString(); 
            stunText.text = unit.IsStunned.ToString();
        }
    }
}
