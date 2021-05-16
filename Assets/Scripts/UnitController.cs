using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using Zenject;
using System.Linq;

namespace LUX {
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(PathFinder))]
    [RequireComponent(typeof(UnitDetailsUi))]
    public class UnitController : TacticalMovement, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler {
        [SerializeField] private List<EffectData> activeEffects;
        [SerializeField] private GameObject selectionHighlightGO;
        [SerializeField] private GameObject attackHighlightGO;
        [SerializeField] private GameObject damagePopupPrefab;
        [SerializeField] private Unit unit;
        [SerializeField] private bool isEnemy;             
        [SerializeField] private bool hasTargetInRange;                  
        [SerializeField] private bool isSelected;
        [SerializeField] private bool isTarget;     
        [SerializeField] private bool hasAttackedThisTurn;
        [SerializeField] private bool isStunned;
        [SerializeField] private List<GameObject> enemiesInRange; 

        public Unit UnitData => unit;
        public TileController CurrentTile => currentTile;
        public bool HasMovedThisTurn => hasMovedThisTurn;
        public bool HasAttackedThisTurn => hasAttackedThisTurn;
        public bool IsStunned => isStunned;
        public bool HasTargetInRange => hasTargetInRange;
        public bool IsEnemy => isEnemy;
        public bool IsFlying => isFlying;
        public bool IsTarget => isTarget;
        public List<GameObject> EnemiesInRange => enemiesInRange;
        public void SetHasMoved(bool state) { hasMovedThisTurn = state; }
        public void SetHasAttacked(bool state) { hasAttackedThisTurn = state; }
        public void SetHasTargetInRange(bool state) { hasTargetInRange = state; }

        public GameEventSystem GameEventSystem => gameEventSystem;
        public MapManager MapManager => mapManager;
        public PlayerController PlayerController => playerController;        

        [Inject] private PlayerController playerController;
        [Inject] private GameEventSystem gameEventSystem;
        [Inject] private UnitManager unitManager;
        [Inject] private MapManager mapManager;
        
        private SpriteRenderer selectionSR;   
        
        private UnitDetailsUi unitDetailsUi;    

        private void Awake() {            
            activeEffects = new List<EffectData>();
            enemiesInRange = new List<GameObject>();
            selectionSR = selectionHighlightGO.GetComponent<SpriteRenderer>(); 
            pathFinder = this.GetComponent<PathFinder>();           
            unitDetailsUi = this.GetComponent<UnitDetailsUi>();
        }
        private void Start() {
            gameEventSystem.onTurnStart += OnTurnStart;
            gameEventSystem.onTurnEnd += OnTurnEnd;
            gameEventSystem.onUnitAttack += OnUnitAttacked;
        }
        private void OnDestroy() {
            gameEventSystem.onTurnStart -= OnTurnStart;
            gameEventSystem.onTurnEnd -= OnTurnEnd;
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
            unit.ResetBonuses();
            unit.Setup();
            unit.RestoreStats();
            this.gameObject.name = unit.name;

            // can this unit fly? 
            this.isFlying = unit.Flight;
            // enemy stuff
            this.isEnemy = isEnemy;                   

            // setup unit info display
            RefreshDetailsUi();            
        }
        public void OnTurnStart() {
            isStunned = false;
            ResetUnitStats();                        
        }
        public void OnTurnEnd() {
            SetSelection(false);
            isTarget = false;
            hasMovedThisTurn = false;
            hasAttackedThisTurn = false;
            hasTargetInRange = false;
            enemiesInRange.Clear();
            DisplayAttackHighlight(false);            
        }        
        public void OnUnitAttacked(bool isEnemyAttack) {
            //RefreshDetails(); // already running inside receive damage function
        }
        public void AddEffect(EffectData effect) {
            if(activeEffects.Contains(effect) == false) {
                activeEffects.Add(effect);
            }
        }
        public void RemoveEffect(EffectData effect) {
            if(activeEffects.Contains(effect)) {
                activeEffects.Remove(effect);
            }
        }
        public void ApplyStatsModifier(EffectData e) {
            bool tookDamage = false;
            switch(e.EffectType) {                
                case EffectType.Hp: unit.BonusHp += e.Amount; unit.CurrentHp += e.Amount; break;
                case EffectType.Damage: unit.CurrentHp -= e.Amount; tookDamage = true; break;
                case EffectType.Stun: isStunned = true; break;
                default: break;
            }
            if(tookDamage) {
                AudioManager.PlaySFX(e.TickSFX);
                DisplayDamagePopup(e.Amount, this.transform.position);
            }
            if(this.unit.CurrentHp <= 0) {
                Die();                
            }
        }
        private void ResetUnitStats() {
            // reset modifiers
            this.unit.ResetBonuses();
            // apply modifiers
            foreach(EffectData e in activeEffects.ToList()) {
                ApplyStatsModifier(e);
                if(e.LastsTheEntireBattle) { continue; } // return if the effect isn't be removed until the end of battle
                // after each turn, reduce its duration value
                e.Duration -= 1;
                // remove the effect if it has reached the end of its duration
                if(e.Duration <= 0) {
                    activeEffects.Remove(e);
                }
            }
            // set everything up after resetting modifiers 
            this.unit.Setup();
            // restore things after the unit's turn has begun
            this.unit.RestoreAfterTurn();
            // refresh unit details display
            RefreshDetailsUi();
            if(isStunned) {
                print($"{this.name} is stunned and lost their turn!");
                playerController.EndTurn();
            }
        }   
        private void RefreshDetailsUi() {
            unitDetailsUi.Refresh(unit);
        }        
        private void OnMouseClick() {
            // if clicked on an enemy unit
            if (isEnemy) {
                if(isTarget) {
                    // apply targetted spell to this enemy
                    if(playerController.SelectedEffect != null) {
                        activeEffects.Add(playerController.SelectedEffect);
                        playerController.SpellWasCast();                     
                    }
                }
                UnitController selectedUnit = unitManager.GetSelectedUnit();
                if(selectedUnit != null) {
                    if(selectedUnit.EnemiesInRange.Contains(this.gameObject)
                    && selectedUnit.HasAttackedThisTurn == false
                    && playerController.HasAttackedThisTurn == false) {
                        selectedUnit.DealAttack(this, this.transform.position);
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
        private void LockSelection() {
            Color red = new Color(.5f,0f,0f,1f);
            selectionSR.color = red;
        }        
        public List<GameObject> GetEnemiesInAtkRange() {
            if(hasAttackedThisTurn) { return null; }
            return enemiesInRange = pathFinder.GetReachableEnemies();
        }
        public void SetSelection(bool state) {
            isSelected = state;  
            Highlight(state);          
        }  
        public void HighlightToBeTargetted(bool state) {
            isTarget = state;
            selectionSR.color = Color.white;
            selectionHighlightGO.SetActive(state);
        }
        public void Highlight(bool state) {
            selectionSR.color = Color.white;
            selectionHighlightGO.SetActive(state);
        }
        public void SetAttackHighlightDamage(int physicalDamage) {
            int finalPhysicalDamage = physicalDamage - this.unit.CurrentMagicArmor > 0 ? physicalDamage : 0;
            AttackHighlight attackHighlight = attackHighlightGO.GetComponent<AttackHighlight>();
            attackHighlight.SetDamageValue(finalPhysicalDamage);
            DisplayAttackHighlight(true);
        }
        public void DisplayAttackHighlight(bool state) {            
            attackHighlightGO.SetActive(state);
        }
        public void DisplayDamagePopup(int value, Vector3 position) {
            GameObject damagePopupGO = Instantiate(damagePopupPrefab, position, Quaternion.identity);
            AttackHighlight damagePopup = damagePopupGO.GetComponentInChildren<AttackHighlight>();
            damagePopup.SetDamageValue(value);
            Destroy(damagePopupGO, 0.66f);
        }
        public void Move(Vector2 clickPoint, GameObject targetTileGO) {
            // return if there are no action points left
            if(this.UnitData.CurrentAp <= 0) { return; }

            MoveUnit(clickPoint, targetTileGO, this);
            
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
            int damageDealt = attackedUnitController.ReceiveDamage(attackDamageData);

            // remove attacked unit's attack highlight
            attackedUnitController.DisplayAttackHighlight(false);

            // display damage popup
            DisplayDamagePopup(damageDealt, attackedUnitPosition);

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
        private void ReceiveDebuffDamage(EffectData debuff) {
            
        }
        public int ReceiveDamage(DamageData damageData) {
            int finalDamage;
            int lethalRandom = Random.Range(0,100);
            if(lethalRandom < damageData.LethalChance) {
                this.unit.CurrentHp = 0;
                print($"{damageData.Source.name} just dealt a LETHAL attack to {this.name}! Instantly killing them!");
                Die();                
                return 999;
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
                    finalDamage = DamageCalculator.DealPhysicalDamage(physicalDamage, this.unit);
                    print($"{damageData.Source.name} just attacked {this.name} for {finalDamage} physical damage! Leaving them at {this.unit.CurrentHp}");                    
                    break;
                case DamageType.Magical:
                    int magicalDamage = damageData.Amount;
                    // final atk damage
                    finalDamage = DamageCalculator.DealMagicalDamage(magicalDamage, this.unit);
                    print($"{damageData.Source.name} just attacked {this.name} for {finalDamage} magical damage! Leaving them at {this.unit.CurrentHp}");
                    break;
                default:
                    finalDamage = 0;
                    break;
            }   
            if(this.unit.CurrentHp <= 0) {                
                Die();
            }         
            RefreshDetailsUi();            
            return finalDamage;                        
        }
        private void Die() {    
            // zero unit hp
            this.unit.CurrentHp = 0;
            // free the tile it dies in
            currentTile.SetHasObstacle(false);
            // send game event
            gameEventSystem.OnUnitDie(this.gameObject);            
        }
        public void OnPointerClick(PointerEventData eventData) {
            OnMouseClick();
        }
        public void OnPointerEnter(PointerEventData eventData) {
            unitDetailsUi.SetDetailsCanvasState(true);
        }
        public void OnPointerExit(PointerEventData eventData) {
            unitDetailsUi.SetDetailsCanvasState(false);
        }
    }
}