using UnityEngine;

public class CellFinder : MonoBehaviour
{
    [SerializeField] private LayerMask layerToPlace = 0;
    private Camera cam;

    private void Start()
    {
        cam = Camera.main;
    }

    public GameObject NodeFromInput(Vector2 position)
    {
        if (Physics.Raycast(cam.ScreenPointToRay(position), out var hit, 50, layerToPlace))
            return hit.collider.gameObject;
        return null;
    }
}