using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Utility;
using UnityEngine;
using UnityEngine.Events;

public class RedBlueTurn : MonoBehaviour
{
    private int maxPoints = 0;
    public static int CurrentPoints { get; set; }
    private static bool isRedFirst = false;

    public static bool IsRedFirst()
    {
        return isRedFirst;
    }


    [SerializeField] private UnityEvent onNextTurn;
    [SerializeField] private UnityEvent onFinishPlacement;
    [SerializeField] private StringEvent onScoreChange;
    [SerializeField] private UnityEvent onRedTurn;
    [SerializeField] private UnityEvent onBlueTurn;

    private bool GetRedIsFirst()
    {
        return isRedFirst;
    }

    public void NextTurn()
    {
        isRedFirst = !isRedFirst;
        if (isRedFirst)
        {
            onRedTurn?.Invoke();
            maxPoints++;
        }
        else
        {
            onBlueTurn?.Invoke();
        }

        CurrentPoints = maxPoints;
        onNextTurn?.Invoke();
        UpdateText();
    }

    public void UpdateText()
    {
        onScoreChange?.Invoke(CurrentPoints.ToString());
        if (CurrentPoints == 0) onFinishPlacement?.Invoke();
    }
}