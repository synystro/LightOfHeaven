using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LUX {
    public class TacticalMovement : MonoBehaviour {
        [SerializeField] protected bool isFlying;
        [SerializeField] protected LayerMask groundLayer;  
        [SerializeField] protected TileController currentTile;
        [SerializeField] protected bool isFacingRight;
        [SerializeField] protected bool hasMovedThisTurn;

        protected UnityEngine.GameObject facingLeftModelGO;
        protected UnityEngine.GameObject facingRightModelGO; 

        protected PathFinder pathFinder;

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
            return pathFinder.GetReachableTiles();
        }
        protected void MoveUnit(Vector2 clickPoint, UnityEngine.GameObject targetTileGO, UnitController unitController, bool ignoreAlreadyMoved) {
            TileController targetTile = targetTileGO.GetComponent<TileController>();
            // if this is the player consume aps here!
            if(unitController.IsEnemy == false) {
                unitController.UnitData.CurrentAp = targetTile.MovesLeft;
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
            this.transform.position = clickPoint;
            // set target tile to be the current tile
            currentTile = targetTile;
            // occupy it
            currentTile.SetCurrentUnit(unitController);           
            if(unitController.IsEnemy == false) {                
                unitController.PlayerController.SetHasMovedThisTurn(true);
            }                        
            hasMovedThisTurn = true;     
            // trigger on unit move event
            unitController.GameEventSystem.OnUnitMove(unitController.IsEnemy);      
        }
    }
}