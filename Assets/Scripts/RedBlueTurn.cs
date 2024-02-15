using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Utility;
using UnityEngine;
using UnityEngine.Events;

public class RedBlueTurn : MonoBehaviour
{
    [SerializeField] private int startingPoints = 0;
    public static int maxPoints;
    public static int currentPoints { get; set; }
    [SerializeField] private int endWave = 7;
    private static bool isRedFirst;
    [SerializeField] private UnityEvent onWin;

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
        isRedFirst = false;
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
        if (currentPoints == endWave)
            onWin?.Invoke();

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