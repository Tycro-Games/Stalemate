using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AttackTypes
{
    EMPTY_SPACE, DESTROY_UNIT, HIT_UNIT
}
public class UnitAttacker : MonoBehaviour
{
    public IEnumerator AttackUnits(List<Tuple<Vector2, AttackTypes>> attackPositions)
    {
        yield return null;
    }
}
