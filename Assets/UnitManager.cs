using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UnitManager : MonoBehaviour
{
    private List<UnitRenderer> RedUnits = new();
    private List<UnitRenderer> BlueUnits = new();
    [SerializeField] private Vector2Int inFrontRed = Vector2Int.down;
    [SerializeField] private Vector2Int inFrontBlue = Vector2Int.up;
    [SerializeField] private float timeBeforeMove = 1.0f;
    [SerializeField] private float timeAfterMove = 1.0f;
    [SerializeField] private UnityEvent onMoveEnd;
    [SerializeField] private UnityEvent onAttackEnd;
    private Board board;

    private void Start()
    {
        board = GetComponent<Board>();
    }

    public void AddNewUnit(UnitRenderer unit)
    {
        var settings = unit.GetUnitSettings();
        if (settings.isRed)
            RedUnits.Add(unit);
        else
            BlueUnits.Add(unit);
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

    private IEnumerator Movement(bool isRed)
    {
        yield return new WaitForSeconds(timeBeforeMove);
        if (isRed)
            MoveUnits(ref RedUnits, inFrontRed);
        else
            MoveUnits(ref BlueUnits, inFrontBlue);


        yield return new WaitForSeconds(timeAfterMove);
        onMoveEnd?.Invoke();
    }

    private IEnumerator Attack(bool isRed)
    {
        yield return new WaitForSeconds(timeBeforeMove);
        if (isRed)
        {
            AttackUnits(ref RedUnits, inFrontRed);
            CleanNullEnemies(ref BlueUnits);
        }
        else
        {
            AttackUnits(ref BlueUnits, inFrontBlue);
            CleanNullEnemies(ref RedUnits);
        }


        yield return new WaitForSeconds(timeAfterMove);
        onAttackEnd?.Invoke();
    }

    private void CleanNullEnemies(ref List<UnitRenderer> units)
    {
        for (var i = units.Count - 1; i >= 0; i--)
            if (units[i].GetUnitSettings() == null)
                units.RemoveAt(i);
    }

    private void MoveUnits(ref List<UnitRenderer> units, Vector2Int inFront)
    {
        for (var i = 0; i < units.Count; i++)
        {
            var newSquare = board.PieceInFront(units[i], inFront);
            Debug.Log(units[i].name);

            if (newSquare.GetUnitSettings() != null)
                continue;
            newSquare.SetUnitSettings(units[i].GetUnitSettings());
            units[i].SetUnitSettings(null);
            units[i] = newSquare;
        }
    }

    public void AttackUnits(ref List<UnitRenderer> units, Vector2Int inFront)
    {
        for (var i = 0; i < units.Count; i++)
        {
            var newSquare = board.PieceInFront(units[i], inFront);
            Debug.Log(units[i].name);
            var attackedSquareSettings = newSquare.GetUnitSettings();
            var squareSettings = units[i].GetUnitSettings();
            if (attackedSquareSettings == null || attackedSquareSettings.isRed == squareSettings.isRed)
                continue;

            //check health of the unit

            //if health is 0, remove the unit
            newSquare.SetUnitSettings(null);
        }
    }
}