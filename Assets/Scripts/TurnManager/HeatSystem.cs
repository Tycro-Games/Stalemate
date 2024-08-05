using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Utility;
using UnityEngine;
using UnityEngine.Events;
public class HeatSystem : MonoBehaviour
{
    [SerializeField] private int redHeat;
    [SerializeField] private int blueHeat;
    [SerializeField] private int decreaseAmount = 2;
    [SerializeField] private int maxHeat = 3;

    [SerializeField] private int minHeat = 0;


    [SerializeField] private UnityEvent onRedIncrement;
    [SerializeField] private UnityEvent onBlueIncrement;
    [SerializeField] private UnityEvent onRedReset;
    [SerializeField] private UnityEvent onBlueReset;

    [SerializeField] private UnityEvent onMaxHeat;
    [SerializeField] private UnityEvent onNoMaxHeat;

    public void DecreaseRedBlue()
    {
        DecreaseBlue();
        DecreaseRed();
    }

    public void DecreaseRed()
    {
        redHeat -= decreaseAmount;
        if (redHeat < minHeat)
            redHeat = minHeat; ;
    }
    public void DecreaseBlue()
    {
        blueHeat -= decreaseAmount;
        if (blueHeat < minHeat)
            blueHeat = minHeat;
    }

    public void IncreaseHeatRed()
    {
        redHeat++;
        onRedIncrement?.Invoke();
        onBlueReset?.Invoke();

    }
    public void IncreaseHeatBlue()
    {
        blueHeat++;
        onBlueIncrement?.Invoke();
        onRedReset?.Invoke();

    }

    public void IncreaseHeat(string side)
    {
        IncrementHeat(side.Equals("Red"));
    }
    public void IncrementHeat(bool isRed)
    {
        if (isRed)
        {
            IncreaseHeatRed();
        }
        else
        {
            IncreaseHeatBlue();
        }
    }

    public void TriggerNationChoice()
    {
        onNoMaxHeat?.Invoke();

        if (redHeat >= maxHeat || blueHeat >= maxHeat)
        {
            onMaxHeat?.Invoke();
            Debug.Log("Full heat");
        }
        
    }
}
