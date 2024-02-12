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

    public void Attack()
    {
    }
}