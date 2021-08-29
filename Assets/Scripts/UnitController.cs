using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using Zenject;
using System.Linq;

namespace LUX.LightOfHeaven {
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(Damage))]
    [RequireComponent(typeof(Effect))]
    [RequireComponent(typeof(PathFinder))]
    [RequireComponent(typeof(UnitDetailsUi))]
    public class UnitController : TacticalMovement, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler {
        [SerializeField] private List<Spell> spellPool;
        //[SerializeField] private List<EffectData> activeEffects;
        [SerializeField] private GameObject selectionHighlightGO;
        [SerializeField] private GameObject damagePreviewGO;
        [SerializeField] private GameObject damagePopupPrefab;
        [Expandable][SerializeField] private Unit unit;
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
        
        private Damage damage;
        private Effect effect;
        private UnitDetailsUi unitDetailsUi;    

        private void Awake() {            
            spellPool = new List<Spell>();            
            enemiesInRange = new List<GameObject>();
            destructiblesInRange = new List<GameObject>();
            selectionSR = selectionHighlightGO.GetComponent<SpriteRenderer>(); 
            pathFinder = this.GetComponent<PathFinder>();           
            unitDetailsUi = this.GetComponent<UnitDetailsUi>();
            damage = this.GetComponent<Damage>();
            effect = this.GetComponent<Effect>();
        }
        private void Start() {
            gameEventSystem.onTurnStarted += OnTurnStart;
            gameEventSystem.onTurnEnded += OnTurnEnd;
            gameEventSystem.onUnitAttacked += OnUnitAttacked;
        }
        private void OnDisable() {
            gameEventSystem.onTurnStarted -= OnTurnStart;
            gameEventSystem.onTurnEnded -= OnTurnEnd;
            gameEventSystem.onUnitAttacked -= OnUnitAttacked;
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
            unit.SetStats();
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
            print($"{this.gameObject.name}'s turn");        
            ResetUnitStats();
            ApplyEffects();
            
            this.unit.SetStats();
            this.unit.RestoreDepletedStats();

            RefreshDetailsUi();

            if(isStunned) {
                print($"{this.name} is stunned and lost their turn!");
                playerController.EndTurn();
            }                      
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
            if(this.unit.ActiveEffects.Contains(effect) == false)
                this.unit.ActiveEffects.Add(effect);
        }
        public void RemoveEffect(EffectData effect) {
            if(this.unit.ActiveEffects.Contains(effect))
                this.unit.ActiveEffects.Remove(effect);
        }
        public void Stun() {
            isStunned = true;
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
            // reset status
            isStunned = false;
            //isSleeping, etc
            // reset modifiers
            this.unit.ResetBonuses();            
        } 
        private void ApplyEffects() {
            effect.ApplyEffects(this);           
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
                        //this.unit.ActiveEffects.Add(playerController.SelectedEffect);
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
                case DamageType.Physical: displayDamage = damage.GetPhysicalDamageOnUnit(damageData.Amount + damageData.Source.AtkDamage, this.unit); break;
                case DamageType.Magical: displayDamage = damage.GetMagicalDamageOnUnit(damageData.Amount, this.unit); break;
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
            int damageDealt = attackedUnitController.Damage(attackDamageData);

            // remove attacked unit's attack highlight
            attackedUnitController.DisplayDamagePreview(false);

            // display damage popup 
            //DisplayDamagePopup(damageDealt, attackedUnitPosition);

            // call attacked unit's onAttacked function
            gameEventSystem.OnUnitAttacked(isEnemy);         
            
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
        public int Damage(DamageData damageData) {
            int damageTaken = damage.GetDamageTaken(damageData, this.unit);
            // subtract this unit's hp by damage taken
            if (damageTaken > 0) {
                unit.CurrentHp -= damageTaken;
            }
            // clamp hp, shields and armors
            ClampDefenseValues();
            // die if 0 hp or less
            if (this.unit.CurrentHp <= 0) {                
                Die();
            }
            // display damage popup
            DisplayDamagePopup(damageTaken, damageData.Type, this.transform.position);
            RefreshDetailsUi();            
            return damageTaken;                        
        }
        private void ClampDefenseValues() {
            if(this.unit.CurrentHp <= 0) { this.unit.CurrentHp = 0; }
            if(this.unit.CurrentShield <= 0) { this.unit.CurrentShield = 0; }
            if(this.unit.CurrentMagicShield <=0) { this.unit.CurrentMagicShield = 0; }
            if(this.unit.CurrentArmor <= 0) { this.unit.CurrentArmor = 0; }
            if(this.unit.CurrentMagicArmor <= 0) { this.unit.CurrentMagicArmor = 0; }
        }
        public void Die() {    
            // zero unit hp
            this.unit.CurrentHp = 0;
            // send game event
            gameEventSystem.OnUnitDied(this.gameObject);            
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