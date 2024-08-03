using UnityEngine;
using UnityEngine.Events;

public class HoverMover : MonoBehaviour
{
    [SerializeField] private Vector2 minLimits;
    [SerializeField] private Vector2 maxLimits;
    [SerializeField] private UnityEvent onMove;
    private bool needsSound = false;
    public void Move(Vector3 pos)
    {
        transform.position = pos;
    }

    public void MoveClamped(Vector3 pos)
    {
        if (!gameObject.activeSelf)
            return;
        Vector3 newPos = new Vector3(Mathf.RoundToInt(Mathf.Clamp(pos.x, minLimits.x, maxLimits.x)),
            Mathf.RoundToInt(Mathf.Clamp(pos.y, minLimits.y, maxLimits.y)), 0);
        if (needsSound)
        {
            needsSound = false;
            onMove?.Invoke();
        }
        if (!transform.position.Equals(newPos))
            needsSound = true;
        transform.position = newPos;

    }

    void OnDisable()
    {
        needsSound = true;
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(transform.position + (Vector3)((minLimits + maxLimits) / 2.0f), (maxLimits));
    }
}