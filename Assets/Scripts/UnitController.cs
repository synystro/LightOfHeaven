﻿using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using Zenject;

namespace LUX {
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(PathFinder))]
    [RequireComponent(typeof(UnitDetails))]
    public class UnitController : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler {
        [SerializeField] private GameObject selectionHighlightGO;
        [SerializeField] private GameObject attackHighlightGO;
        [SerializeField] private Unit unit;
        [SerializeField] private bool isEnemy;
        [SerializeField] private bool isFlying;  
        [SerializeField] private bool hasTargetInRange;  
        [SerializeField] private LayerMask groundLayer;  
        [SerializeField] private TileController currentTile;
        [SerializeField] private bool isFacingRight;
        [SerializeField] private bool isSelected;
        [SerializeField] private bool hasMovedThisTurn;
        [SerializeField] private bool hasAttackedThisTurn;
        [SerializeField] private List<GameObject> enemiesInRange;
        //private List<GameObject> attackHighlights;
        public Unit UnitData => unit;
        public TileController CurrentTile => currentTile;
        public bool HasMovedThisTurn => hasMovedThisTurn;
        public bool HasAttackedThisTurn => hasAttackedThisTurn;
        public bool HasTargetInRange => hasTargetInRange;
        public bool IsEnemy => isEnemy;
        public bool IsFlying => isFlying;
        public List<GameObject> EnemiesInRange => enemiesInRange;
        public void SetHasMoved(bool state) { hasMovedThisTurn = state; }
        public void SetHasAttacked(bool state) { hasAttackedThisTurn = state; }
        public void SetHasTargetInRange(bool state) { hasTargetInRange = state; }

        [Inject] private PlayerController playerController;
        [Inject] private GameEventSystem gameEventSystem;
        [Inject] private UnitManager unitManager;
        [Inject] private MapManager mapManager;        

        private GameObject facingLeftModelGO;
        private GameObject facingRightModelGO; 

        private SpriteRenderer selectionSR;   
        private PathFinder pathFinder;
        private UnitDetails unitDetails;    

        private void Awake() {            
            enemiesInRange = new List<GameObject>();
            selectionSR = selectionHighlightGO.GetComponent<SpriteRenderer>();
            pathFinder = this.GetComponent<PathFinder>();
            unitDetails = this.GetComponent<UnitDetails>();
        }
        private void Start() {
            gameEventSystem.onTurnEnd += TurnEndReset;
            gameEventSystem.onUnitAttack += OnUnitAttacked;
        }
        private void OnDestroy() {
            gameEventSystem.onTurnEnd -= TurnEndReset;
            gameEventSystem.onUnitAttack -= OnUnitAttacked;
        }
        public void Setup(Unit unit, GameObject tileToSpawnGO, bool isEnemy) {
            // set facing direction
            isFacingRight = true;
            if(unit.charPrefabLeft != null) {
                facingLeftModelGO = Instantiate(unit.charPrefabLeft, this.transform.position, Quaternion.identity, this.transform);
                if(isFacingRight == true) { facingLeftModelGO.SetActive(false); }
            } else {
                Debug.LogError("Forgot to set char prefab on Unit SO.");
            }
            if(unit.charPrefabRight != null) {
                facingRightModelGO = Instantiate(unit.charPrefabRight, this.transform.position, Quaternion.identity, this.transform);
                if(isFacingRight == false) { facingRightModelGO.SetActive(false); }
            } else {
                Debug.LogError("Forgot to set char prefab on Unit SO.");
            } 
            this.unit = unit;
            this.currentTile = tileToSpawnGO.transform.GetComponent<TileController>();
            this.currentTile.SetCurrentUnit(this);
            this.currentTile.SetHasObstacle(true);
            unit.Reset();
            this.gameObject.name = unit.name;

            // can this unit fly? 
            this.isFlying = unit.Flight;
            // enemy stuff
            this.isEnemy = isEnemy;           

            // setup unit info display
            RefreshDetails();            
            }
        public void TurnEndReset() {
            SetSelection(false);
            hasMovedThisTurn = false;
            hasAttackedThisTurn = false;
            hasTargetInRange = false;
            enemiesInRange.Clear();
            DisplayAttackHighlight(false);
            // RESET UNIT ! put somewhere else ?
            ResetUnitStats();
        }
        public void OnUnitAttacked(bool isEnemyAttack) {
            //RefreshDetails(); // already running inside receive damage function
        }
        private void ResetUnitStats() {
            this.UnitData.CurrentAp = this.UnitData.MaxAp;
        }        
        private void MouseClick() {
            // if clicked on an enemy unit
            if (isEnemy) {
                UnitController selectedUnit = unitManager.GetSelectedUnit();
                if(selectedUnit != null) {
                    if(selectedUnit.EnemiesInRange.Contains(this.gameObject)
                    && selectedUnit.HasAttackedThisTurn == false
                    && playerController.HasAttackedThisTurn == false) {
                        selectedUnit.DealAttack(this, this.transform.position);
                        this.DisplayAttackHighlight(false);
                        // display damage done
                    }
                }
            }
            // if clicked on a player unit
            else {
                // if has moved, return
                if (hasMovedThisTurn || playerController.HasMovedThisTurn) { return; }

                if (isSelected) {
                    unitManager.DeselectUnit();
                    mapManager.ResetTiles();
                } else {
                    if (unitManager.GetSelectedUnit() != null) {
                        unitManager.DeselectUnit();
                    }
                    unitManager.SetSelectedUnit(this);
                    mapManager.ResetTiles();
                    GetReachableTiles();
                    GetEnemiesInAtkRange();
                }
            }
        }        
        private void RefreshDetails() {
            unitDetails.Refresh(unit);
        }  
        private void SetFacingDirectionTowardsCoordX(int targetPositionX) {
            if (targetPositionX - this.transform.position.x > 0) {
                if (isFacingRight == false) {
                    isFacingRight = true;
                    facingRightModelGO.SetActive(true);
                    facingLeftModelGO.SetActive(false);
                }
            } else {
                if (isFacingRight == true) {
                    isFacingRight = false;
                    facingRightModelGO.SetActive(false);
                    facingLeftModelGO.SetActive(true);
                }
            }
        }
        private void LockSelection() {
            Color red = new Color(.5f,0f,0f,1f);
            selectionSR.color = red;
        }
        public List<GameObject> GetReachableTiles() {
            if (hasMovedThisTurn) { return null; }           
            return pathFinder.GetReachableTiles();
        }
        public List<GameObject> GetEnemiesInAtkRange() {
            if(hasAttackedThisTurn) { return null; }
            return enemiesInRange = pathFinder.GetReachableEnemies();
        }
        public void SetSelection(bool state) {
            isSelected = state;
            selectionSR.color = Color.white;
            selectionHighlightGO.SetActive(state);
        }  
        public void DisplayAttackHighlight(bool state) {
            attackHighlightGO.SetActive(state);
        }
        public void Move(Vector2 clickPoint, GameObject targetTileGO) {
            // return if there are no action points left
            if(this.UnitData.CurrentAp <= 0) { return; }

            TileController targetTile = targetTileGO.GetComponent<TileController>();
            // return if the player has already moved an unit this turn
            if(playerController.HasMovedThisTurn) { return; }
            // reset all tiles visual changes
            mapManager.ResetTiles();
            // free current tile
            currentTile.SetCurrentUnit(null);
            currentTile.SetHasObstacle(false);
            // change unit facing direction towards target tile
            SetFacingDirectionTowardsCoordX(Mathf.RoundToInt(clickPoint.x));
            // move to target tile
            this.transform.position = clickPoint;
            // set target tile to be the current tile
            currentTile = targetTile;
            // occupy it
            currentTile.SetCurrentUnit(this);
            targetTile.SetHasObstacle(true);
            // trigget on unit move event
            gameEventSystem.OnUnitMove(isEnemy);
            if(isEnemy == false) {
                playerController.SetHasMovedThisTurn(true);
            }            
            hasMovedThisTurn = true;
            // if has already attacked, no reason for the unit to be selected
            if(hasAttackedThisTurn) {
                SetSelection(false);
                return;
            }
            // reset all tiles
            mapManager.ResetTiles();
            // after unit has moved, scan for enemies in range           
            GetEnemiesInAtkRange();
            // display locked selection
            LockSelection();
        }
        public void DealAttack(UnitController attackedUnitController, Vector3 attackedUnitPosition) { 
            // return if the player has already attacked an unit this turn
            if(playerController.HasAttackedThisTurn) { return; } 

            Unit attackedUnit = attackedUnitController.UnitData;

            // change unit facing direction towards enemy
            SetFacingDirectionTowardsCoordX(Mathf.RoundToInt(attackedUnitPosition.x));

            DamageData attackDamageData = new DamageData(this.unit, this.unit.AtkDamage, DamageType.Physical, this.unit.CritChance, this.unit.StunChance, this.unit.LethalChance);

            // attack!
            attackedUnitController.ReceiveDamage(attackDamageData);

            // call attacked unit's onAttacked function
            gameEventSystem.OnUnitAttack(isEnemy);

            // if this is a player unit
            if(isEnemy == false) {
                playerController.SetHasAttackedThisTurn(true);
            } 

            hasAttackedThisTurn = true;            
            
            // if has already moved, no reason for the unit to be selected
            if(hasMovedThisTurn) {
                SetSelection(false);
                return;
            }
            // display locked selection
            LockSelection();
        }
        public void ReceiveDamage(DamageData damageData) {
            int lethalRandom = Random.Range(0,100);
            if(lethalRandom < damageData.LethalChance) {
                this.unit.CurrentHp = 0;
                print($"{damageData.Source.name} just dealt a LETHAL attack to {this.unit.name}! Instantly killing them!");
                Die();                
                return;
            }
            switch(damageData.Type) {
                case DamageType.Physical:
                    int physicalDamage = damageData.Amount;
                    // critical
                    int critRandom = Random.Range(0,100);
                    if(critRandom < damageData.CritChance) {
                        physicalDamage = physicalDamage * 2;                                                
                    }
                    // final atk damage
                    int finalPhysicalDamage = physicalDamage - this.unit.Armor > 0 ? physicalDamage : 0;
                    this.unit.CurrentHp -= finalPhysicalDamage;
                    print($"{damageData.Source.name} just attacked {this.unit.name} for {finalPhysicalDamage} physical damage! Leaving them at {this.unit.CurrentHp}");                    
                    break;
                case DamageType.Magical:
                    int magicalDamage = damageData.Amount;
                    int finalMagicalDamage = magicalDamage - this.unit.MagicResistance > 0 ? damageData.Amount : 0;
                    this.unit.CurrentHp -= finalMagicalDamage;
                    break;
                default:
                    break;
            }   
            if(this.unit.CurrentHp <= 0) {
                this.unit.CurrentHp = 0;
                Die();
            }         
            RefreshDetails();                        
        }
        private void Die() {            
            gameEventSystem.OnUnitDie(this.gameObject);            
        }
        public void OnPointerClick(PointerEventData eventData) {
            MouseClick();
        }
        public void OnPointerEnter(PointerEventData eventData) {
            unitDetails.SetDetailsCanvasState(true);
        }
        public void OnPointerExit(PointerEventData eventData) {
            unitDetails.SetDetailsCanvasState(false);
        }
    }
}