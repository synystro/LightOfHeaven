using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace LUX.LightOfHeaven {
    public class EquipReward : MonoBehaviour {
        [SerializeField] private Equippable equip;
        private Button button;
        private Image icon;

        [Inject] HeroManager heroManager;

        public void SetEquip(Equippable equip) {
            this.equip = equip;
            SetIcon();
        }

        private void RemoveEquip() {
            equip = null;
            SetIcon();
        }

        private void SetIcon() {
            if(equip == null) { icon.sprite = null; icon.enabled = false; return; }
            if(icon == null) { icon = this.GetComponent<Image>(); }
            icon.sprite = equip.Icon;
        }

        private void Awake() {
            button = this.GetComponent<Button>();
        }

        private void OnEnable() {
            button.onClick.AddListener(GetReward);
            if(icon)
                icon.enabled = true;
        }
        
        private void OnDisable() {
            button.onClick.RemoveAllListeners();
            if(icon)
                icon.enabled = false;
        }

        private void GetReward() {
            if(equip == null) return;
            heroManager.AddEquip(equip);
            RemoveEquip();
        }
    }
}
