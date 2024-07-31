using UnityEngine;

public class HoverMover : MonoBehaviour
{
    [SerializeField] private Vector2 minLimits;
    [SerializeField] private Vector2 maxLimits;


    public void Move(Vector3 pos)
    {
        transform.position = pos;
    }

    public void MoveClamped(Vector3 pos)
    {
        transform.position = new Vector3(Mathf.RoundToInt(Mathf.Clamp(pos.x, minLimits.x, maxLimits.x)),
            Mathf.RoundToInt(Mathf.Clamp(pos.y, minLimits.y, maxLimits.y)), 0);
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(transform.position + (Vector3)((minLimits + maxLimits) / 2.0f), (maxLimits));
    }
}