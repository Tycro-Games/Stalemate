using UnityEngine;
using UnityEngine.Events;

public class CellFinder : MonoBehaviour
{
    [SerializeField] private LayerMask layerToPlace = 0;
    private Camera cam;
    private bool isActive = true;
    [SerializeField] private UnityEvent onNoNode;
    [SerializeField] private UnityEvent onNode;
    private void Start()
    {
        cam = Camera.main;
    }

    public void SwitchActive()
    {
        isActive = !isActive;
    }
    public GameObject NodeFromInput(Vector2 position)
    {
        if (!isActive)
            return null;
        if (Physics.Raycast(cam.ScreenPointToRay(position), out var hit, 50, layerToPlace))
        {
            onNode?.Invoke();
            return hit.collider.gameObject;

        }

        onNoNode?.Invoke();
        return null;
    }
}