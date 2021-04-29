﻿using UnityEngine;
using System.Collections.Generic;
using Zenject;

namespace LUX {
    public class AiController : MonoBehaviour {
        [SerializeField] private UnitController selectedUnitAi;
        [SerializeField] private LayerMask tileMask;
        [SerializeField] private UnitController randomPlayerTargetUnit;

        private bool hasMovedThisTurn;
        private bool hasAttackedThisTurn;

        public bool HasMovedThisTurn => hasMovedThisTurn;
        public bool HasAttackThisTurn => hasAttackedThisTurn;

        [Inject] private GameEventSystem gameEventSystem;
        [Inject] private UnitManager unitManager;
        [Inject] private TurnManager turnManager;

        private void OnEnable() {
            gameEventSystem.onTurnEnd += Reset;
        }
        private void OnDisable() {
            gameEventSystem.onTurnEnd -= Reset;
        }
        public void Reset() {
            hasMovedThisTurn = false;
            hasAttackedThisTurn = false;
            selectedUnitAi = null;
            randomPlayerTargetUnit = null;
        }
        public void StartTurn() {
            if(turnManager.IsPlayerTurn() == false) {
                StartActionPhase();
            }
        }
        private void StartActionPhase() {
            int randomIndex = Random.Range(0, unitManager.EnemyUnits.Count);  
            GameObject unitGO = unitManager.EnemyUnits[randomIndex];

            // below is the code for 1 unit
            
            selectedUnitAi = unitGO.GetComponent<UnitController>();
            selectedUnitAi.SetSelection(true);
            // decide player unit to attack
            ChooseRandomPlayerUnitAsTarget();
            // try to attack if player unit is in attack range
            selectedUnitAi.GetEnemiesInAtkRange();
            if(IsTargetInRange()) {
                Attack();
            }    
            // move 
            while(selectedUnitAi.UnitData.CurrentAp > 0 && IsTargetInRange() == false) {                
                // return if this enemy unit is now in attack range of a player unit
                if(selectedUnitAi.EnemiesInRange.Contains(randomPlayerTargetUnit.gameObject)) {
                    break;
                }
                Move();
            }            
            hasMovedThisTurn = true; // has moved this turn
            // try to attack if hasn't already and player unit is in attack range        
            if(IsTargetInRange()) {
                Attack();
            }
            turnManager.EndTurn(); // end this turn
        }
        private bool IsTargetInRange() {
            if(hasAttackedThisTurn) { return false; } //return false if already attacked this turn

            return selectedUnitAi.EnemiesInRange.Contains(randomPlayerTargetUnit.gameObject);
        }
        private void ChooseRandomPlayerUnitAsTarget() {
            // choose a random player unit to target
            int randomIndex = Random.Range(0, unitManager.PlayerUnits.Count);  
            GameObject randomPlayerUnitGO = unitManager.PlayerUnits[randomIndex];
            randomPlayerTargetUnit = randomPlayerUnitGO.GetComponent<UnitController>();
        }
        private void Move() {            
            if(hasMovedThisTurn || selectedUnitAi.UnitData.CurrentAp <= 0) { return; } // if has already moved (this turn), return

            // select a random player unit
            int randomIndex = Random.Range(0, unitManager.PlayerUnits.Count);
            GameObject randomPlayerUnitGO = unitManager.PlayerUnits[randomIndex];

            // display path towards player
            AstarPathFinding selectedUnitPF = selectedUnitAi.GetComponent<AstarPathFinding>();
            selectedUnitPF.FindPath(selectedUnitAi.transform.position, randomPlayerUnitGO.transform.position);
            Vector2 targetNodePos = selectedUnitPF.FinalPath[0].position;
            // get the tile
            TileController tileToMove;
            Collider2D tileHit = Physics2D.OverlapCircle(targetNodePos, 0.2f, tileMask);
            if(tileHit) {
                tileToMove = tileHit.GetComponent<TileController>();
            } else {
                tileToMove = null;
            }
            if(tileToMove != null) {
                selectedUnitAi.Move(tileToMove.transform.position, tileToMove.gameObject);
                // subtract 1 AP because it has moved 1 tile
                selectedUnitAi.UnitData.CurrentAp -= 1;                
            } else {
                Debug.LogError("Something is wrong with AI movement behaviour's LOGIC. You probably forgot to set tileLayer's mask");                
            }

            ///////////////////////selectedUnitAi.GetEnemiesInAtkRange();

        }
        private void Attack() {
            // if has already attacked (this turn), return
            if(hasAttackedThisTurn) { return; }

            selectedUnitAi.Attack(randomPlayerTargetUnit.UnitData, randomPlayerTargetUnit.transform.position);

            hasAttackedThisTurn = true;
        }     
    }
}
