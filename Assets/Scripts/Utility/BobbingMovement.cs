using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    private GameObject EmptyParent;
    public float HeightOffset;
    public float BobbingSpeed;
    void OnEnable()
    {
        EmptyParent = new GameObject($"{gameObject.name} Offset Parent");
        EmptyParent.transform.parent = gameObject.transform.parent;
        gameObject.transform.parent = EmptyParent.transform;
#if UNITY_EDITOR
        Debug.unityLogger.logEnabled = true;
#else
        Debug.unityLogger.logEnabled = false;
#endif
    }

    void Update()
    {
        EmptyParent.transform.position =
            new(EmptyParent.transform.position.x, CalcOffset(), EmptyParent.transform.position.z);
    }

    float CalcOffset()
    {
        return HeightOffset * Mathf.Sin(BobbingSpeed * (float)Time.timeAsDouble);
    }
}
