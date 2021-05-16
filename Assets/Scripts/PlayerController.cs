using UnityEngine;
using Zenject;

namespace LUX {
    public class PlayerController : MonoBehaviour {
        [SerializeField] private GameObject playerGO;
        [SerializeField] private EffectData selectedEffect;
        [SerializeField] private GameObject selectedSpellButton;       

        private bool hasMovedThisTurn;
        private bool hasAttackedThisTurn;
        public GameObject PlayerGO => playerGO;
        public bool HasMovedThisTurn => hasMovedThisTurn;
        public bool HasAttackedThisTurn => hasAttackedThisTurn;
        public EffectData SelectedEffect => selectedEffect;
        public void SetPlayerGO(GameObject playerGO) { this.playerGO = playerGO; }
        public void SetHasMovedThisTurn(bool state) { hasMovedThisTurn = state; }
        public void SetHasAttackedThisTurn(bool state) { hasAttackedThisTurn = state; }
        public void SetSelectedTargetEffect(EffectData e) {
            selectedEffect = e;
            OnSelectedTargetEffect();
        }
        public void SetSelectedSpellButton(GameObject sbGO) { selectedSpellButton = sbGO; }
        public void SetSpellsUi(SpellsUi spellsUi) { this.spellsUi = spellsUi; }

        private InputMaster inputMaster;
        private SpellsUi spellsUi;

        [Inject] private GameEventSystem gameEventSystem;
        [Inject] private TurnManager turnManager;
        [Inject] private UnitManager unitManager;

        private void Awake() {
            inputMaster = new InputMaster();
        }

        private void OnEnable() {
            inputMaster.Enable();
            gameEventSystem.onTurnEnd += Reset;
            inputMaster.Player.EndTurn.performed += _ => EndTurn();
            inputMaster.Player.StartBattle.performed += _ => turnManager.Init();
        }
        private void OnDisable() {
            gameEventSystem.onTurnEnd -= Reset;
            inputMaster.Player.EndTurn.performed -= _ => EndTurn();
            inputMaster.Player.StartBattle.performed -= _ => turnManager.Init();
            inputMaster.Disable();
        }
        public void Reset() {
            hasMovedThisTurn = false;
            hasAttackedThisTurn = false;
        }
        public void EndTurn() {
            //print("Player ended their turn");
            spellsUi.CheckIfOutOfSpells();
            turnManager.EndTurn();
        }
        private void OnSelectedTargetEffect() {
            unitManager.HighlightEnemiesToTarget(true);
        }
        public void SpellWasCast() {
            unitManager.HighlightEnemiesToTarget(false);

            spellsUi.DeactivateSpellButton(selectedSpellButton);            
            
            selectedEffect = null;
        }
    }
}