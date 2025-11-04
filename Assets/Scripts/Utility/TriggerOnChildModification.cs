using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.Utility
{
[ExecuteInEditMode]
public class TriggerOnChildModification : MonoBehaviour
{
    private int previousChildCount;
    [SerializeField]
    private UnityEvent onChildModification;

    private void Start()
    {
        previousChildCount = transform.hierarchyCount;
    }

    private void Update()
    {
        if (transform.hierarchyCount != previousChildCount)
        {
            Debug.Log("Children were added or deleted.");
            onChildModification?.Invoke();
            previousChildCount = transform.hierarchyCount;
        }
    }
}
}
