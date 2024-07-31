using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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
    private bool goBack = false;
    [SerializeField] private UnityEvent onPeak;
    [SerializeField] private UnityEvent onUnPeak;

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
        var currentUnits = RedBlueTurn.IsRedFirst() ? red : blue;

        unitManager.MoveUnits(ref currentUnits);
        unitManager.AttackUnits(ref currentUnits);
        unitManager.BoostUnits(ref currentUnits);
        //just change 
        board.pieces = currentBoard;
        unitManager.ResetRedBlueUnitLists();
    }


    public void GoBack()
    {
        for (var i = 0; i < previosBoard.Count; i++)
            board.pieces[i].SetUnitData(previosBoard[i]);
        unitManager.ResetRedBlueUnitLists();
    }

    public void Peaking()
    {
        if (goBack)
        {
            GoBack();
            goBack = false;
            onUnPeak?.Invoke();
        }
        else
        {
            StepAhead();
            goBack = true;
            onPeak?.Invoke();
        }
    }

    //true means the non_AI side is moving
    public void EndTurnBoard(bool moveCurrentUnits)
    {
        if (goBack)
        {
            GoBack();
            goBack = false;
            onUnPeak?.Invoke();
        }
        else
        {
            bool isPlayerFirst = RedBlueTurn.IsPlayerFirst();
            if (moveCurrentUnits)
            {
                GetCurrentPieces();

                var currentUnits = isPlayerFirst ? red : blue;
                var otherUnits = !isPlayerFirst ? red : blue;
                MoveAttackBoostUnits(currentUnits);
                unitManager.CleanNullEnemies(ref otherUnits);
            }
            else
            {
                red = Board.GetAllPieces(SquareType.RED, ref currentBoard);
                blue = Board.GetAllPieces(SquareType.BLUE, ref currentBoard);
                var currentUnits = isPlayerFirst ? red : blue;
                var otherUnits = !isPlayerFirst ? red : blue;
                MoveAttackBoostUnits(otherUnits);

                unitManager.CleanNullEnemies(ref currentUnits);

                goBack = true;
                onPeak?.Invoke();
                board.pieces = currentBoard;
                unitManager.ResetRedBlueUnitLists();
            }
        }
    }

    private void MoveAttackBoostUnits(List<UnitRenderer> currentUnits)
    {
        unitManager.MoveUnits(ref currentUnits);
        unitManager.AttackUnits(ref currentUnits);
        unitManager.BoostUnits(ref currentUnits);
    }
}