using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Utility;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class AIPlacer : MonoBehaviour
{
    [SerializeField] private UnityEvent onFogOfWar;
    [SerializeField] private UnityEvent onEnemyEndTurn;
    [SerializeField] private UnitRenderEvent onEnemyPlace;
    private Board board;
    [SerializeField] private UnitBoardInfo fogOfWarRed;
    [SerializeField] private UnitBoardInfo fogOfWarBlue;
    [SerializeField] private Transform redUnitsParent;
    [SerializeField] private Transform blueUnitsParent;
    private List<UnitBoardInfo> redUnits = new();
    private List<UnitBoardInfo> blueUnits = new();
    private int weight;
    private List<UnitRenderer> positions;

    private void Awake()
    {
        board = FindObjectOfType<Board>();
        var reds = redUnitsParent.GetComponentsInChildren<UnitUIRenderer>();
        var blues = blueUnitsParent.GetComponentsInChildren<UnitUIRenderer>();
        foreach (var red in reds)
            redUnits.Add(red.GetBoardInfo());
        foreach (var blue in blues) blueUnits.Add(blue.GetBoardInfo());
        redUnits = redUnits.OrderBy(x => x.unitSettings.cost).ToList();
        blueUnits = blueUnits.OrderBy(x => x.unitSettings.cost).ToList();
    }

    public void AIFogOfWar()
    {
        //Debug.Log("AIPlacer.AIFogOfWar");
        weight = RedBlueTurn.maxPoints;
        // between 1 and 4
        var countEnemies = Random.Range(1 + weight / 6, Mathf.Min(weight, 5));


        var isRed = RedBlueTurn.IsRedFirst();
        var fogOfWar = isRed ? fogOfWarBlue : fogOfWarRed;
        positions = new List<UnitRenderer>(board.GetSquares(isRed ? SquareType.BLUE : SquareType.RED));
        countEnemies = Mathf.Min(countEnemies, positions.Count);
        GetListOfPositions(ref positions, countEnemies);
        //Debug.Log(positions);
        foreach (var unitRenderer in positions) unitRenderer.SetUnitSettings(fogOfWar);

        onFogOfWar?.Invoke();
    }

    public void AIFogOfWarOtherSide()
    {
        //Debug.Log("AIPlacer.AIFogOfWar");
        weight = RedBlueTurn.maxPoints;
        // between 1 and 4
        var countEnemies = Random.Range(1 + weight / 6, Mathf.Min(weight, 5));


        var isRed = !RedBlueTurn.IsRedFirst();
        var fogOfWar = isRed ? fogOfWarBlue : fogOfWarRed;
        positions = new List<UnitRenderer>(board.GetSquares(isRed ? SquareType.BLUE : SquareType.RED));
        countEnemies = Mathf.Min(countEnemies, positions.Count);
        GetListOfPositions(ref positions, countEnemies);
        //Debug.Log(positions);
        foreach (var unitRenderer in positions) unitRenderer.SetUnitSettings(fogOfWar);

        // onFogOfWar?.Invoke();
    }

    public void PlaceEnemies()
    {
        weight = Mathf.Min(weight, positions.Count * 5);
        if (positions.Count == 0)
        {
            onEnemyEndTurn?.Invoke();
            return;
        }

        var enemyList = RedBlueTurn.IsRedFirst() ? blueUnits : redUnits;
        var indexEnemy = new List<int>();
        var unitRenderers = new List<UnitRenderer>();
        while (positions.Count > 0 && weight > 0)
        {
            var min = positions.Count;
            var max = weight - (positions.Count - 1);
            if (min > max)
                min = max;
            var enemy = ChooseEnemy(min, max, enemyList);
            var currentW = enemy.unitSettings.cost;
            weight -= currentW;
            var unitRenderer = positions[Random.Range(0, positions.Count)];
            indexEnemy.Add(currentW - 1);
            unitRenderers.Add(unitRenderer);
            positions.Remove(unitRenderer);
        }

        if (weight > 0)
        {
            var enemyIndicies = indexEnemy;
            enemyIndicies = enemyIndicies.Where(en => en < 5).ToList();

            while (weight > 0)
            {
                enemyIndicies[Random.Range(0, enemyIndicies.Count)]++;
                Debug.Log(enemyIndicies);
                weight--;
                Debug.Log("Remaining weight:" + weight);
            }

            for (var i = 0; i < indexEnemy.Count; i++)
                if (unitRenderers[i].GetUnitSettings().unitSettings.cost < 5)
                    SetUnitRenderer(unitRenderers[i], enemyList[enemyIndicies[i]]);
                else
                    SetUnitRenderer(unitRenderers[i], enemyList[enemyList.Count - 1]);
        }
        else

        {
            for (var i = 0; i < unitRenderers.Count; i++)
                SetUnitRenderer(unitRenderers[i], enemyList[indexEnemy[i]]);
        }

        onEnemyEndTurn?.Invoke();
    }

    public void PlaceEnemiesOtherSide()
    {
        weight = Mathf.Min(weight, positions.Count * 5);
        if (positions.Count == 0)
        {
            onEnemyEndTurn?.Invoke();
            return;
        }

        var enemyList = !RedBlueTurn.IsRedFirst() ? blueUnits : redUnits;
        var indexEnemy = new List<int>();
        var unitRenderers = new List<UnitRenderer>();
        while (positions.Count > 0 && weight > 0)
        {
            var min = positions.Count;
            var max = weight - (positions.Count - 1);
            if (min > max)
                min = max;
            var enemy = ChooseEnemy(min, max, enemyList);
            var currentW = enemy.unitSettings.cost;
            weight -= currentW;
            var unitRenderer = positions[Random.Range(0, positions.Count)];
            indexEnemy.Add(currentW - 1);
            unitRenderers.Add(unitRenderer);
            positions.Remove(unitRenderer);
        }

        if (weight > 0)
        {
            var enemyIndicies = indexEnemy;
            enemyIndicies = enemyIndicies.Where(en => en < 5).ToList();

            while (weight > 0)
            {
                enemyIndicies[Random.Range(0, enemyIndicies.Count)]++;
                Debug.Log(enemyIndicies);
                weight--;
                Debug.Log("Remaining weight:" + weight);
            }

            for (var i = 0; i < indexEnemy.Count; i++)
                if (unitRenderers[i].GetUnitSettings().unitSettings.cost < 5)
                    SetUnitRenderer(unitRenderers[i], enemyList[enemyIndicies[i]]);
                else
                    SetUnitRenderer(unitRenderers[i], enemyList[enemyList.Count - 1]);
        }
        else

        {
            for (var i = 0; i < unitRenderers.Count; i++)
                SetUnitRenderer(unitRenderers[i], enemyList[indexEnemy[i]]);
        }

        //  onEnemyEndTurn?.Invoke();
    }

    private void SetUnitRenderer(UnitRenderer unitRenderer, UnitBoardInfo settings)
    {
        unitRenderer.SetUnitSettings(settings);
        onEnemyPlace?.Invoke(unitRenderer);
    }

    private UnitBoardInfo ChooseEnemy(int minWeight, int maxWeight, List<UnitBoardInfo> enemies)
    {
        var temp = enemies.Where(en => en.unitSettings.cost <= maxWeight && en.unitSettings.cost >= minWeight)
            .ToArray();

        // return temp[Random.Range(0, temp.Length)]; random
        //max
        return temp[Random.Range(0, temp.Length)];
    }

    private void GetListOfPositions(ref List<UnitRenderer> positions, int enemyCount)
    {
        if (positions.Count == 0)
            return;
        var choosenPositions = new List<UnitRenderer>();
        while (enemyCount > 0)
        {
            var index = Random.Range(0, positions.Count);
            enemyCount--;
            var position = positions[index];
            choosenPositions.Add(position);
            positions.Remove(position);
        }

        positions = choosenPositions;
    }
}