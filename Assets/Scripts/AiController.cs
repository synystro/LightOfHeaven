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
        [SerializeField] private bool selectedUnitAiValidPath;
        [SerializeField] private LayerMask tileMask;

        private bool selectedUnitAttacked;
        private HashSet<TileController> tilesChecked = new HashSet<TileController>();

        private const float unitTurnTime = .1f;
        private const float attackDelayTime = .1f;

        [Inject] private GameEventSystem gameEventSystem;
        [Inject] private UnitManager unitManager;
        [Inject] private TurnManager turnManager;
        [Inject] private MapManager mapManager;

        [Inject] PathFindingGrid grid;

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

            if (selectedSpell != null) {
                Attack();
            } else if (selectedUnitAi.DestructiblesInRange.Count > 0 && selectedUnitAttacked == false) {
                AttackObstacle();
            }

            // move 
            while (selectedUnitAi.UnitData.CurrentAp > 0 &&
            selectedUnitAttacked == false &&
            IsBlockedByObstacle() == false &&
            SelectSpell() == false &&
            selectedUnitAiValidPath == true) {
                Move();
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
            selectedUnitAiValidPath = true;

            // try to cast chosen spell if player unit is in cast range
            selectedUnitAttacked = false;
            //selectedUnitAi.GetEnemiesInRangeOf(selectedUnitAi.UnitData.AtkRange, false, selectedUnitAi.UnitData.Flight);
            IsBlockedByObstacle();

            // if there are no spells left to cast, reset the spells pool
            if (selectedUnitAi.SpellPool.Count <= 0) {
                selectedUnitAi.ResetSpellsPool();
            }

            // use buff on self/other enemy?
            // apply debuff to player?            
            // decide spell to cast on player (longest range first)
            SelectSpell();

            Task atkDelayTask = new Task(WaitThenAct(attackDelayTime));
            new Task(KillWait(attackDelayTime, atkDelayTask));
        }
        private bool IsTargetInRange() {
            return selectedUnitAi.EnemiesInRange.Contains(unitManager.PlayerUnits[0]);
        }
        private bool IsBlockedByObstacle() {
            return selectedUnitAi.GetDestructiblesInRangeOf(selectedUnitAi.UnitData.AtkRange, false, selectedUnitAi.UnitData.Flight).Count > 0;
        }
        private bool SelectSpell() {
            int higherDamage = -100;
            bool hasSpellToCast = false;

            foreach (Spell spell in selectedUnitAi.SpellPool) {
                // reset tiles spell in range props            
                mapManager.ResetTiles();
                List<GameObject> spellTargetsInRange = selectedUnitAi.GetEnemiesInRangeOf(spell.Range, true, spell.IgnoreObstacles);
                if (spellTargetsInRange.Count <= 0) { continue; } // continue if no targets in range

                GameObject selectedSpellTarget = spellTargetsInRange[0];
                if (selectedSpellTarget != null) {
                    if (selectedUnitAi.UnitData.CurrentAp >= spell.Cost) {
                        if (spell.AmountInstant > higherDamage) {
                            higherDamage = spell.AmountInstant;
                            selectedSpellTargetUnit = selectedSpellTarget.GetComponent<UnitController>();
                            selectedSpell = spell;
                            selectedEffect = new EffectData(selectedUnitAi.UnitData, selectedSpell.EffectType, selectedSpell.DamageType, selectedSpell.AmountInstant, selectedSpell.AmountOverTurns, selectedSpell.Range, selectedSpell.IgnoreObstacles, selectedSpell.Duration, selectedSpell.SFX, selectedSpell.LastsTheEntireBattle);
                            hasSpellToCast = true;
                        }
                    }
                }
            }
            return hasSpellToCast;
        }
        private void Move() {
            if (selectedUnitAi.UnitData.CurrentAp <= 0) { return; } // if has already moved (this turn), return

            // display path towards player
            AstarPathFinding selectedUnitPF = selectedUnitAi.GetComponent<AstarPathFinding>();

            // for detecting "dynamic" obstacles properly (e.g. other units)
            grid.Generate();

            Vector2 targetPos = unitManager.PlayerUnits[0].transform.position;

            selectedUnitAiValidPath = selectedUnitPF.FindPath(selectedUnitAi.transform.position, targetPos, selectedUnitAi.IsFlying);
            //selectedUnitPF.FindPath(selectedUnitAi.transform.position, targetPos, true);
            if (selectedUnitAiValidPath == false || selectedUnitPF.FinalPath.Count <= 0) {
                selectedUnitPF.FindPath(selectedUnitAi.transform.position, targetPos, true);
            }

            Vector2 targetNodePos = selectedUnitPF.FinalPath[0].position;

            // get the tile
            TileController tileToMove;
            Collider2D tileHit = Physics2D.OverlapCircle(targetNodePos, 0.2f, tileMask);
            if (tileHit) {
                tileToMove = tileHit.GetComponent<TileController>();
            } else {
                tileToMove = null;
            }
            if (tileToMove != null) {
                if (tileToMove.HasObstacle() == false) {
                    selectedUnitAi.Move(tileToMove.transform.position, tileToMove.gameObject, false);
                } else {
                    //Debug.LogWarning($"an obstacle is on the way of {selectedUnitAi.UnitData.name}");
                }
                // subtract 1 AP because it has moved 1 tile
                selectedUnitAi.UnitData.CurrentAp -= 1;
            } else {
                Debug.LogError("Something is wrong with AI movement behaviour's LOGIC. You probably forgot to set tileLayer's mask");
            }
        }
        private void Attack() {
            // select spell to use            
            //selectedSpell = selectedUnitAi.UnitData.Spells[0];
            //selectedEffect = new EffectData(selectedUnitAi.UnitData, selectedSpell.EffectType, selectedSpell.DamageType, selectedSpell.AmountInstant, selectedSpell.AmountOverTurns, selectedSpell.Range, selectedSpell.IgnoreObstacles, selectedSpell.Duration, selectedSpell.SFX, selectedSpell.LastsTheEntireBattle); 
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
            selectedUnitAi.DestructiblesInRange[0].GetComponent<IDestructible>().Damage(selectedUnitAi.UnitData.AtkDamage);
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
                pUC.DisplayDamagePreview(false);
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
