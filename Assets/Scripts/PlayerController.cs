using UnityEngine;
using Zenject;

namespace LUX {
    public class PlayerController : MonoBehaviour {
        private InputMaster inputMaster;
        private bool hasMovedThisTurn;
        private bool hasAttackedThisTurn;
        public bool HasMovedThisTurn => hasMovedThisTurn;
        public bool HasAttackedThisTurn => hasAttackedThisTurn;
        public void SetHasMovedThisTurn(bool state) { hasMovedThisTurn = state; }
        public void SetHasAttackedThisTurn(bool state) { hasAttackedThisTurn = state; }

        [Inject] GameEventSystem gameEventSystem;
        [Inject] TurnManager turnManager;

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
        private void EndTurn() {
            //print("Player ended their turn");
            turnManager.EndTurn();
        }
    }
}