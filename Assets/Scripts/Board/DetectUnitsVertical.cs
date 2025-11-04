using UnityEngine;

public class DetectUnitsVertical : MonoBehaviour
{
    private UnitManager manager;

    private void Start()
    {
        manager = GetComponentInParent<UnitManager>();
        manager.positions.Add(transform);
    }
}
