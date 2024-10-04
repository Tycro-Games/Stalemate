
using UnityEngine;
using UnityEngine.Events;

public class SimpleFollow: MonoBehaviour 
{

    [SerializeField] private Vector2 minLimits;
    [SerializeField] private Vector2 maxLimits;

    public void Move(Vector3 pos)
    {
        transform.position = pos;
    }

    
   
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(transform.position + (Vector3)((minLimits + maxLimits) / 2.0f), (maxLimits));
    }
}
