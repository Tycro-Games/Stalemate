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
    //[SerializeField] private AnimationCurve animationCurve;
    [SerializeField] private GameObject emptyAttackAnimation;

    private List<GameObject> spriteGameObject = new();

    private void Start()
    {


        //spriteGameObject.Add(Instantiate(emptyAttackAnimation, transform));

    }
    public IEnumerator AttackUnits(List<Tuple<Vector2, AttackTypes>> attackPositions, bool isRed)
    {
        for (int i = 0; i < attackPositions.Count; i++)
        {
            //add based on type
            GameObject exp = Instantiate(emptyAttackAnimation, attackPositions[i].Item1, Quaternion.identity, transform);
            exp.GetComponent<ExplosionRenderer>().DrawColor(isRed);
        }
        yield return null;
    }
}
