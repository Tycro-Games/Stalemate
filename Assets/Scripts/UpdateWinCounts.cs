using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class UpdateWinCounts : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI redUnitCount;
    [SerializeField] private TextMeshProUGUI blueUnitCount;
    [SerializeField] private UnityEvent onWinRed;
    [SerializeField] private UnityEvent onWinBlue;
    [SerializeField] private UnityEvent onLose;
    [SerializeField] private UnityEvent onNotLose;
    public static Vector2Int redBlueWinMaxCounts = Vector2Int.one * 3;
    public static Vector2Int winCounts;

    private void OnEnable()
    {
        UnitManager.onWinConditionChange += UpdateWinCount;
    }

    private void OnDisable()
    {
        UnitManager.onWinConditionChange -= UpdateWinCount;
    }

    public void SetWinCounts(int newWinCount)
    {
        redBlueWinMaxCounts.x = newWinCount;
        redBlueWinMaxCounts.y = newWinCount;
        UpdateWinCount(winCounts.x, winCounts.y);
    }

    private void UpdateWinCount(int redCount, int blueCount)
    {
        redUnitCount.text = redCount + "/" + redBlueWinMaxCounts.x;
        blueUnitCount.text = blueCount + "/" + redBlueWinMaxCounts.y;
        winCounts.x = redCount;
        winCounts.y = blueCount;
    }

    public void CheckWinCondition()
    {
        var redWin = winCounts.x >= redBlueWinMaxCounts.x;
        var blueWin = winCounts.y >= redBlueWinMaxCounts.y;

        if (winCounts.x >= redBlueWinMaxCounts.x || winCounts.y >= redBlueWinMaxCounts.y)
        {
            onLose?.Invoke();
            Debug.Log("You lose");
        }
        else
        {
            onNotLose?.Invoke();
        }

        if (redWin)
        {
            onWinRed?.Invoke();
            Debug.Log("Red wins");
        }

        if (blueWin)
        {
            onWinBlue?.Invoke();
            Debug.Log("Blue wins");
        }
    }
}