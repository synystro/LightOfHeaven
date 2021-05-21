using UnityEngine;
using System.Collections.Generic;
using Zenject;
using System.Collections;

namespace LUX {
    public class AiController : MonoBehaviour {
        [SerializeField] private UnitController selectedUnitAi;
        [SerializeField] private Spell selectedSpell;
        private EffectData selectedEffect;
        [SerializeField] private bool selectedUnitAiValidPath;
        [SerializeField] private LayerMask tileMask;

        private bool selectedUnitAttacked;

        private const float unitTurnTime = 0.1f;

        [Inject] private GameEventSystem gameEventSystem;
        [Inject] private UnitManager unitManager;
        [Inject] private TurnManager turnManager;
        [Inject] private MapManager mapManager;

        [Inject] PathFindingGrid grid;

        IEnumerator WaitUnitTurn(float seconds) {
            int i = 0;
            while(i < unitManager.EnemyUnits.Count) {
                selectedUnitAi = unitManager.EnemyUnits[i].GetComponent<UnitController>();
                StartUnitActionPhase();
                yield return new WaitForSeconds(seconds);
                // unit's end turn
                OnUnitEndTurn();                 
                i++;                                
            }
            turnManager.EndTurn(); // end this turn
            yield return null;                     
        }
        IEnumerator KillWait(float delay, Task t) {
            yield return new WaitForSeconds(delay);
            t.Stop();
        }

        private void OnEnable() {
            gameEventSystem.onTurnEnd += Reset;
        }
        private void OnDisable() {
            gameEventSystem.onTurnEnd -= Reset;
        }
        private void OnUnitEndTurn() {
            mapManager.ResetTiles();
            selectedUnitAi.SetSelection(false);
        }
        public void Reset() {
            selectedUnitAi = null;                        
        }
        public void StartTurn() {
            // if there are no enemy units, actually end the battle
            if(unitManager.EnemyUnits.Count <= 0) {
                turnManager.EndTurn();
                return;
            }
            Task waitTask = new Task(WaitUnitTurn(unitTurnTime));
            new Task(KillWait((unitTurnTime * unitManager.EnemyUnits.Count) + unitTurnTime, waitTask));
        }
        private void StartUnitActionPhase() {
            selectedUnitAi.SetSelection(true);
            selectedUnitAiValidPath = true;

            // decide player unit to attack??

            // try to attack if player unit is in attack range
            selectedUnitAttacked = false;
            selectedUnitAi.GetEnemiesInRangeOf(selectedUnitAi.UnitData.AtkRange, false, selectedUnitAi.UnitData.Flight);
            
            if(IsTargetInRange()) {
                Attack();
            }    
            // move 
            while(selectedUnitAi.UnitData.CurrentAp > 0 && IsTargetInRange() == false && selectedUnitAiValidPath == true) {   
                Move();
            }         
            // try to attack if hasn't already and player unit is in attack range        
            if(IsTargetInRange() && selectedUnitAttacked == false) {
                Attack();
            }                      
        }
        private bool IsTargetInRange() {
            return selectedUnitAi.EnemiesInRange.Contains(unitManager.PlayerUnits[0]);
        }
        private void Move() {        
            if(selectedUnitAi.HasMovedThisTurn || selectedUnitAi.UnitData.CurrentAp <= 0) { return; } // if has already moved (this turn), return

            // display path towards player
            AstarPathFinding selectedUnitPF = selectedUnitAi.GetComponent<AstarPathFinding>();

            // import for detecting "dynamic" obstacles properly (e.g. other units)
            grid.Generate();

            selectedUnitAiValidPath = selectedUnitPF.FindPath(selectedUnitAi.transform.position, unitManager.PlayerUnits[0].transform.position, selectedUnitAi.IsFlying);
            if(selectedUnitAiValidPath == false) {
                //nopath!
                return;
            }
            Vector2 targetNodePos = selectedUnitPF.FinalPath[0].position;
            // get the tile
            TileController tileToMove;
            Collider2D tileHit = Physics2D.OverlapCircle(targetNodePos, 0.2f, tileMask);
            if(tileHit) {
                tileToMove = tileHit.GetComponent<TileController>();
            } else {
                tileToMove = null;
            }
            if(tileToMove != null) {
                if(tileToMove.HasObstacle() == false) {
                    selectedUnitAi.Move(tileToMove.transform.position, tileToMove.gameObject, false);
                } else {
                    print($"an obstacle is on the way of {selectedUnitAi.UnitData.name}");
                }
                // subtract 1 AP because it has moved 1 tile
                selectedUnitAi.UnitData.CurrentAp -= 1;                
            } else {
                Debug.LogError("Something is wrong with AI movement behaviour's LOGIC. You probably forgot to set tileLayer's mask");                
            }
        }
        private void Attack() {
            // select spell to use            
            selectedSpell = selectedUnitAi.UnitData.Spells[0];
            selectedEffect = new EffectData(selectedUnitAi.UnitData, selectedSpell.EffectType, selectedSpell.DamageType, selectedSpell.AmountInstant, selectedSpell.AmountOverTurns, selectedSpell.Range, selectedSpell.IgnoreObstacles, selectedSpell.Duration, selectedSpell.SFX, selectedSpell.LastsTheEntireBattle); 
            // check the spell's target type
            switch(selectedSpell.TargetType) {
                case SpellTargetType.NoTarget: break;
                //case SpellTargetType.TargetSelf: TargetSelf(); break;
                case SpellTargetType.TargetUnit: HandleTargetUnit(); break;
                case SpellTargetType.TargetTile: break;
                default: break;
            }
            selectedUnitAttacked = true;       
        }
        // private void TargetSelf() {
        //     selectedUnitAi.AddEffect(selectedEffect);
        //     selectedUnitAi.SetSelectedEffect(selectedEffect);            
        //     selectedUnitAi.SpellSelfTarget();
        // }
        private void HandleTargetUnit() {
            // highlight player units in range
            foreach(GameObject playerUnit in selectedUnitAi.EnemiesInRange) {
                UnitController playerUnitController = playerUnit.GetComponent<UnitController>();
                playerUnitController.SetSpellPreviewDamage(selectedEffect.InstantDamageData);
                playerUnitController.DisplayDamagePreview(true);
                playerUnitController.SetIsTarget(true);
                playerUnitController.Highlight(true);
            }
            // decide which one to cast the spell on
            int randomIndex = Random.Range(0, selectedUnitAi.EnemiesInRange.Count);
            UnitController pUC = selectedUnitAi.EnemiesInRange[randomIndex].GetComponent<UnitController>();
            SpellCastOn(pUC);
        }
        private void SpellCastOn(UnitController targetUnitController) {
            // apply effect to target unit
            targetUnitController.AddEffect(selectedEffect);
            
            // enemy targetting was disabled here
            foreach(GameObject e in unitManager.EnemyUnits) {
                UnitController eUC = e.GetComponent<UnitController>();
                eUC.DisplayDamagePreview(false);
                eUC.SetIsTarget(false);
                eUC.Highlight(false);
            }
            
            // if spell has an instant damage or heal, apply it now
            switch(selectedEffect.DamageType) {
                case DamageType.Physical: selectedUnitAi.DealAttack(targetUnitController, selectedEffect.AmountInstant, targetUnitController.transform.position); break;
                case DamageType.Magical: targetUnitController.ReceiveDamage(selectedEffect.InstantDamageData); break;
                case DamageType.Piercing: targetUnitController.ReceiveDamage(selectedEffect.InstantDamageData); break;
                default: break;
            }
            // if is spell is only once per combat, consume it
            // if(selectedSpell.OncePerCombat) {
            //     spellCast.SetIsConsumed(true);
            // }        
            // deselect spell and effect
            selectedSpell = null;
            selectedEffect = null;
        }        
    }
}
