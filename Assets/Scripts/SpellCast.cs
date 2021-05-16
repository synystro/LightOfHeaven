using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace LUX {
    public class SpellCast : MonoBehaviour {        
        [SerializeField] Spell spell;
        public Spell Spell => spell;
        private EffectData effect;
        
        //[Inject] UnitManager unitManager;
        [Inject] PlayerController playerController;

        private Image image;
        
        private void Awake() {
            image = this.GetComponent<Image>();
        }

        public virtual void Init() {
            // base code here 
            effect = new EffectData(playerController.PlayerGO.GetComponent<UnitController>().UnitData, spell.EffectType, spell.DamageType, spell.Amount, spell.Duration, spell.SFX, false); 
            Cast();                   
        }
        public void AddSpell(Spell s) {
            spell = s;            
            SetSprite();
        }
        public void Cast() {            
            //unitManager.EnemyUnits[0].GetComponent<UnitController>().AddEffect(effect);
            switch(spell.TargetType) {
                case SpellTargetType.NoTarget: break;
                case SpellTargetType.TargetUnit: SelectTargetUnit(); break;
                case SpellTargetType.TargetTile: break;
                default: break;
            }           
            //DeactivateSpellButton();
        }
        private void SetSprite() {
            image.sprite = spell.Image;            
        }
        private void SelectTargetUnit() {
            playerController.SetSelectedTargetEffect(effect);
            playerController.SetSelectedSpellButton(this.gameObject);
        }
        private void DeactivateSpellButton() {
            this.gameObject.SetActive(false);
        }
    }
}