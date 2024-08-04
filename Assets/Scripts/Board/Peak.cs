using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Board))]
[RequireComponent(typeof(UnitManager))]
public class Peak : MonoBehaviour
{
    private List<UnitRenderer> red;
    private List<UnitRenderer> blue;
    private List<UnitData> previousBoard;
    private List<UnitRenderer> currentBoard;

    private Board board;
    private UnitManager unitManager;
    private bool goBack = false;
    [SerializeField] private UnityEvent onPeak;
    [SerializeField] private UnityEvent onUnPeak;
    private UnitAttacker unitAttacker;
    private void Start()
    {
        board = GetComponent<Board>();
        unitManager = GetComponent<UnitManager>();
        unitAttacker = GetComponent<UnitAttacker>();
    }

    public void GetCurrentPieces()
    {
        currentBoard = board.pieces;
        previousBoard = new List<UnitData>();
        foreach (var unit in currentBoard) previousBoard.Add(unit.Clone());
        red = Board.GetAllPieces(SquareType.RED, ref currentBoard);
        blue = Board.GetAllPieces(SquareType.BLUE, ref currentBoard);
    }

    public void StepAhead()
    {
        GetCurrentPieces();
        var currentUnits = RedBlueTurn.IsRedFirst() ? red : blue;

        unitManager.MoveUnits(ref currentUnits);
        unitManager.AttackUnits(ref currentUnits);
        if (currentUnits.Count > 0)
        {
            unitManager.GetAttackingSquares(out List<Tuple<Vector2, AttackTypes>> attackingPos);
            if (attackingPos != null && attackingPos.Count > 0)
            {
                StartCoroutine(unitAttacker.PreviewAttackUnits(attackingPos, RedBlueTurn.IsRedFirst()));
            }
        }

        unitManager.BoostUnits(ref currentUnits);
        //just change 
        board.pieces = currentBoard;
        unitManager.ResetRedBlueUnitLists();
    }


    public void GoBack()
    {
        for (var i = 0; i < previousBoard.Count; i++)
            board.pieces[i].SetUnitData(previousBoard[i]);
        unitManager.ResetRedBlueUnitLists();
        unitAttacker.DeleteAllPreviews();
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
        AudioManager.instance.PlayOneShot(AudioManager.instance.peakSound);

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
        if (currentUnits.Count == 0) return;
        unitManager.MoveUnits(ref currentUnits);
        unitManager.AttackUnits(ref currentUnits);

        unitManager.BoostUnits(ref currentUnits);
    }
}