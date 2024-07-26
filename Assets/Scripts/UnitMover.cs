using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitMover : MonoBehaviour
{
    public IEnumerator MoveUnits(List<UnitRenderer> init, List<UnitRenderer> fin)
    {
        for (int i = 0; i < init.Count; i++)
        {
            Debug.Log(init[i].name + " " + fin[i].name);




        }

        yield return null;
    }
}
