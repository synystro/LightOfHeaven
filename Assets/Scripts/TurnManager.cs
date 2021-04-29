using UnityEngine;
using Zenject;

namespace LUX {
    public enum TurnState { Start, Action, End }
    public class TurnManager : MonoBehaviour {
        [SerializeField] private int turnIndex;
        [SerializeField] private TurnState state;
        [Inject] private GameEventSystem gameEventSystem;
        [Inject] private AiController aiController;
        public bool IsPlayerTurn() { return turnIndex % 2 == 0; }  
        private void Start() {
            BeginTurn();
        }
        private void BeginTurn() {
            // if(IsPlayerTurn()) {
            //     print("Beginning Player's turn.");
            // } else {
            //     print("Beginning AI's turn");
            // }
            state = TurnState.Start;
            gameEventSystem.OnTurnStart();
            ActionPhase();
        }
        private void ActionPhase() {
            state = TurnState.Action;
            // if it's AI's turn
            if(IsPlayerTurn() == false) {
                aiController.StartTurn();
            }
        }
        public void EndTurn() {
            state = TurnState.End;
            gameEventSystem.OnTurnEnd();
            // reset all units and controllers
            turnIndex++;
            BeginTurn();
        }
    }
}
