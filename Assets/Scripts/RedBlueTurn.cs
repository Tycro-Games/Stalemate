using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Utility;
using UnityEngine;
using UnityEngine.Events;

public class RedBlueTurn : MonoBehaviour
{
    [SerializeField] private int startingPoints = 0;
    public static int maxPoints = 0;
    public static int currentPoints { get; set; }
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

    private void OnEnable()
    {
        maxPoints = startingPoints;
        SetValues();
    }

    public void SetValues()
    {
        isRedFirst = !isRedFirst;
        if (isRedFirst)
            maxPoints++;
        currentPoints = maxPoints;
        UpdateText();
    }

    public void NextTurn()
    {
        if (isRedFirst)
            onRedTurn?.Invoke();
        else
            onBlueTurn?.Invoke();

        onNextTurn?.Invoke();
    }

    public void UpdateText()
    {
        onScoreChange?.Invoke(currentPoints.ToString());
    }

    public void FinishPlacement()
    {
        onFinishPlacement?.Invoke();
    }
}