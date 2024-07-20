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



    private void OnEnable()
    {
        isRedFirst = false;
        maxPoints = startingPoints;
        //SwitchSides();
        //SetValues();
    }
    public void SetPoints(string points)
    {
        maxPoints = int.Parse(points);
        currentPoints = maxPoints;
        UpdateText();
    }
    public void SetValues()
    {
        //SwitchSides();
        //if (isRedFirst)
        maxPoints++;
        currentPoints = maxPoints;
        UpdateText();
    }

    public void SwitchSides()
    {
        isRedFirst = !isRedFirst;
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