using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AIPlacer : MonoBehaviour
{
    [SerializeField] private UnityEvent onFogOfWar;
    private Board board;

    private void Start()
    {
        board = FindObjectOfType<Board>();
    }

    public void AIFogOfWar()
    {
        Debug.Log("AIPlacer.AIFogOfWar");
        var weight = RedBlueTurn.CurrentPoints;
        //max between 1 and 4
        var countEnemies = Random.Range(1, Mathf.Min(weight, 5));


        var isRed = RedBlueTurn.IsRedFirst();
        var positions = new List<GameObject>(board.GetSquares(isRed ? SquareType.RED : SquareType.BLUE));


        onFogOfWar?.Invoke();
    }

    private void GetListOfPositions(ref List<GameObject> positions)
    {
    }
}