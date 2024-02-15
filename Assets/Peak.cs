using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Board))]
[RequireComponent(typeof(UnitManager))]
public class Peak : MonoBehaviour
{
    private List<UnitRenderer> red;
    private List<UnitRenderer> blue;
    private List<UnitData> previosBoard;
    private List<UnitRenderer> currentBoard;

    private Board board;
    private UnitManager unitManager;

    private void Start()
    {
        board = GetComponent<Board>();
        unitManager = GetComponent<UnitManager>();
    }

    public void GetCurrentPieces()
    {
        currentBoard = board.pieces;
        previosBoard = new List<UnitData>();
        foreach (var unit in currentBoard) previosBoard.Add(unit.Clone());
        red = Board.GetAllPieces(SquareType.RED, ref currentBoard);
        blue = Board.GetAllPieces(SquareType.BLUE, ref currentBoard);
    }

    public void StepAhead()
    {
        GetCurrentPieces();
        List<UnitRenderer> currentUnits;
        if (RedBlueTurn.IsRedFirst())
            currentUnits = red;
        else
            currentUnits = blue;

        unitManager.MoveUnits(ref currentUnits);
        unitManager.AttackUnits(ref currentUnits);
        unitManager.BoostUnits(ref currentUnits);
        //just change 
        board.pieces = currentBoard;
    }

    public void GoBack()
    {
        for (var i = 0; i < previosBoard.Count; i++) board.pieces[i].SetUnitData(previosBoard[i]);
        unitManager.ResetRedBlueUnitLists();
    }
}