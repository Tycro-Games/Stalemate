using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Utility;
using UnityEngine;
using UnityEngine.Events;

public class TurnModifiers : MonoBehaviour
{
    [SerializeField] private int startingOvertimePoints = 6;
    [SerializeField] private int numberOfUnitsToLose = 2;

    [SerializeField] private IntEvent onModification;

    //only happens when the turn is equal to 6
    public void StartOvertime()
    {
        Debug.Log("Turn Modified");
        if (RedBlueTurn.currentPoints >= startingOvertimePoints) onModification?.Invoke(numberOfUnitsToLose);
    }
}