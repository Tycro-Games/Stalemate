using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    private GameObject EmptyParent;
    public float HeightOffset;
    public float BobbingSpeed;
    // Start is called before the first frame update
    void OnEnable()
    {//Create an empty Parent for the Unit
        EmptyParent = new GameObject($"{gameObject.name} Offset Parent");
        EmptyParent.transform.parent = gameObject.transform.parent;
        gameObject.transform.parent = EmptyParent.transform;
#if UNITY_EDITOR
        Debug.unityLogger.logEnabled = true;
#else
 Debug.unityLogger.logEnabled = false;
#endif
    }

    // Update is called once per frame
    void Update()
    {
        EmptyParent.transform.position = new(EmptyParent.transform.position.x, CalcOffset(), EmptyParent.transform.position.z);
    }

    float CalcOffset()
    {
        return HeightOffset * Mathf.Sin(BobbingSpeed * (float)Time.timeAsDouble);
    }
}
