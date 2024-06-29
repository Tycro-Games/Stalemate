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
    private Vector2Int RedBlueWinCounts = Vector2Int.one * 3;
    private Vector2Int winCounts;

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
        RedBlueWinCounts.x = newWinCount;
        RedBlueWinCounts.y = newWinCount;
        UpdateWinCount(winCounts.x, winCounts.y);
    }

    private void UpdateWinCount(int redCount, int blueCount)
    {
        redUnitCount.text = redCount + "/" + RedBlueWinCounts.x;
        blueUnitCount.text = blueCount + "/" + RedBlueWinCounts.y;
        winCounts.x = redCount;
        winCounts.y = blueCount;
    }

    public void CheckWinCondition()
    {
        var redWin = winCounts.x >= RedBlueWinCounts.x;
        var blueWin = winCounts.y >= RedBlueWinCounts.y;

        if (winCounts.x >= RedBlueWinCounts.x || winCounts.y >= RedBlueWinCounts.y)
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