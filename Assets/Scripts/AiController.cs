using UnityEngine;
using System.Collections.Generic;
using Zenject;
using System.Collections;

namespace LUX.LightOfHeaven {
    public class AiController : MonoBehaviour {
        [SerializeField] private UnitController selectedUnitAi;
        [SerializeField] private Spell selectedSpell;
        [SerializeField] private UnitController selectedSpellTargetUnit;
        private EffectData selectedEffect;
        [SerializeField] private LayerMask tileMask;

        private bool selectedUnitAttacked;
        private HashSet<TileController> tilesChecked = new HashSet<TileController>();

        private const float unitTurnTime = 1f;
        private const float attackDelayTime = 0f;

        [Inject] private GameEventSystem gameEventSystem;
        [Inject] private UnitManager unitManager;
        [Inject] private TurnManager turnManager;
        [Inject] private MapManager mapManager;

        private void OnEnable() {
            gameEventSystem.onTurnEnded += Reset;
        }
        private void OnDisable() {
            gameEventSystem.onTurnEnded -= Reset;
        }
        IEnumerator WaitUnitTurn(float seconds) {
            int i = 0;
            while (i < unitManager.EnemyUnits.Count) {
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
        IEnumerator WaitThenAct(float seconds) {
            yield return new WaitForSeconds(seconds);

            SelectSpell(selectedUnitAi.CurrentTile, selectedUnitAi.CurrentSp);

            if (selectedSpell != null) {
                Attack();
            } else if (selectedUnitAi.DestructiblesInRange.Count > 0 && selectedUnitAttacked == false) {
                AttackObstacle();
            }
            List<TileController> path = selectedUnitAi.PathFinder.GetPathToTargetOnTile(selectedUnitAi.CurrentTile, unitManager.Player.CurrentTile);
            int movedCount = 0;
            // move 
            while (selectedUnitAi.CurrentSp > 0 &&
            selectedUnitAttacked == false &&
            SelectSpell(selectedUnitAi.CurrentTile, selectedUnitAi.CurrentSp) == false &&
            path.Count > 0
            ) {
                if(path[movedCount].HasObstacle())
                    break;
                Move(path[movedCount]);
                movedCount++;
            }
            // try to attack if hasn't already and player unit is in attack range        
            if (selectedSpell != null && selectedUnitAttacked == false) {
                Attack();
            } else if (selectedUnitAi.DestructiblesInRange.Count > 0 && selectedUnitAttacked == false && selectedUnitAi.HasMovedThisTurn == false) {
                AttackObstacle();
            }
        }
        IEnumerator KillWait(float delay, Task t) {
            yield return new WaitForSeconds(delay);
            t.Stop();
        }

        private void Awake() {
            tilesChecked = new HashSet<TileController>();
        }
        private void OnUnitEndTurn() {
            mapManager.ResetTiles();
            PlanUnitNextActionPhase();
            selectedUnitAi.SetSelection(false);
        }
        public void Reset() {
            selectedUnitAi = null;
        }
        public void StartTurn() {
            // if there are no enemy units, actually end the battle
            if (unitManager.EnemyUnits.Count <= 0) {
                turnManager.EndTurn();
                return;
            }
            Task unitTurnDelayTask = new Task(WaitUnitTurn(unitTurnTime));
            new Task(KillWait((unitTurnTime * unitManager.EnemyUnits.Count) + unitTurnTime, unitTurnDelayTask));
        }
        private void StartUnitActionPhase() {
            selectedUnitAi.SetSelection(true);
            selectedUnitAi.DisplayOutgoingDamagePreview(false);

            // try to cast chosen spell if player unit is in cast range
            selectedUnitAttacked = false;

            // if there are no spells left to cast, reset the spells pool
            if (selectedUnitAi.SpellPool.Count <= 0) {
                selectedUnitAi.ResetSpellsPool();
            }

            // use buff on self/other enemy?
            // apply debuff to player?            
            // decide spell to cast on player (longest range first)            

            Task atkDelayTask = new Task(WaitThenAct(attackDelayTime));
            new Task(KillWait(attackDelayTime, atkDelayTask));
        }
        private void PlanUnitNextActionPhase() {
            //selectedUnitAi.SetSelection(true);

            // if there are no spells left to cast, reset the spells pool
            if (selectedUnitAi.SpellPool.Count <= 0) {
                selectedUnitAi.ResetSpellsPool();
            }

            // use buff on self/other enemy?
            // apply debuff to player?            
            // decide spell to cast on player (longest range first)            

            PlanUnitAction();
        }
        private void PlanUnitAction() {
            SelectSpell(selectedUnitAi.CurrentTile, selectedUnitAi.UnitStats.MaxSp);

            if (selectedSpell != null) {
                selectedEffect = new EffectData(selectedUnitAi.UnitStats, selectedSpell.EffectType, selectedSpell.DamageType, selectedSpell.AmountInstant, selectedSpell.AmountOverTurns, selectedSpell.Range, selectedSpell.IgnoreObstacles, selectedSpell.Duration, selectedSpell.SFX, selectedSpell.LastsTheEntireBattle);
                selectedUnitAi.SetDamagePreview(selectedEffect.InstantDamageData, false, true);
                //print($"{selectedUnitAi.UnitData.name} intends to ATTACK this turn");
                selectedSpell = null;
                return;
            }

            List<TileController> path = selectedUnitAi.PathFinder.GetPathToTargetOnTile(selectedUnitAi.CurrentTile, unitManager.Player.CurrentTile);
            int movedCount = 0;
            int remainingSp = selectedUnitAi.UnitStats.MaxSp;
            TileController plannedCurrentTile = selectedUnitAi.CurrentTile;
            // move 
            while (remainingSp > 0 &&
            SelectSpell(plannedCurrentTile, remainingSp) == false &&
            path.Count > 0
            ) {
                if(path[movedCount].HasObstacle())
                    break;
                plannedCurrentTile = path[movedCount];
                remainingSp--;
                movedCount++;
            }
            // try to attack if hasn't already and player unit is in attack range        
            if (selectedSpell != null) {
                selectedEffect = new EffectData(selectedUnitAi.UnitStats, selectedSpell.EffectType, selectedSpell.DamageType, selectedSpell.AmountInstant, selectedSpell.AmountOverTurns, selectedSpell.Range, selectedSpell.IgnoreObstacles, selectedSpell.Duration, selectedSpell.SFX, selectedSpell.LastsTheEntireBattle);
                selectedUnitAi.SetDamagePreview(selectedEffect.InstantDamageData, false, true);
                //print($"{selectedUnitAi.UnitData.name} intends to ATTACK this turn");
                selectedSpell = null;
            }
            else {
                selectedUnitAi.SetIntent(IntentType.Move);
                //print($"{selectedUnitAi.UnitData.name} intends to MOVE this turn");   
            }        
        }
        private bool IsTargetInRange() {
            return selectedUnitAi.EnemiesInRange.Contains(unitManager.PlayerUnits[0]);
        }
        private bool SelectSpell(TileController currentTile, int spLeft) {
            int higherDamage = -100;
            bool hasSpellToCast = false;
            List<Spell> unusableSpells = new List<Spell>();

            foreach (Spell spell in selectedUnitAi.SpellPool) {
                // reset tiles spell in range props            
                mapManager.ResetTiles();
                List<GameObject> spellTargetsInRange = selectedUnitAi.GetEnemiesInRangeOf(currentTile, spell.Range, true, spell.IgnoreObstacles);
                if (spellTargetsInRange.Count <= 0) { continue; } // continue if no targets in range

                GameObject selectedSpellTarget = spellTargetsInRange[0];
                if (selectedSpellTarget != null) {
                    if (spLeft >= spell.Cost) {
                        if (spell.AmountInstant > higherDamage) {
                            higherDamage = spell.AmountInstant;
                            selectedSpellTargetUnit = selectedSpellTarget.GetComponent<UnitController>();
                            selectedSpell = spell;
                            selectedEffect = new EffectData(selectedUnitAi.UnitStats, selectedSpell.EffectType, selectedSpell.DamageType, selectedSpell.AmountInstant, selectedSpell.AmountOverTurns, selectedSpell.Range, selectedSpell.IgnoreObstacles, selectedSpell.Duration, selectedSpell.SFX, selectedSpell.LastsTheEntireBattle);
                            hasSpellToCast = true;
                        }
                    } else if(spell.Cost > selectedUnitAi.UnitStats.MaxSp) {
                        unusableSpells.Add(spell);                        
                    }
                }
            }
            foreach(Spell spell in unusableSpells)
                selectedUnitAi.RemoveSpellFromPool(spell);
            return hasSpellToCast;
        }
        
        private void Move(TileController tile) {  
            selectedUnitAi.Move(tile.transform.position, tile.gameObject, false);
        }

        private void Attack() {
            // check the spell's target type
            switch (selectedSpell.TargetType) {
                case SpellTargetType.NoTarget: break;
                //case SpellTargetType.TargetSelf: TargetSelf(); break;
                case SpellTargetType.TargetUnit: HandleUnitTarget(); break;
                case SpellTargetType.TargetTile: break;
                default: break;
            }

            UnitConsumeSelectedSpell();

            // deselect spell and effect
            selectedSpell = null;
            selectedEffect = null;
            // set as attacked
            selectedUnitAttacked = true;
        }
        private void AttackObstacle() {
            selectedUnitAi.DestructiblesInRange[0].GetComponent<IDestructible>().Damage(selectedUnitAi.UnitStats.PhyDamage.Value);
            selectedUnitAttacked = true;
            //print($"{selectedUnitAi.UnitData.name} attacked an obstacle for {selectedUnitAi.UnitData.AtkDamage}");
        }
        private void UnitConsumeSelectedSpell() {
            selectedUnitAi.RemoveSpellFromPool(selectedSpell);
        }
        private void TargetSelf() {
            selectedUnitAi.AddEffect(selectedEffect);
            SpellCastOn(selectedUnitAi);
        }
        private void HandleUnitTarget() {
            SpellCastOn(selectedSpellTargetUnit);
        }
        private void SpellCastOn(UnitController targetUnitController) {
            // apply effect to target unit
            targetUnitController.AddEffect(selectedEffect);

            // enemy targetting was disabled here
            foreach (GameObject p in unitManager.PlayerUnits) {
                UnitController pUC = p.GetComponent<UnitController>();
                pUC.DisplayIncomingDamagePreview(false);
                pUC.SetIsTarget(false);
                pUC.Highlight(false);
            }

            // if spell has an instant damage or heal, apply it now
            switch (selectedEffect.DamageType) {
                case DamageType.Physical: selectedUnitAi.DealAttack(targetUnitController, selectedEffect.AmountInstant, targetUnitController.transform.position); break;
                case DamageType.Magical: targetUnitController.Damage(selectedEffect.InstantDamageData); break;
                case DamageType.Piercing: targetUnitController.Damage(selectedEffect.InstantDamageData); break;
                default: break;
            }
            // play spell sfx
            AudioManager.PlaySFX(selectedSpell.SFX);
            // if is spell is only once per combat, consume it
            // if(selectedSpell.OncePerCombat) {
            //     spellCast.SetIsConsumed(true);
            // }
        }
    }
}
