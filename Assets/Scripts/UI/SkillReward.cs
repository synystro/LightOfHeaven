using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace LUX.LightOfHeaven {
    public class SkillReward : MonoBehaviour {
        [SerializeField] private Spell skill;
        private Button button;
        private Image icon;

        [Inject] HeroManager heroManager;

        public void SetSkill(Spell skill) {
            this.skill = skill;
            SetIcon();
        }

        private void RemoveSkill() {
            skill = null;
            SetIcon();
        }

        private void SetIcon() {
            if(skill == null) { icon.sprite = null; icon.enabled = false; return; }
            if(icon == null) { icon = this.GetComponent<Image>(); }
            icon.sprite = skill.Image;
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
            if(skill == null) return;
            heroManager.AddSkill(skill);
            RemoveSkill();
        }
    }
}
