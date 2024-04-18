using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class UpdateWinCounts : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI redUnitCount;
    [SerializeField] private TextMeshProUGUI blueUnitCount;
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

    private void UpdateWinCount(int redCount, int blueCount)
    {
        redUnitCount.text = redCount + "/" + RedBlueWinCounts.x;
        blueUnitCount.text = blueCount + "/" + RedBlueWinCounts.y;
        winCounts.x = redCount;
        winCounts.y = blueCount;
    }

    public void CheckWinCondition()
    {
        if (winCounts.x >= RedBlueWinCounts.x || winCounts.y >= RedBlueWinCounts.y)
        {
            onLose?.Invoke();
            Debug.Log("You lose");
        }
        else
        {
            onNotLose?.Invoke();
        }
    }
}