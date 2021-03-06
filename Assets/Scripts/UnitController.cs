using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using Zenject;
using System.Linq;

namespace LUX.LightOfHeaven {

    public enum IntentType {
        Unkown,
        Attack,
        Block,
        Move,
        Debuff,
        Buff,
        Escape,
        Sleep,
        Stun,        
    }

    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(UnitStats))]
    [RequireComponent(typeof(EquipmentSystem))]
    [RequireComponent(typeof(Damage))]
    [RequireComponent(typeof(Effect))]
    [RequireComponent(typeof(PathFinder))]
    [RequireComponent(typeof(RangeFinder))]
    [RequireComponent(typeof(UnitDetailsUi))]
    public class UnitController : TacticalMovement, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler {
        [SerializeField] private List<EffectData> activeEffects;
        public List<EffectData> ActiveEffects => activeEffects;
        [Header("Status")]
        public int CurrentHp;
        public int CurrentEp;
        public int CurrentSp;
        public int CurrentPhyShield;
        public int CurrentMagShield;
        public int CurrentPhyArmor;
        public int CurrentMagArmor;
        [Space]
        [SerializeField] private List<Spell> spellPool;
        [SerializeField] private GameObject selectionHighlightGO;
        [SerializeField] private GameObject intentDisplayGO;
        [SerializeField] private GameObject incomingDamagePreviewGO;
        [SerializeField] private GameObject outgoingDamagePreviewGO;
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
        private SpriteRenderer intentDisplaySR;
        
        private Damage damage;
        private Effect effect;
        public UnitStats UnitStats => unitStats;
        private UnitStats unitStats;
        private UnitDetailsUi unitDetailsUi;   
        private bool isPlayer() => this == unitManager.Player; 

        private void Awake() {        
            spellPool = new List<Spell>();            
            enemiesInRange = new List<GameObject>();
            destructiblesInRange = new List<GameObject>();
            selectionSR = selectionHighlightGO.GetComponent<SpriteRenderer>();  
            intentDisplaySR = intentDisplayGO.GetComponent<SpriteRenderer>();                  
            unitDetailsUi = this.GetComponent<UnitDetailsUi>();
            pathFinder = this.GetComponent<PathFinder>();
            rangeFinder = this.GetComponent<RangeFinder>();  
            damage = this.GetComponent<Damage>();
            effect = this.GetComponent<Effect>();
            unitStats = this.GetComponent<UnitStats>();
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
            ResetUnitStatus();
            SetBaseStats();
            SetInitialStatus();
            ResetRestorableStatus();
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
            //print($"{this.gameObject.name}'s turn");
            HideIntent();     
            ResetUnitStatus();
            ApplyEffects();
            
            ResetRestorableStatus();

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
            DisplayIncomingDamagePreview(false);      
        }
        public void OnUnitAttacked(bool isEnemyAttack) {
            //RefreshDetails(); // already running inside receive damage function
        }
        public void AddEffect(EffectData effect) {
            if(activeEffects.Contains(effect) == false && (effect.Duration > 0 || effect.LastsTheEntireBattle))
                activeEffects.Add(effect);
        }
        public void RemoveEffect(EffectData effect) {
            if(activeEffects.Contains(effect))
                activeEffects.Remove(effect);
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

        public void ConsumeStamina(int amount) {
            CurrentSp -= amount;
            if(CurrentSp < 0)
                CurrentSp = 0;
            RefreshDetailsUi();
        }

        private void RefreshDetailsUi() {
            unitDetailsUi.Refresh(this);
        }

        private void SetBaseStats() {
            unitStats.Strength.BaseValue = unit.Strength;
            unitStats.Stamina.BaseValue = unit.Stamina;
            unitStats.Vitality.BaseValue = unit.Vitality;
            unitStats.Dexterity.BaseValue = unit.Dexterity;
            unitStats.Intelligence.BaseValue = unit.Intelligence;

            unitStats.Hp.BaseValue = unit.Hp;
            unitStats.Ep.BaseValue = unit.Ep;
            unitStats.Sp.BaseValue = unit.Sp;

            unitStats.PhyDamage.BaseValue = unit.PhyDamage;
            unitStats.MagDamage.BaseValue = unit.MagDamage;
            unitStats.AtkRange.BaseValue = unit.AtkRange;
            unitStats.AtkAccuracy.BaseValue = unit.AtkAccuracy;

            unitStats.PhyShield.BaseValue = unit.PhyShield;
            unitStats.MagShield.BaseValue = unit.MagShield;
            unitStats.PhyArmor.BaseValue = unit.PhyArmor;
            unitStats.MagArmor.BaseValue = unit.MagArmor;
            unitStats.Poise.BaseValue = unit.Poise;

            if(isPlayer())
                OnPlayerBaseStatsChangedCallback();
        }

        private void OnPlayerBaseStatsChangedCallback() {
            gameEventSystem.OnPlayerMaxHpChanged(unitStats.MaxHp);
        }

        private void SetInitialStatus() {
            CurrentHp = unitStats.MaxHp;                
            CurrentEp = unitStats.MaxEp;
            CurrentSp = unitStats.MaxSp;
            CurrentPhyShield = unitStats.PhyShield.Value;
            CurrentMagShield = unitStats.MagShield.Value;
            CurrentPhyArmor = unitStats.PhyArmor.Value;
            CurrentMagArmor = unitStats.MagArmor.Value;

            if(isPlayer())
                OnPlayerInitialStatsSetCallback();
        }

        private void OnPlayerInitialStatsSetCallback() {
            gameEventSystem.OnPlayerHpChanged(CurrentHp);
        }

        private void ResetRestorableStatus() {
            CurrentSp = unitStats.MaxSp;
            CurrentPhyShield = unitStats.PhyShield.Value;
            CurrentMagShield = unitStats.MagShield.Value;
        }

        private void ResetUnitStatus() {
            // reset status
            isStunned = false;
            //isSleeping, etc
            // reset modifiers        
        } 
        private void ApplyEffects() {
            effect.ApplyEffects(this);           
        }
        
        private void OnMouseClick() {
            // if clicked on an enemy unit
            if (isEnemy) {
                if (isTarget) {
                    // apply targetted spell to this enemy
                    if (playerController.SelectedEffect != null) {
                        //this.unit.ActiveEffects.Add(playerController.SelectedEffect);
                        playerController.SpellCastOn(this);
                        RefreshDetailsUi(); 
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
        public List<GameObject> GetEnemiesInRangeOf(int range, bool isSpell, bool ignoreObstacles) {
            return enemiesInRange = rangeFinder.GetEnemiesInRangeOf(this.currentTile, range, ignoreObstacles);
        }
        public List<GameObject> GetEnemiesInRangeOf(TileController tile, int range, bool isSpell, bool ignoreObstacles) {
            return enemiesInRange = rangeFinder.GetEnemiesInRangeOf(tile, range, ignoreObstacles);
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
        public void SetIntent(IntentType intent) {
            switch(intent) {
                case IntentType.Move: DisplayIntent(unitDetailsUi.IntentSprites.Where(i => i.intent == intent).FirstOrDefault().sprite); break;
                default: break;
            }
        }
        public void SetDamagePreview(DamageData damageData, bool incoming, bool ignoreBlock) {
            GameObject damageDisplayGO = incoming? incomingDamagePreviewGO : outgoingDamagePreviewGO;
            DamagePopup damagePopup = damageDisplayGO.GetComponent<DamagePopup>();
            int displayDamage = GetDisplayDamage(damageData, ignoreBlock); 

            damagePopup.SetDamageValue(displayDamage);
            damagePopup.SetDamageSpriteByDamageType(damageData.Type);

            if(incoming)
                DisplayIncomingDamagePreview(true);
            else
                DisplayOutgoingDamagePreview(true);
        }
        public void DisplayIntent(Sprite sprite) {
            intentDisplaySR.sprite = sprite;
            intentDisplayGO.SetActive(true);
        }
        public void HideIntent() {
            intentDisplayGO.SetActive(false);
        }
        public void DisplayIncomingDamagePreview(bool state) {            
            incomingDamagePreviewGO.SetActive(state);
        }
        public void DisplayOutgoingDamagePreview(bool state) {            
            outgoingDamagePreviewGO.SetActive(state);
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
            if(CurrentSp <= 0 && ignoreStamina == false) { return; }

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

            DamageData attackDamageData = new DamageData(
                this.unitStats,
                Mathf.RoundToInt((this.unitStats.AtkDamage + spellAtkDamage) *
                (1 + ((this.unitStats.Potent.Value - this.unitStats.Weak.Value) * .01f))),
                DamageType.Physical, this.unitStats.Critical,
                this.unitStats.Stun, this.unitStats.Lethal.Value
            );

            // attack!
            int damageDealt = attackedUnitController.Damage(attackDamageData);

            // remove attacked unit's attack highlight
            attackedUnitController.DisplayIncomingDamagePreview(false);

            // display damage popup 
            //DisplayDamagePopup(damageDealt, attackedUnitPosition);

            // call attacked unit's onAttacked function
            gameEventSystem.OnUnitAttacked(isEnemy);

            RefreshDetailsUi();     
            
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
            CurrentHp += amount;
            if(CurrentHp > unitStats.MaxHp) {
                CurrentHp = unitStats.MaxHp;
            }
            RefreshDetailsUi();           
        }
        public int Damage(DamageData damageData) {
            int damageTaken = damage.GetDamageTaken(damageData, this);
            // subtract this unit's hp by damage taken
            if (damageTaken > 0) {
                CurrentHp -= damageTaken;
            }
            // clamp hp, shields and armors
            ClampDefenseValues();
            // die if 0 hp or less
            if (CurrentHp <= 0) {                
                Die();
            }
            // display damage popup
            DisplayDamagePopup(damageTaken, damageData.Type, this.transform.position);
            RefreshDetailsUi();
            // call event
            if(isPlayer())
                gameEventSystem.OnPlayerHpChanged(this.CurrentHp);

            return damageTaken;
        }
        private int GetDisplayDamage(DamageData damageData, bool ignoreBlock) {
            int displayDamage = 0;
            switch(damageData.Type) {
                case DamageType.Physical:
                    float hexesModifiersPercentage = 1 + ((damageData.Source.Potent.Value - damageData.Source.Weak.Value) * .01f);
                    int previewDamage = Mathf.RoundToInt((damageData.Source.AtkDamage + damageData.Amount) * hexesModifiersPercentage);                    
                    displayDamage = ignoreBlock ? previewDamage : damage.GetPhysicalDamageOnUnit(previewDamage, this);                    
                    break;
                case DamageType.Magical: displayDamage = damage.GetMagicalDamageOnUnit(damageData.Amount, this); break;
                case DamageType.Piercing: displayDamage = damageData.Amount; break;
                default: break;
            }
            return displayDamage;
        }
        private void ClampDefenseValues() {
            if(CurrentHp <= 0) { CurrentHp = 0; }
            if(CurrentPhyShield <= 0) { CurrentPhyShield = 0; }
            if(CurrentMagShield <=0) { CurrentMagShield = 0; }
            if(CurrentPhyArmor <= 0) { CurrentPhyArmor = 0; }
            if(CurrentMagArmor <= 0) { CurrentMagArmor = 0; }
        }
        public void Die() {    
            // zero unit hp
            CurrentHp = 0;
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