using Assets.Scripts.Utility;
using UnityEngine;

public class TurnModifiers : MonoBehaviour
{
    [SerializeField]
    private int startingOvertimeTurns = 11;
    [SerializeField]
    private int numberOfUnitsToLose = 2;

    [SerializeField]
    private IntEvent onModification;

    // only happens when the turn is equal to 6
    public void StartOvertime()
    {
        if (RedBlueTurn.currentTurn >= startingOvertimeTurns)
        {
            onModification?.Invoke(numberOfUnitsToLose);
            Debug.Log("Turn Modified");
        }
    }
}
