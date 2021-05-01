﻿using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using Zenject;

namespace LUX {  
    [RequireComponent(typeof(TileData))]  
    [RequireComponent(typeof(SpriteRenderer))]
    public class TileController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler {
        [Header("Status")]
        [SerializeField] private UnitController currentUnit;
        [SerializeField] private bool hasObstacle; 
        private int apsLeft;   
        [SerializeField] private bool isReachable;
        [SerializeField] private bool isInAtkRange;
        [SerializeField] private bool isMouseOver;
        [SerializeField] private List<TileController> adjacentTiles;
        [SerializeField] private TileController leftTile;
        [SerializeField] private TileController rightTile;
        [SerializeField] private TileController upTile;
        [SerializeField] private TileController downTile;
        [Header("Setup")] 
        [SerializeField] LayerMask obstacleLayer;
        [SerializeField] Color highlightColor;        
        private float hoverSize = 0.2f; 

        public TileData TileData => tileData;
        public List<TileController> AdjacentTiles => adjacentTiles;
        public bool HasObstacle => hasObstacle;
        public bool IsReachable => isReachable;
        public bool IsInAtkRange => isInAtkRange;
        public int ApsLeft => apsLeft;
        public UnitController CurrentUnit => currentUnit;
        public TileController LeftTile => leftTile;
        public TileController RightTile => rightTile;
        public TileController UpTile => upTile;
        public TileController DownTile => downTile;

        [Inject] private GameEventSystem gameEventSystem;
        [Inject] private UnitManager unitManager;

        private SpriteRenderer spriteRenderer;
        private TileData tileData;
        private AdjacentFinder adjacentFinder;
        
        private void Awake() {
            spriteRenderer = this.GetComponent<SpriteRenderer>();            
            tileData = this.GetComponent<TileData>();
            adjacentFinder = this.GetComponent<AdjacentFinder>();

            SetupAdjacentTiles(); 
        }
        private void Start() {
            // subscribe to events
            gameEventSystem.onTurnEnd += TurnEndReset;
        }
        private void SetupAdjacentTiles() {
            adjacentTiles = new List<TileController>();
            foreach(GameObject tileGO in adjacentFinder.GetAdjacentObjects()) {
                adjacentTiles.Add(tileGO.GetComponent<TileController>());              
            }
            if(adjacentFinder.LeftGO != null)
                leftTile = adjacentFinder.LeftGO.GetComponent<TileController>();
            if(adjacentFinder.RightGO != null)
                rightTile = adjacentFinder.RightGO.GetComponent<TileController>();
            if(adjacentFinder.UpGO != null)
                upTile = adjacentFinder.UpGO.GetComponent<TileController>();
            if(adjacentFinder.DownGO != null)
                downTile = adjacentFinder.DownGO.GetComponent<TileController>();   
        }
        private void OnDestroy() {
            gameEventSystem.onTurnEnd -= TurnEndReset;
        }
        private void TurnEndReset() {
            Reset();
        }
        public void SetCurrentUnit(UnitController unit) {
            currentUnit = unit;
        }
        public void SetHasObstacle(bool state) {
            hasObstacle = state;
        }
        public void SetAsReachable() {
            spriteRenderer.color = highlightColor;
            isReachable = true;
        }
        public void SetInAtkRange() {
            isInAtkRange = true;
        }
        public void SetApsLeft(int apsLeft) {
            this.apsLeft = apsLeft;
        }
        public void Reset() {
            spriteRenderer.color = Color.white;
            isInAtkRange = false;
            isReachable = false;
            apsLeft = 0;            
        }
        public void OnPointerEnter(PointerEventData eventData) {
            this.transform.localScale += Vector3.one * hoverSize;
        }
        public void OnPointerExit(PointerEventData eventData) {
            this.transform.localScale -= Vector3.one * hoverSize;
        }
        public void OnPointerClick(PointerEventData eventData) {
            if(isReachable == false || hasObstacle || unitManager.GetSelectedUnit() == null) { return; }
            unitManager.GetSelectedUnit().Move(this.transform.position, tileData.gameObject);
        }
    }
}