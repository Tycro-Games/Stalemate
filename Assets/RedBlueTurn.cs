using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RedBlueTurn : MonoBehaviour
{
    private int maxPoints = 0;
    private bool isRedFirst = false;
    [SerializeField] private UnityEvent onNextTurn;
    [SerializeField] private UnityEvent onRedTurn;
    [SerializeField] private UnityEvent onBlueTurn;

    private bool GetRedIsFirst()
    {
        return isRedFirst;
    }

    public void NextTurn()
    {
        onNextTurn?.Invoke();
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
    }
}