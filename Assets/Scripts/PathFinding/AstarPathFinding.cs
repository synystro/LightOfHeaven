using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace LUX {
    public class AstarPathFinding : MonoBehaviour {
        public List<Node> FinalPath;

        [Inject] private PathFindingGrid grid;

        // private void Update() {
        //     if (targetTransform != null) {
        //         FindPath(this.transform.position, targetTransform.position);
        //     }
        // }
        public bool FindPath(Vector3 _startPosition, Vector3 _targetPosition, bool ignoreObstacles) {

            // rount target position to int
            _targetPosition.x = Mathf.RoundToInt(_targetPosition.x);
            _targetPosition.y = Mathf.RoundToInt(_targetPosition.y);

            // set field's position offset?
            _startPosition -= grid.transform.position;
            _targetPosition -= grid.transform.position;

            Node startNode = grid.NodeFromWorldPosition(_startPosition);
            Node targetNode = grid.NodeFromWorldPosition(_targetPosition);

            Heap<Node> OpenList = new Heap<Node>(grid.MaxSize);
            HashSet<Node> ClosedList = new HashSet<Node>();

            OpenList.Add(startNode);

            while (OpenList.Count > 0) {
                Node currentNode = OpenList.RemoveFirst();
                
                ClosedList.Add(currentNode);
                

                if (currentNode == targetNode) {
                    GetFinalPath(startNode, targetNode);
                    break;
                }

                foreach (Node neighbourNode in grid.GetNeighbourNodesDiagonal(currentNode)) {
                    // if(neighbourNode == targetNode && currentNode.isWalkable) {
                    //     targetNode.Child = currentNode;
                    // }
                    neighbourNode.Child = currentNode;
                    if (ignoreObstacles == false) {
                        if (neighbourNode.isWalkable == false || ClosedList.Contains(neighbourNode)) {
                            continue;
                        }
                    } else if (ClosedList.Contains(neighbourNode)) {
                        continue;
                    }

                    int moveCost = currentNode.gCost + GetManhattanDistance(currentNode, neighbourNode);
                    if (moveCost < neighbourNode.gCost || !OpenList.Contains(neighbourNode)) {
                        neighbourNode.gCost = moveCost;
                        neighbourNode.hCost = GetManhattanDistance(neighbourNode, targetNode);
                        neighbourNode.Parent = currentNode;

                        if (OpenList.Contains(neighbourNode) == false) {
                            OpenList.Add(neighbourNode);
                        }
                    }
                }
            }
            // if(targetNode.isWalkable == false) {
            //     targetNode = targetNode.Child;
            //     GetFinalPath(startNode, targetNode);
            // }      
            bool validPath = true;      
            if(targetNode.isWalkable == false) {
                validPath = false;
                foreach (Node n in grid.GetNeighbourNodesOrthogonal(targetNode)) {
                    if(n.isWalkable == true) {                            
                        targetNode = n;
                        GetFinalPath(startNode, targetNode);
                        validPath = true;
                        break;
                    }
                }
                if(validPath == false) {
                    Debug.LogWarning("No path!");
                }
            }
            //return targetNode.isWalkable ? true : false;
            return validPath;
        }
        void GetFinalPath(Node _startNode, Node _endNode) {
            FinalPath = new List<Node>();
            Node currentNode = _endNode;

            while (currentNode != _startNode) {
                FinalPath.Add(currentNode);
                currentNode = currentNode.Parent;
            }
            FinalPath.Reverse();

            grid.FinalPath = FinalPath;
        }
        int GetManhattanDistance(Node _nodeA, Node _nodeB) {
            int iX = Mathf.Abs(_nodeA.gridX - _nodeB.gridX);
            int iY = Mathf.Abs(_nodeA.gridY - _nodeB.gridY);

            return iX + iY;
        }
    }
}