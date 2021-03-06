using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace LUX.LightOfHeaven {
    public class SpellsUi : MonoBehaviour {
        [SerializeField] private GameObject spellButtonPrefab;
        [SerializeField] private List<GameObject> spellButtons;
        [Inject] GameEventSystem gameEventSystem;
        [Inject] HeroManager heroManager;
        [Inject] PlayerController playerController;

        private Image panelBg;
        private void OnEnable() {
            gameEventSystem.onBattleStarted += Init;
            gameEventSystem.onSkillAdded += AddSkillToPanel;
        }
        private void OnDisable() {
            gameEventSystem.onBattleStarted -= Init;
            gameEventSystem.onSkillAdded -= AddSkillToPanel;
        }
        private void Awake() {
            panelBg = this.GetComponent<Image>();
        }
        public void Init() {
            playerController.SetSpellsUi(this);
            panelBg.enabled = true;
            GetPlayerSpells();
        }
        private void AddSkillToPanel(Spell skill) {
            GameObject spellButtonGO = Instantiate(spellButtonPrefab, this.transform);
            spellButtons.Add(spellButtonGO);
            SpellCast spellCast = spellButtonGO.GetComponent<SpellCast>();
            spellCast.AddSpell(skill);
        }
        private void GetPlayerSpells() {
            spellButtons.Clear();
            foreach (Spell s in heroManager.Hero.Spells) {
                AddSkillToPanel(s);
            }
        }
        public void DeactivateSpellButton(GameObject button) {
            button.SetActive(false);
        }
        public void CheckIfOutOfSpells() {
            bool outOfSpells = true;
            foreach (GameObject b in spellButtons) {
                if (b.activeSelf == true) {
                    outOfSpells = false;
                }
            }
            if (outOfSpells == true) {
                RefreshAllSkills();
            }
        }
        public void RefreshAllSkills() {
            foreach (GameObject sbGO in spellButtons) {
                if (sbGO.activeSelf == false && sbGO.GetComponent<SpellCast>().IsConsumed == false) {
                    sbGO.SetActive(true);
                }
            }
        }
    }
}
