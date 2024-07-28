using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
[RequireComponent(typeof(UnitMover))]
public class UnitManager : MonoBehaviour
{
    private List<UnitRenderer> redUnits = new();
    private List<UnitRenderer> blueUnits = new();
    [SerializeField] private float timeBeforeMove = 1.0f;
    [SerializeField] private float timeAfterMove = 1.0f;
    [SerializeField] private UnityEvent onMoveEnd;
    [SerializeField] private UnityEvent onAttackEnd;
    [SerializeField] private UnityEvent onBoostEnd;
    private Board board;
    private UnitMover mover;

    private bool isPlayerTurn = true;

    public static Action onUnitManipulation;
    public static Action<int, int> onWinConditionChange;

    [HideInInspector] public List<Transform> positions;
    private int redUnitsOnY = 0;
    private int blueUnitsOnY = 0;
    private bool hasMovedPriorityNation = false;

    private List<UnitRenderer> initialUnitSpace = new();
    private List<UnitRenderer> finalUnitSpace = new();
    public void SetCurrentSide(bool playerTurn)
    {
        if (hasMovedPriorityNation)
        {
            hasMovedPriorityNation = false;
            isPlayerTurn = !RedBlueTurn.IsRedFirst();

        }
        else
        {
            hasMovedPriorityNation = true;
            isPlayerTurn = RedBlueTurn.IsRedFirst();
        }


    }

    public void UpdateWinningCounts()
    {
        redUnitsOnY = 0;
        blueUnitsOnY = 0;
        foreach (var middleLine in positions)
        {
            foreach (var redUnit in redUnits.FindAll(x => (int)x.transform.position.x == (int)middleLine.position.x))
                if ((int)redUnit.transform.position.y <= middleLine.position.y)
                    redUnitsOnY++;
            foreach (var blueUnit in blueUnits.FindAll(x => (int)x.transform.position.x == (int)middleLine.position.x))
                if ((int)blueUnit.transform.position.y >= middleLine.position.y)
                    blueUnitsOnY++;
        }

        onWinConditionChange?.Invoke(redUnitsOnY, blueUnitsOnY);
    }

    public List<UnitRenderer> GetRedUnits()
    {
        return redUnits;
    }

    public List<UnitRenderer> GetBlueUnits()
    {
        return blueUnits;
    }

    private void Start()
    {
        board = GetComponent<Board>();
        mover = GetComponent<UnitMover>();
    }


    public void ResetRedBlueUnitLists()
    {
        redUnits = Board.GetAllPieces(SquareType.RED, ref board.pieces);
        blueUnits = Board.GetAllPieces(SquareType.BLUE, ref board.pieces);
        UpdateWinningCounts();
    }

    public void MoveCurrentSide()
    {

        StartCoroutine(Movement(isPlayerTurn));


    }

    public void AttackCurrentSide()
    {
        StartCoroutine(Attack(isPlayerTurn));

    }

    public void BoostCurrentSide()
    {
        StartCoroutine(Boost(isPlayerTurn));

    }


    private void MoveOtherSide()
    {

    }

    private void AttackOtherSide()
    {

    }

    private void BoostOtherSide()
    {

    }


    private IEnumerator Movement(bool isRed)
    {
        yield return new WaitForSeconds(timeBeforeMove);
        if (isRed)
            MoveUnits(ref redUnits);
        else
            MoveUnits(ref blueUnits);

      
        yield return StartCoroutine(mover.MoveUnits(initialUnitSpace, finalUnitSpace));

        yield return new WaitForSeconds(timeAfterMove);


        onMoveEnd?.Invoke();
        onUnitManipulation?.Invoke();
        UpdateWinningCounts();
    }

    private IEnumerator Boost(bool isRed)
    {
        yield return new WaitForSeconds(timeBeforeMove);
        if (isRed)
            BoostUnits(ref redUnits);
        else
            BoostUnits(ref blueUnits);


        yield return new WaitForSeconds(timeAfterMove);
        onBoostEnd?.Invoke();
        onUnitManipulation?.Invoke();
        UpdateWinningCounts();
    }

    private IEnumerator Attack(bool isRed)
    {
        yield return new WaitForSeconds(timeBeforeMove);
        if (isRed)
        {
            AttackUnits(ref redUnits);
            CleanNullEnemies(ref blueUnits);
        }
        else
        {
            AttackUnits(ref blueUnits);
            CleanNullEnemies(ref redUnits);
        }


        yield return new WaitForSeconds(timeAfterMove);
        onAttackEnd?.Invoke();
        onUnitManipulation?.Invoke();
        UpdateWinningCounts();
    }

    public void CleanNullEnemies(ref List<UnitRenderer> units)
    {
        for (var i = units.Count - 1; i >= 0; i--)
            if (units[i].GetUnitSettings().unitSettings == null)
                units.RemoveAt(i);
    }


    public void MoveUnits(ref List<UnitRenderer> units)
    {
        if (units.Count == 0)
            return;
        initialUnitSpace = new List<UnitRenderer>(new UnitRenderer[units.Count]);
        finalUnitSpace = new List<UnitRenderer>(new UnitRenderer[units.Count]);

        SortUnits(ref units);
        for (var i = 0; i < units.Count; i++)
        {
            var settings = units[i].GetUnitSettings();
            var sign = settings.isRed ? 1 : -1;

            if (initialUnitSpace[i] == null)
                initialUnitSpace[i] = units[i];
            //get settings
            for (var j = 0; j < settings.unitSettings.movePositions.Length; j++)
            {
                UnitRenderer newSquare = Board.PieceInFrontWithPadding(units[i], settings.unitSettings.movePositions[j] * sign,
                    board.pieces);
                if (newSquare == null)
                    continue;


                if (newSquare.GetUnitSettings().unitSettings != null)
                    continue;
                newSquare.SetUnitSettings(settings);


                finalUnitSpace[i] = newSquare;

                units[i].SetUnitSettings(new UnitBoardInfo());

                units[i] = newSquare;

            }
        }


    }

    private static void SortUnits(ref List<UnitRenderer> units)
    {
        var decreasingSortOrder = !units[0].GetUnitSettings().isRed;
        if (decreasingSortOrder)
            units.Sort((a, b) => b.transform.position.y.CompareTo(a.transform.position.y));
        else
            units.Sort((a, b) => a.transform.position.y.CompareTo(b.transform.position.y));
    }

    public void BoostUnits(ref List<UnitRenderer> units)
    {
        if (units.Count == 0)
            return;
        SortUnits(ref units);
        var piecesToBoost = new List<UnitRenderer>();
        for (var i = 0; i < units.Count; i++)
        {
            var settings = units[i].GetUnitSettings();
            if (!settings.unitSettings.boost)
                continue;
            var sign = settings.isRed ? 1 : -1;
            //this should only be one
            for (var j = 0; j < settings.unitSettings.boostPositions.Length; j++)
            {
                var newSquare = Board.PieceInFront(units[i], settings.unitSettings.boostPositions[j] * sign,
                    board.pieces);
                if (newSquare == null)
                    continue;

                var attackedSquareSettings = newSquare.GetUnitSettings();
                //move itself
                if (attackedSquareSettings.unitSettings == null || attackedSquareSettings.isRed != settings.isRed)
                {
                    Debug.Log("unit " + units[i].name + "boosted itself");

                    piecesToBoost.Add(units[i]);
                }
                //boost what is in the boost positions
                else
                {
                    Debug.Log("unit " + units[i].name + "boosted" + newSquare.name);

                    piecesToBoost.Add(newSquare);
                }
            }
        }

        if (piecesToBoost.Count == 0)
            return;
        piecesToBoost = piecesToBoost.Distinct().ToList();
        var isRed = piecesToBoost[0].GetUnitSettings().isRed;


        MoveUnits(ref piecesToBoost);
        AttackUnits(ref piecesToBoost);

        CleanNullEnemies(ref redUnits);
        CleanNullEnemies(ref blueUnits);
        if (isRed)
            redUnits.AddRange(piecesToBoost);
        else
            blueUnits.AddRange(piecesToBoost);

        ResetRedBlueUnitLists();
    }

    public void AttackUnits(ref List<UnitRenderer> units)
    {
        if (units.Count == 0)
            return;
        SortUnits(ref units);
        for (var i = 0; i < units.Count; i++)
        {
            var settings = units[i].GetUnitSettings();
            var sign = settings.isRed ? 1 : -1;

            for (var j = 0; j < settings.unitSettings.attackPositions.Length; j++)
            {
                var newSquare = Board.PieceInFront(units[i], settings.unitSettings.attackPositions[j] * sign,
                    board.pieces);
                if (newSquare == null) continue;
                //Debug.Log("unit " + units[i].name + " attacked:" + newSquare.name);
                var attackedSquareSettings = newSquare.GetUnitSettings();
                if (attackedSquareSettings.unitSettings == null || !attackedSquareSettings.isKillable || attackedSquareSettings.isRed == settings.isRed)
                    continue;

                //check health of the unit

                //if health is 0, remove the unit
                if (newSquare.TryToKill()) newSquare.SetUnitSettings(new UnitBoardInfo());
            }
        }
    }
}