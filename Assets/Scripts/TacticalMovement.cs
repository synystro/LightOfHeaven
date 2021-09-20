using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LUX.LightOfHeaven {
    public class TacticalMovement : MonoBehaviour {
        [SerializeField] protected bool isFlying;
        [SerializeField] protected LayerMask groundLayer;  
        [SerializeField] protected TileController currentTile;
        [SerializeField] protected bool isFacingRight;
        [SerializeField] protected bool hasMovedThisTurn;

        protected UnityEngine.GameObject facingLeftModelGO;
        protected UnityEngine.GameObject facingRightModelGO; 

        public PathFinder PathFinder => pathFinder;
        protected PathFinder pathFinder;
        protected RangeFinder rangeFinder;

        // private void Awake() {
        //     pathFinder = this.GetComponent<PathFinder>();
        //     rangeFinder = this.GetComponent<RangeFinder>();    
        // }

        protected void SetFacingDirectionTowardsCoordX(int targetPositionX) {
            if (targetPositionX - this.transform.position.x > 0) {
                if (isFacingRight == false) {
                    isFacingRight = true;
                    facingRightModelGO.SetActive(true);
                    facingLeftModelGO.SetActive(false);
                }
            } else {
                if (isFacingRight == true) {
                    isFacingRight = false;
                    facingRightModelGO.SetActive(false);
                    facingLeftModelGO.SetActive(true);
                }
            }
        }
        protected List<UnityEngine.GameObject> GetReachableTiles() {
            if (hasMovedThisTurn) { return null; }           
            return rangeFinder.GetReachableTiles();
        }
        protected void MoveUnit(Vector2 clickPoint, UnityEngine.GameObject targetTileGO, UnitController unitController, bool ignoreAlreadyMoved) {
            TileController targetTile = targetTileGO.GetComponent<TileController>();
            
            if(unitController.IsEnemy == false) {
                unitController.ConsumeStamina(unitController.CurrentSp - targetTile.MovesLeft);
            } else {
                unitController.ConsumeStamina(1);
            }
            // return if the player has already moved an unit this turn
            if(unitController.PlayerController.HasMovedThisTurn == true && ignoreAlreadyMoved == false) { return; }
            // reset all tiles visual changes
            unitController.MapManager.ResetTiles();
            // free current tile
            currentTile.SetCurrentUnit(null);
            // change unit facing direction towards target tile
            SetFacingDirectionTowardsCoordX(Mathf.RoundToInt(clickPoint.x));
            // move to target tile
            this.transform.position = clickPoint + (Vector2)Utilities.IsoPosOffset;
            // set target tile to be the current tile
            currentTile = targetTile;
            // occupy it
            currentTile.SetCurrentUnit(unitController);           
            if(unitController.IsEnemy == false) {                
                unitController.PlayerController.SetHasMovedThisTurn(true);
            }                        
            hasMovedThisTurn = true;     
            // trigger on unit move event
            unitController.GameEventSystem.OnUnitMoved(unitController.IsEnemy);      
        }
    }
}