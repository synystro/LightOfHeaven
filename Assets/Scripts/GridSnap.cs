using UnityEngine;

public class GridSnap : MonoBehaviour
{
    private Grid grid;
    private void Start() {
        grid = FindObjectOfType<Grid>();
    }
    private void Update() {
        Vector3Int cp = grid.LocalToCell(transform.localPosition);
        this.transform.position = grid.GetCellCenterLocal(cp);        
    }
}
