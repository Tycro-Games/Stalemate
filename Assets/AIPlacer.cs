using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class AIPlacer : MonoBehaviour
{
    [SerializeField] private UnityEvent onFogOfWar;
    [SerializeField] private UnityEvent onEnemyPlacement;
    private Board board;
    [SerializeField] private ScriptableUnitSettings fogOfWarRed;
    [SerializeField] private ScriptableUnitSettings fogOfWarBlue;
    [SerializeField] private List<ScriptableUnitSettings> redUnits;
    [SerializeField] private List<ScriptableUnitSettings> blueUnits;
    private int weight;
    private List<UnitRenderer> positions;

    private void Start()
    {
        board = FindObjectOfType<Board>();
        redUnits = redUnits.OrderBy(x => x.cost).ToList();
        blueUnits = blueUnits.OrderBy(x => x.cost).ToList();
    }

    public void AIFogOfWar()
    {
        Debug.Log("AIPlacer.AIFogOfWar");
        weight = RedBlueTurn.maxPoints;
        //max between 1 and 4
        var countEnemies = Random.Range(1 + weight / 6, Mathf.Min(weight, 5));


        var isRed = RedBlueTurn.IsRedFirst();
        var fogOfWar = isRed ? fogOfWarBlue : fogOfWarRed;
        positions = new List<UnitRenderer>(board.GetSquares(isRed ? SquareType.BLUE : SquareType.RED));
        countEnemies = Mathf.Min(countEnemies, positions.Count);
        GetListOfPositions(ref positions, countEnemies);
        Debug.Log(positions);
        foreach (var unitRenderer in positions) unitRenderer.SetUnitSettings(fogOfWar);

        onFogOfWar?.Invoke();
    }

    public void PlaceEnemies()
    {
        weight = Mathf.Min(weight, positions.Count * 5);
        if (positions.Count == 0)
            return;
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
            var currentW = enemy.cost;
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
                if (unitRenderers[i].GetUnitSettings().cost < 5)
                    unitRenderers[i].SetUnitSettings(enemyList[enemyIndicies[i]]);
                else
                    unitRenderers[i].SetUnitSettings(enemyList[enemyList.Count - 1]);
        }
        else

        {
            for (var i = 0; i < unitRenderers.Count; i++)
                unitRenderers[i].SetUnitSettings(enemyList[indexEnemy[i]]);
        }

        onEnemyPlacement?.Invoke();
    }


    private ScriptableUnitSettings ChooseEnemy(int minWeight, int maxWeight, List<ScriptableUnitSettings> enemies)
    {
        var temp = enemies.Where(en => en.cost <= maxWeight && en.cost >= minWeight).ToArray();

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