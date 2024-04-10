using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

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

    public static Action onUnitManipulation;

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
    }


    public void ResetRedBlueUnitLists()
    {
        redUnits = Board.GetAllPieces(SquareType.RED, ref board.pieces);
        blueUnits = Board.GetAllPieces(SquareType.BLUE, ref board.pieces);
    }

    public void MoveCurrentSide()
    {
        StartCoroutine(Movement(RedBlueTurn.IsRedFirst()));
    }

    public void AttackCurrentSide()
    {
        StartCoroutine(Attack(RedBlueTurn.IsRedFirst()));
    }

    public void MoveOtherSide()
    {
        StartCoroutine(Movement(!RedBlueTurn.IsRedFirst()));
    }

    public void AttackOtherSide()
    {
        StartCoroutine(Attack(!RedBlueTurn.IsRedFirst()));
    }

    public void BoostOtherSide()
    {
        StartCoroutine(Boost(!RedBlueTurn.IsRedFirst()));
    }

    public void BoostCurrentSide()
    {
        StartCoroutine(Boost(RedBlueTurn.IsRedFirst()));
    }

    private IEnumerator Movement(bool isRed)
    {
        yield return new WaitForSeconds(timeBeforeMove);
        if (isRed)
            MoveUnits(ref redUnits);
        else
            MoveUnits(ref blueUnits);


        yield return new WaitForSeconds(timeAfterMove);
        onMoveEnd?.Invoke();
        onUnitManipulation?.Invoke();
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
    }

    private void CleanNullEnemies(ref List<UnitRenderer> units)
    {
        for (var i = units.Count - 1; i >= 0; i--)
            if (units[i].GetUnitSettings() == null)
                units.RemoveAt(i);
    }

    public void MoveUnits(ref List<UnitRenderer> units)
    {
        if (units.Count == 0)
            return;
        SortUnits(ref units);
        for (var i = 0; i < units.Count; i++)
        {
            var settings = units[i].GetUnitSettings();
            var sign = settings.isRed ? 1 : -1;
            //get settings
            for (var j = 0; j < settings.movePositions.Length; j++)
            {
                var newSquare = Board.PieceInFrontWithPadding(units[i], settings.movePositions[j] * sign, board.pieces);
                if (newSquare == null)
                    continue;
                Debug.Log("unit " + units[i].name + " moved to:" + newSquare.name);


                if (newSquare.GetUnitSettings() != null)
                    continue;
                newSquare.SetUnitSettings(settings);
                units[i].SetUnitSettings(null);
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
            if (!settings.boost)
                continue;
            var sign = settings.isRed ? 1 : -1;
            //this should only be one
            for (var j = 0; j < settings.boostPositions.Length; j++)
            {
                var newSquare = Board.PieceInFront(units[i], settings.boostPositions[j] * sign, board.pieces);
                if (newSquare == null)
                    continue;

                var attackedSquareSettings = newSquare.GetUnitSettings();
                //move itself
                if (attackedSquareSettings == null || attackedSquareSettings.isRed != settings.isRed)
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

            for (var j = 0; j < settings.attackPositions.Length; j++)
            {
                var newSquare = Board.PieceInFront(units[i], settings.attackPositions[j] * sign, board.pieces);
                if (newSquare == null) continue;
                Debug.Log("unit " + units[i].name + " attacked:" + newSquare.name);
                var attackedSquareSettings = newSquare.GetUnitSettings();
                if (attackedSquareSettings == null || attackedSquareSettings.isRed == settings.isRed)
                    continue;

                //check health of the unit

                //if health is 0, remove the unit
                if (newSquare.TryToKill())
                    newSquare.SetUnitSettings(null);
            }
        }
    }
}