using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace LUX.LightOfHeaven {
    public class SpellsUi : MonoBehaviour {
        [SerializeField] private GameObject spellButtonPrefab;
        [SerializeField] private List<GameObject> spellButtons;
        [Inject] GameEventSystem gameEventSystem;
        [Inject] PlayerController playerController;
        private void OnEnable() {
            gameEventSystem.onPlayerSpawn += Init;
        }
        private void OnDisable() {
            gameEventSystem.onPlayerSpawn -= Init;
        }
        public void Init() {
            playerController.SetSpellsUi(this);
            GetPlayerSpells();
        }
        private void GetPlayerSpells() {
            Unit playerUnit = playerController.PlayerGO.GetComponent<UnitController>().UnitData;
            foreach(Spell s in playerUnit.Spells) {
                GameObject spellButtonGO = Instantiate(spellButtonPrefab, this.transform);
                spellButtons.Add(spellButtonGO);
                SpellCast spellCast = spellButtonGO.GetComponent<SpellCast>();
                spellCast.AddSpell(s);               
            }
        }
        public void DeactivateSpellButton(GameObject button) {            
            button.SetActive(false);  
            //CheckIfOutOfSpells();
        }
        public void CheckIfOutOfSpells() {
            bool outOfSpells = true;
            foreach(GameObject b in spellButtons) {
                if(b.activeSelf == true) {
                    outOfSpells = false;
                }
            }
            if(outOfSpells == true) {
                RefreshSpells();
            }
        }
        public void RefreshSpells() {
            foreach(GameObject sbGO in spellButtons) {
                if(sbGO.activeSelf == false && sbGO.GetComponent<SpellCast>().IsConsumed == false) {
                    sbGO.SetActive(true);
                }
            }
        }
    }
}
