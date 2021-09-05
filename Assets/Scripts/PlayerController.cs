using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace LUX.LightOfHeaven {
    public class PlayerController : MonoBehaviour {
        [SerializeField] private Equippable longSword;
        [SerializeField] private UnitController playerUnitController;
        [SerializeField] private GameObject playerGO;
        [SerializeField] private EffectData selectedEffect;
        [SerializeField] private GameObject selectedSpellButton;       

        private bool hasMovedThisTurn;
        private bool hasAttackedThisTurn;
        public UnitController PlayerUnitController => playerUnitController;
        public GameObject PlayerGO => playerGO;
        public bool HasMovedThisTurn => hasMovedThisTurn;
        public bool HasAttackedThisTurn => hasAttackedThisTurn;
        public EffectData SelectedEffect => selectedEffect;
        public void SetPlayerGO(GameObject p) { playerGO = p; GetPlayerComponents(); }
        public void SetHasMovedThisTurn(bool state) { hasMovedThisTurn = state; }
        public void SetHasAttackedThisTurn(bool state) { hasAttackedThisTurn = state; }        
        public void SetSelectedSpellButton(GameObject sbGO) { selectedSpellButton = sbGO; }
        public void SetSpellsUi(SpellsUi s) { spellsUi = s; }

        private InputMaster inputMaster;
        private SpellsUi spellsUi;
        private EquipmentSystem equipmentSystem;

        [Inject] private GameEventSystem gameEventSystem;
        [Inject] MapManager mapManager;
        [Inject] private TurnManager turnManager;
        [Inject] private UnitManager unitManager;

        private void Awake() {
            inputMaster = new InputMaster();
        }

        private void OnEnable() {
            inputMaster.Enable();
            gameEventSystem.onTurnEnded += Reset;
            inputMaster.Player.EndTurn.performed += _ => EndTurn();
        }
        private void OnDisable() {
            gameEventSystem.onTurnEnded -= Reset;
            //gameEventSystem.onEquipped -= playerUnitController.UnitData.AddBonus;
            inputMaster.Player.EndTurn.performed -= _ => EndTurn();
            inputMaster.Disable();
        }
        private void GetPlayerComponents() {
            playerUnitController = playerGO.GetComponent<UnitController>();
            equipmentSystem = playerGO.GetComponent<EquipmentSystem>();

            //gameEventSystem.onEquipped += playerUnitController.UnitData.AddBonus;
        }
        public void Reset() {
            hasMovedThisTurn = false;
            hasAttackedThisTurn = false;
        }
        public void EndTurn() {
            spellsUi.CheckIfOutOfSpells();
            turnManager.EndTurn();
        }
        public void EquipSword() {
            equipmentSystem.Equip(longSword, playerUnitController);
            //equipmentSystem.Equip(longSword);
        }
        public void PlayerToStartingPosition() {
            playerUnitController.Move(mapManager.StartingTiles[0].transform.position, mapManager.StartingTiles[0].gameObject, true);
        }
        public void SetSelectedEffect(EffectData e) {
            selectedEffect = e;
        }
        public void OnSelectedTargetEffect(EffectData e) {
            List<GameObject> enemiesInRange = playerGO.GetComponent<UnitController>().GetEnemiesInRangeOf(e.Range, true, e.IgnoreObstacles);
            foreach(GameObject enemy in enemiesInRange) {
                UnitController enemyController = enemy.GetComponent<UnitController>();
                enemyController.SetSpellPreviewDamage(e.InstantDamageData);
                enemyController.DisplayDamagePreview(true);
                enemyController.SetIsTarget(true);
                enemyController.Highlight(true);
            }
        }
        public void SpellCastOn(UnitController targetUnitController) {
            // apply effect to target unit
            targetUnitController.AddEffect(selectedEffect);

            SpellCast spellCast = selectedSpellButton.GetComponent<SpellCast>();

            spellCast.CastOnTarget(targetUnitController);

            // deactivate spell button
            spellsUi.DeactivateSpellButton(selectedSpellButton);            
            // deselect effect
            selectedEffect = null;
        }
        public void SpellSelfTarget() {
            SpellCast spellCast = selectedSpellButton.GetComponent<SpellCast>();
            spellCast.CastOnTarget(playerUnitController);
            // deactivate spell button
            spellsUi.DeactivateSpellButton(selectedSpellButton);            
            // deselect effect
            selectedEffect = null;
        }
    }
}