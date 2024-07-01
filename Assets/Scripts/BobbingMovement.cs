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
        //Create an empty Parent for the Unit
        EmptyParent = new GameObject($"{gameObject.name} Offset Parent");
        EmptyParent.transform.parent = gameObject.transform.parent;
        gameObject.transform.parent = EmptyParent.transform;
    }

    void Update()
    {
        EmptyParent.transform.position = new Vector3(EmptyParent.transform.position.x, CalcOffset(), EmptyParent.transform.position.z);
    }

    float CalcOffset()
    {
        //Caclulate the Offset
        return HeightOffset * Mathf.Sin(BobbingSpeed * Time.time);
    }
}
