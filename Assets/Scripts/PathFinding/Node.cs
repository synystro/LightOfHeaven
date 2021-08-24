using UnityEngine;

namespace LUX.LightOfHeaven {
    [System.Serializable]
    public class Node : IHeapItem<Node> {

        public int gridX;
        public int gridY;

        public bool isWalkable;
        public Vector3 position;

        public Node Parent;
        public Node Child;

        public int gCost;
        public int hCost;

        public int fCost { get { return gCost + hCost; } }

        private int heapIndex;

        public Node(bool _isWalkable, Vector2 _position, int _gridX, int _gridY) {
            isWalkable = _isWalkable;
            position = _position;
            gridX = _gridX;
            gridY = _gridY;
        }
        public int HeapIndex {
            get {
                return heapIndex;
            }
            set {
                heapIndex = value;
            }
        }
        public int CompareTo(object obj) {
            Node nodeToCompare = obj as Node;
            int compare = fCost.CompareTo(nodeToCompare.fCost);
            if (compare == 0) {
                compare = hCost.CompareTo(nodeToCompare.hCost);
            }
            return -compare;
        }
    }
}