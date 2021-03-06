using UnityEngine;
using Zenject;

namespace LUX.LightOfHeaven {
    public enum TurnState { Start, Action, End }
    public class TurnManager : MonoBehaviour {
        [SerializeField] private int turnIndex;
        [SerializeField] private int moveIndex;
        [SerializeField] private TurnState state;
        [Inject] private GameEventSystem gameEventSystem;
        [Inject] private AiController aiController;
        public bool IsEnemyTurn() { return moveIndex % 2 == 0; }  

        private void OnEnable() {
            gameEventSystem.onBattleStarted += Init;
        }
        private void OnDisable() {
            gameEventSystem.onBattleStarted -= Init;
        }
        public void Init() {
            BeginTurn();
        }
        private void BeginTurn() {
            // if(IsEnemyTurn()) {
            //     print("Beginning AI's turn.");
            // } else {
            //     print("Beginning Player's turn");
            // }
            state = TurnState.Start;
            if(IsEnemyTurn()) {
                gameEventSystem.OnTurnStarted();
            }
            ActionPhase();
        }
        private void ActionPhase() {
            state = TurnState.Action;
            // if it's AI's turn
            if(IsEnemyTurn()) {
                aiController.StartTurn();
            }
        }
        public void EndTurn() {
            state = TurnState.End;
            if(IsEnemyTurn() == false) {
                gameEventSystem.OnTurnEnded();
            }
            // reset all units and controllers
            moveIndex++;
            if(moveIndex % 2 == 0) {
                turnIndex++;
            }
            BeginTurn();
        }
    }
}
