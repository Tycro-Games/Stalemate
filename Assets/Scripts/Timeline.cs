using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Timeline : MonoBehaviour
{
    private List<UnitRenderer> red;
    private List<UnitRenderer> blue;
    private List<List<UnitData>> previosBoard = new();
    private List<UnitRenderer> currentBoard;

    [SerializeField] private Board board;
    [SerializeField] private UnitManager unitManager;

    private int index = 0;

    private void OnEnable()
    {
        UnitManager.onUnitManipulation += GetCurrentPieces;
    }

    private void OnDisable()
    {
        UnitManager.onUnitManipulation -= GetCurrentPieces;
    }

    public void GetCurrentPieces()
    {
        index++;
        currentBoard = board.pieces;
        var prevBoard = new List<UnitData>();
        foreach (var unit in currentBoard) prevBoard.Add(unit.Clone());
        previosBoard.Add(prevBoard);
        red = Board.GetAllPieces(SquareType.RED, ref currentBoard);
        blue = Board.GetAllPieces(SquareType.BLUE, ref currentBoard);
    }

    public void StepAhead()
    {
        index++;
        if (index > previosBoard.Count - 1)
        {
            index = previosBoard.Count - 1;
            return;
        }

        var prevBoard = previosBoard[index];
        for (var i = 0; i < prevBoard.Count; i++) board.pieces[i].SetUnitData(prevBoard[i]);
        unitManager.ResetRedBlueUnitLists();
    }

    public void GoBack()
    {
        index--;
        if (index < 0)
        {
            index = 0;
            return;
        }

        var prevBoard = previosBoard[index];
        for (var i = 0; i < prevBoard.Count; i++) board.pieces[i].SetUnitData(prevBoard[i]);
        unitManager.ResetRedBlueUnitLists();
    }
}