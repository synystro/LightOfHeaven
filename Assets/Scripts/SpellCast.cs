using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

namespace LUX {
    public class SpellCast : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler {        
        [SerializeField] Spell spell;
        public Spell Spell => spell;
        public bool IsConsumed => isConsumed;
        private EffectData effect;
        private bool isConsumed;

        [Inject] SpellDetailsUi spellDetailsUi;
        [Inject] MapManager mapManager;        
        [Inject] UnitManager unitManager;
        [Inject] PlayerController playerController;

        private Image image;
        
        private void Awake() {
            image = this.GetComponent<Image>();
        }

        public virtual void Init() {        
            effect = new EffectData(playerController.PlayerUnitController.UnitData, spell.EffectType, spell.DamageType, spell.AmountInstant, spell.AmountOverTurns, spell.Range, spell.IgnoreObstacles, spell.Duration, spell.SFX, spell.LastsTheEntireBattle); 
            if(playerController.PlayerUnitController.UnitData.CurrentAp < spell.Cost) {
                print($"{playerController.PlayerUnitController.UnitData.name} has not enough stamina to cast {spell.name}. Needs {spell.Cost}");
                return;
            }
            Cast();                   
        }
        public void AddSpell(Spell s) {
            spell = s;            
            SetSprite();
            // set gameobject's name to the spell's name
            this.gameObject.name = spell.name;
        }
        public void Cast() {
            // deselect player
            unitManager.DeselectUnit();
            // reset tiles spell in range props            
            mapManager.ResetTiles();
            // untarget any targetted enemy units
            unitManager.UntargetEnemyUnits();                              
            // check the spell's target type
            switch(spell.TargetType) {
                case SpellTargetType.NoTarget: break;
                case SpellTargetType.TargetSelf: TargetSelf(); break;
                case SpellTargetType.TargetUnit: SelectTargetUnit(); break;
                case SpellTargetType.TargetTile: break;
                default: break;
            }           
        }
        public void ConsumeStamina() {  
            playerController.PlayerUnitController.UnitData.CurrentAp -= spell.Cost; 
        }
        public void SetIsConsumed(bool state) {
            isConsumed = state;
        }
        private void SetSprite() {
            image.sprite = spell.Image;            
        }
        private void TargetSelf() {
            playerController.PlayerUnitController.AddEffect(effect);
            playerController.SetSelectedSpellButton(this.gameObject);
            playerController.SetSelectedEffect(effect);            
            playerController.SpellSelfTarget();
        }
        private void SelectTargetUnit() {
            playerController.SetSelectedEffect(effect);
            playerController.OnSelectedTargetEffect(effect);
            playerController.SetSelectedSpellButton(this.gameObject);
        }
        public void CastOnTarget(UnitController targetUnitController) {                    
            // enemy targetting was disabled here
            
            targetUnitController.DisplayDamagePreview(false);
            targetUnitController.SetIsTarget(false);
            targetUnitController.Highlight(false);
            
            // call consume stamina on spellcast go
            ConsumeStamina();
            // if spell has an instant damage or heal, apply it now
            UnitController playerUnitController = playerController.PlayerUnitController;
            switch(spell.DamageType) {
                case DamageType.Physical: playerUnitController.DealAttack(targetUnitController, spell.AmountInstant, targetUnitController.transform.position); break;
                case DamageType.Magical: targetUnitController.ReceiveDamage(playerController.SelectedEffect.InstantDamageData); break;
                case DamageType.Piercing: targetUnitController.ReceiveDamage(playerController.SelectedEffect.InstantDamageData); break;
                default: break;
            }
            // play spell sfx
            AudioManager.PlaySFX(spell.SFX);
            // if is spell is only once per combat, consume it
            if(spell.OncePerCombat) {
                SetIsConsumed(true);
            }
        }

        public void OnPointerClick(PointerEventData eventData) {
            Init();
        }

        public void OnPointerEnter(PointerEventData eventData) {
            spellDetailsUi.Refresh(spell);     
            spellDetailsUi.gameObject.SetActive(true);       
        }

        public void OnPointerExit(PointerEventData eventData) {
            spellDetailsUi.gameObject.SetActive(false);
        }
    }
}