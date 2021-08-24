using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using Zenject;
using System.Linq;

namespace LUX.LightOfHeaven {
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(PathFinder))]
    [RequireComponent(typeof(UnitDetailsUi))]
    public class UnitController : TacticalMovement, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler {
        [SerializeField] private List<Spell> spellPool;
        [SerializeField] private List<EffectData> activeEffects;
        [SerializeField] private GameObject selectionHighlightGO;
        [SerializeField] private GameObject damagePreviewGO;
        [SerializeField] private GameObject damagePopupPrefab;
        [SerializeField] private Unit unit;
        [SerializeField] private bool isEnemy;             
        [SerializeField] private bool hasTargetInRange;                  
        [SerializeField] private bool isSelected;
        [SerializeField] private bool isTarget;     
        [SerializeField] private bool isStunned;
        [SerializeField] private List<GameObject> enemiesInRange; 
        [SerializeField] private List<GameObject> destructiblesInRange;

        public List<Spell> SpellPool => spellPool;
        public Unit UnitData => unit;
        public TileController CurrentTile => currentTile;
        public bool HasMovedThisTurn => hasMovedThisTurn;
        public bool IsStunned => isStunned;
        public bool HasTargetInRange => hasTargetInRange;
        public bool IsEnemy => isEnemy;
        public bool IsFlying => isFlying;
        public bool IsTarget => isTarget;
        public List<GameObject> EnemiesInRange => enemiesInRange;
        public List<GameObject> DestructiblesInRange => destructiblesInRange;
        public void SetHasMoved(bool state) { hasMovedThisTurn = state; }
        public void SetHasTargetInRange(bool state) { hasTargetInRange = state; }

        public GameEventSystem GameEventSystem => gameEventSystem;
        public MapManager MapManager => mapManager;
        public PlayerController PlayerController => playerController;        

        [Inject] private PlayerController playerController;
        [Inject] private GameEventSystem gameEventSystem;
        [Inject] private TurnManager turnManager;
        [Inject] private UnitManager unitManager;
        [Inject] private MapManager mapManager;
        
        private SpriteRenderer selectionSR;   
        
        private UnitDetailsUi unitDetailsUi;    

        private void Awake() {            
            spellPool = new List<Spell>();
            activeEffects = new List<EffectData>();
            enemiesInRange = new List<GameObject>();
            destructiblesInRange = new List<GameObject>();
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
            unit.ResetBonuses();
            unit.Setup();
            unit.RestoreStats();
            // set unit gameobject's name
            this.gameObject.name = unit.name;
            // set spells pool
            ResetSpellsPool();

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
            hasTargetInRange = false;
            enemiesInRange.Clear();
            destructiblesInRange.Clear();
            DisplayDamagePreview(false);            
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
        public void ApplyEffect(EffectData e) {
            bool tookHit = false;
            switch(e.EffectType) {                
                case EffectType.Hp: unit.BonusHp += e.AmountOverTurns; unit.CurrentHp += e.AmountOverTurns; break;
                case EffectType.Heal: Heal(e.AmountOverTurns); break;
                case EffectType.Damage: 
                    if(e.OverTurnsDamageData.Amount > 0) {
                        this.ReceiveDamage(e.OverTurnsDamageData);
                        tookHit = true;
                     }
                     break;
                case EffectType.Stun: isStunned = true; this.ReceiveDamage(e.OverTurnsDamageData); break;
                default: break;
            }
            if(tookHit) {
                AudioManager.PlaySFX(e.TickSFX);                
            }
            if(this.unit.CurrentHp <= 0) {
                Die();                
            }
        }
        public void RemoveSpellFromPool(Spell s) {
            if(spellPool.Contains(s)) {
                spellPool.Remove(s);
            }
        }
        public void ResetSpellsPool() {
            spellPool.Clear();
            foreach(Spell s in unit.Spells) {
                spellPool.Add(s);
            }
        }
        private void ResetUnitStats() {
            // reset modifiers
            this.unit.ResetBonuses();
            // apply modifiers
            foreach(EffectData e in activeEffects.ToList()) {
                ApplyEffect(e);
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
                if (isTarget) {
                    // apply targetted spell to this enemy
                    if (playerController.SelectedEffect != null) {
                        //activeEffects.Add(playerController.SelectedEffect);
                        playerController.SpellCastOn(this);
                    }
                }
            }
            // if clicked on a player unit
            else {
                if(turnManager.IsEnemyTurn() == true) { return; } // return if it's the enemy's turn
                // untarget enemy units
                unitManager.UntargetEnemyUnits();
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
                }
            }
        }  
        private void LockSelection() {
            Color red = new Color(.5f,0f,0f,1f);
            selectionSR.color = red;
        }
        public List<GameObject> GetEnemiesInRangeOf(int tilesDistance, bool isSpell, bool ignoreObstacles) {
            return enemiesInRange = pathFinder.GetEnemiesInRangeOf(tilesDistance, ignoreObstacles);
        }
        public List<GameObject> GetDestructiblesInRangeOf(int tilesDistance, bool isSpell, bool ignoreObstacles) {
            return destructiblesInRange = pathFinder.GetDestructiblesInRangeOf(tilesDistance, ignoreObstacles);
        }
        public void SetSelection(bool state) {
            isSelected = state;  
            Highlight(state);          
        }  
        public void SetIsTarget(bool state) {
            isTarget = state;
        }
        public void Highlight(bool state) {
            selectionSR.color = Color.white;
            selectionHighlightGO.SetActive(state);
        }
        public void SetSpellPreviewDamage(DamageData damageData) {
            DamagePopup damagePopup = damagePreviewGO.GetComponent<DamagePopup>();
            int displayDamage = 0;
            switch(damageData.Type) {
                case DamageType.Physical: displayDamage = DamageHandler.GetPhysicalDamageOnUnit(damageData.Amount + damageData.Source.AtkDamage, this.unit); break;
                case DamageType.Magical: displayDamage = DamageHandler.GetMagicalDamageOnUnit(damageData.Amount, this.unit); break;
                case DamageType.Piercing: displayDamage = damageData.Amount; break;
                default: break;
            }
            damagePopup.SetDamageValue(displayDamage);
            damagePopup.SetDamageSpriteByDamageType(damageData.Type);
            DisplayDamagePreview(true);
        }
        public void DisplayDamagePreview(bool state) {            
            damagePreviewGO.SetActive(state);
        }
        public void DisplayDamagePopup(int value, DamageType damageType, Vector3 position) {
            GameObject damagePopupGO = Instantiate(damagePopupPrefab, position, Quaternion.identity);
            DamagePopup damagePopup = damagePopupGO.GetComponentInChildren<DamagePopup>();
            damagePopup.SetDamageValue(value);
            damagePopup.SetDamageSpriteByDamageType(damageType);
            Destroy(damagePopupGO, 0.66f);
        }
        public void Move(Vector2 clickPoint, GameObject targetTileGO, bool ignoreStamina) {
            // return if there are no action points left
            if(this.UnitData.CurrentAp <= 0 && ignoreStamina == false) { return; }

            MoveUnit(clickPoint, targetTileGO, this, ignoreStamina);

            // reset all tiles
            mapManager.ResetTiles();
            // display locked selection
            LockSelection();
        }        
        public void DealAttack(UnitController attackedUnitController, int spellAtkDamage, Vector3 attackedUnitPosition) {
            Unit attackedUnit = attackedUnitController.UnitData;

            // change unit facing direction towards enemy
            SetFacingDirectionTowardsCoordX(Mathf.RoundToInt(attackedUnitPosition.x));

            DamageData attackDamageData = new DamageData(this.unit, (this.unit.AtkDamage + spellAtkDamage), DamageType.Physical, this.unit.CritChance, this.unit.StunChance, this.unit.LethalChance);

            // attack!
            int damageDealt = attackedUnitController.ReceiveDamage(attackDamageData);

            // remove attacked unit's attack highlight
            attackedUnitController.DisplayDamagePreview(false);

            // display damage popup 
            //DisplayDamagePopup(damageDealt, attackedUnitPosition);

            // call attacked unit's onAttacked function
            gameEventSystem.OnUnitAttack(isEnemy);         
            
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
        public void Heal(int amount) {
            this.unit.CurrentHp += amount;
            if(this.unit.CurrentHp > this.unit.MaxHp) {
                this.unit.CurrentHp = this.unit.MaxHp;
            }            
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
                    finalDamage = DamageHandler.DealPhysicalDamageOnUnit(physicalDamage, this.unit);
                    //print($"{damageData.Source.name} just attacked {this.name} for {finalDamage} physical damage! Leaving them at {this.unit.CurrentHp}");                    
                    break;
                case DamageType.Magical:
                    int magicalDamage = damageData.Amount;
                    // final atk damage
                    finalDamage = DamageHandler.DealMagicalDamageOnUnit(magicalDamage, this.unit);
                    //print($"{damageData.Source.name} just attacked {this.name} for {finalDamage} magical damage! Leaving them at {this.unit.CurrentHp}");
                    break;
                case DamageType.Piercing:
                    finalDamage = damageData.Amount;
                    break;
                default:
                    finalDamage = 0;
                    break;
            }
            // subtract this unit's hp by finaldamage
            if (finalDamage > 0) {
                unit.CurrentHp -= finalDamage;
            }
            // clamp hp, shields and armors
            ClampDefenseValues();
            // die if hits 0 hp or less
            if (this.unit.CurrentHp <= 0) {                
                Die();
            }
            // display damage popup
            DisplayDamagePopup(finalDamage, damageData.Type, this.transform.position);
            RefreshDetailsUi();            
            return finalDamage;                        
        }
        private void ClampDefenseValues() {
            if(this.unit.CurrentHp <= 0) { this.unit.CurrentHp = 0; }
            if(this.unit.CurrentShield <= 0) { this.unit.CurrentShield = 0; }
            if(this.unit.CurrentMagicShield <=0) { this.unit.CurrentMagicShield = 0; }
            if(this.unit.CurrentArmor <= 0) { this.unit.CurrentArmor = 0; }
            if(this.unit.CurrentMagicArmor <= 0) { this.unit.CurrentMagicArmor = 0; }
        }
        private void Die() {    
            // zero unit hp
            this.unit.CurrentHp = 0;
            // send game event
            gameEventSystem.OnUnitDie(this.gameObject);            
        }
        public void OnPointerClick(PointerEventData eventData) {                   
            OnMouseClick();
        }
        public void OnPointerEnter(PointerEventData eventData) {
            unitDetailsUi.SetDisplayState(true);
        }
        public void OnPointerExit(PointerEventData eventData) {
            unitDetailsUi.SetDisplayState(false);
        }
    }
}