﻿using System.Collections.Generic;
using UnityEngine;

// this script finds gameobjects adjacent to this gameobject based on a distance

namespace LUX.LightOfHeaven {
    public class AdjacentFinder : MonoBehaviour {        
        [SerializeField] List<GameObject> adjacentObjects;
        [SerializeField] int distanceOffset = 2;
        [SerializeField] LayerMask targetLayer;

        private GameObject leftGO;
        private GameObject rightGO;
        private GameObject upGO;
        private GameObject downGO;

        public GameObject LeftGO => leftGO;
        public GameObject RightGO => rightGO;
        public GameObject UpGO => upGO;
        public GameObject DownGO => downGO;

        private void Awake() {
            adjacentObjects = new List<GameObject>(); 

            Vector2 offsetUp = new Vector2(this.transform.position.x, this.transform.position.y + distanceOffset);
            Vector2 offsetDown = new Vector2(this.transform.position.x, this.transform.position.y - distanceOffset);            
            Vector2 offsetLeft = new Vector2(this.transform.position.x - distanceOffset, this.transform.position.y);
            Vector2 offsetRight = new Vector2(this.transform.position.x + distanceOffset, this.transform.position.y);

            Collider2D hitUp = Physics2D.OverlapCircle(offsetUp, 0.2f, targetLayer);
            Collider2D hitDown = Physics2D.OverlapCircle(offsetDown, 0.2f, targetLayer);
            Collider2D hitLeft = Physics2D.OverlapCircle(offsetLeft, 0.2f, targetLayer);
            Collider2D hitRight = Physics2D.OverlapCircle(offsetRight, 0.2f, targetLayer);

            if(hitUp) { adjacentObjects.Add(hitUp.gameObject); upGO = hitUp.gameObject; }
            
            if(hitDown) { adjacentObjects.Add(hitDown.gameObject); downGO = hitDown.gameObject; }
            
            if(hitLeft) { adjacentObjects.Add(hitLeft.gameObject); leftGO = hitLeft.gameObject; }
            
            if(hitRight) { adjacentObjects.Add(hitRight.gameObject); rightGO = hitRight.gameObject; }           
        }
        public List<GameObject> GetAdjacentObjects() {
            return adjacentObjects;
        }
    }
}
