using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Utility;
using UnityEngine;
using UnityEngine.Events;

public class HeatSystem : MonoBehaviour
{
    [SerializeField] private int redHeat;
    [SerializeField] private int blueHeat;
    [SerializeField] private int maxHeat = 3;
    [SerializeField] private int minHeat = 0;


    [SerializeField] private UnityEvent onRedIncrement;
    [SerializeField] private UnityEvent onBlueIncrement;
    [SerializeField] private UnityEvent onRedReset;
    [SerializeField] private UnityEvent onBlueReset;

    [SerializeField] private UnityEvent onMaxHeat;
    [SerializeField] private UnityEvent onNoMaxHeat;

    public void ResetValuesToZero()
    {
        ResetBlueToZero();
        ResetRedToZero();
    }

    public void ResetRedToZero()
    {
        redHeat = minHeat;
    }
    public void ResetBlueToZero()
    {
        blueHeat = minHeat;
    }

    public void IncreaseHeatRed()
    {
        redHeat++;
        if (redHeat >= maxHeat)
        {
            onMaxHeat?.Invoke();
        }
        else
        {
            onNoMaxHeat?.Invoke();
        }
    }
    public void IncreaseHeatBlue()
    {
        blueHeat++;
        if (blueHeat >= maxHeat)
        {
            onMaxHeat?.Invoke();
        }
        else
        {
            onNoMaxHeat?.Invoke();

        }
    }
    public void IncrementHeat(bool isRed)
    {
        if (isRed)
        {
            onRedIncrement?.Invoke();
            onBlueReset?.Invoke();
        }
        else
        {
            onBlueIncrement?.Invoke();
            onRedReset?.Invoke();
        }
    }

    public void TriggerNationChoice()
    {

    }
}
