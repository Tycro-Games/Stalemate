using System;
using Assets.Scripts.Utility;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class Spawning
{
    //0 to 3
    public List<int> placement;

    public Spawning()
    {
        placement = new List<int> { 0, 0, 0, 0 };
    }

    public Spawning(int _first, int _second, int _third, int _forth)
    {
        placement = new List<int> { 0, 0, 0, 0 };

        placement[0] = _first;
        placement[1] = _second;
        placement[2] = _third;
        placement[3] = _forth;
    }

    public Spawning(ref Spawning spawning)
    {
        placement = new List<int>(4);

        placement[0] = spawning.placement[0];
        placement[1] = spawning.placement[1];
        placement[2] = spawning.placement[2];
        placement[3] = spawning.placement[3];
    }
}

public class AIPlacer : MonoBehaviour
{
    [SerializeField] private UnityEvent onFogOfWar;
    [SerializeField] private UnityEvent onEnemyEndTurn;
    [SerializeField] private UnitRenderEvent onEnemyPlace;
    private Board board;
    private Peak peak;
    [SerializeField] private UnitBoardInfo fogOfWarRed;
    [SerializeField] private UnitBoardInfo fogOfWarBlue;
    [SerializeField] private Transform redUnitsParent;
    [SerializeField] private Transform blueUnitsParent;
    [SerializeField] private ScriptableAI redAI;
    [SerializeField] private ScriptableAI blueAI;


    //auxiliary variables
    private List<UnitBoardInfo> redUnits = new();
    private List<UnitBoardInfo> blueUnits = new();
    private int weight;
    private List<UnitRenderer> positions;
    private List<int> indexEnemy;
    private List<UnitRenderer> unitRenderers;
    private List<UnitBoardInfo> enemyList;

    private List<int> enemyIndicies;

    //AI permutations
    private List<Spawning> validSpawns = new();
    private int spawningsCount;
    private int emptySlots = 4;

    public void Init()
    {
        board = FindObjectOfType<Board>();
        peak = FindObjectOfType<Peak>();
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
        ChooseEnemies();

        onFogOfWar?.Invoke();
    }

    [SerializeField] private FloatEvent onTotalSpawnings;
    private bool isRed;

    //the same as AIFogOfWar but with no event trigger
    public void AiSpawning()
    {
        ChooseEnemies();
        onTotalSpawnings?.Invoke(spawningsCount - 1);
    }

    private List<Tuple<Spawning, int>> spawnsScore;

    private void ChooseEnemies()
    {
        isRed = RedBlueTurn.IsPlayerFirst();
        var aiSettings = isRed ? blueAI : redAI;

        enemyIndicies = new List<int>();
        unitRenderers = new List<UnitRenderer>();
        indexEnemy = new List<int>();

        if (aiSettings.random)
        {
            if (RandomSpawn(isRed))
                return;
        }
        //this AI chooses stuff
        else
        {
            //Get all possible positions
            weight = RedBlueTurn.currentPoints;
            //min 1


            positions = new List<UnitRenderer>(board.GetEmptySquares(isRed ? SquareType.BLUE : SquareType.RED));
            if (positions.Count == 0)
                return;

            weight = Mathf.Min(weight, positions.Count * 5);
            enemyList = isRed ? blueUnits : redUnits;
            //call to recursive backtracking
            indexEnemy = new List<int>();

            GenerateSpawnings();
            //Assign scores based on end conditions
            spawnsScore = new List<Tuple<Spawning, int>>();

            for (var i = 0; i < validSpawns.Count; i++)
            {
                peak.EndTurnBoard(true);
                ChooseSpawning(i);
                peak.EndTurnBoard(false);
                RateBoard();
                //go back to the original state
                peak.EndTurnBoard(false);


                spawnsScore.Add(new Tuple<Spawning, int>(validSpawns[i], score));
            }

            //Sort them based on scores
            spawnsScore = spawnsScore.OrderByDescending(spawn => spawn.Item2).ToList();
            validSpawns = spawnsScore.Select(spawnScore => spawnScore.Item1).ToList();
            var fogOfWar = isRed ? fogOfWarBlue : fogOfWarRed;

            var choiceIndex = 0;
            ChooseMaximalSpawning(ref choiceIndex);
            if (choiceIndex == -1) Debug.Log("Choice index for option is invalid");

            var placement = validSpawns[int.Parse(choiceIndex.ToString())];

            var indexUnit = 0;
            foreach (var unitSlot in positions)
            {
                if (placement.placement[indexUnit] > 0)
                {
                    SetUnitRenderer(unitSlot, fogOfWar);
                    unitRenderers.Add(unitSlot);
                    indexEnemy.Add(placement.placement[indexUnit] - 1);
                }

                indexUnit++;
            }
        }
    }

    public void ChooseMaximalSpawning(ref int index)
    {
        index = -1;
        // Assuming spawnsScore is sorted and validSpawns is aligned with it
        var highestScore = spawnsScore.First().Item2; // Get the highest score

        var highestScoreIndices = new List<int>();

        for (var i = 0; i < spawnsScore.Count; i++)
            if (spawnsScore[i].Item2 == highestScore)
                highestScoreIndices.Add(i); // Add index to list
            else
                break; // Since spawnsScore is sorted, no need to check further once a lower score is found

        index = Random.Range(0, highestScoreIndices.Count);
        //ChooseSpawningRate(index);
    }

    public void DebugChooseMaximalSpawning()
    {
        // Assuming spawnsScore is sorted and validSpawns is aligned with it
        var highestScore = spawnsScore.First().Item2; // Get the highest score

        var highestScoreIndices = new List<int>();

        for (var i = 0; i < spawnsScore.Count; i++)
            if (spawnsScore[i].Item2 == highestScore)
                highestScoreIndices.Add(i); // Add index to list
            else
                break; // Since spawnsScore is sorted, no need to check further once a lower score is found

        var index = Random.Range(0, highestScoreIndices.Count);
        ChooseSpawningRate(index);
    }

    public void ClearPositions()
    {
        positions = new List<UnitRenderer>(board.GetSquares(isRed ? SquareType.BLUE : SquareType.RED));

        for (var i = 0; i < positions.Count; i++) positions[i].SetUnitSettings(new UnitBoardInfo());
    }

    private void GenerateSpawnings()
    {
        emptySlots = positions.Count - 1;
        validSpawns = new List<Spawning>();
        var spawning = new Spawning();
        spawningsCount = 0;
        Backtracking(0, ref spawning);
        Debug.Log(spawningsCount);
    }

    private bool IsValid(List<int> spawnings)
    {
        //return true;
        var totalCost = 0;
        foreach (var spawning in spawnings)
        {
            totalCost += spawning;
            if (spawning > 5)
                return false;
        }

        //exactly the weight
        if (totalCost == weight)
            return true;


        return false;
    }

    private void DisplaySpawning(Spawning toDisplay)
    {
        //Debug.Log(toDisplay.placement[0] + " " + toDisplay.placement[1] + " " + toDisplay.placement[2] + " " +
        //          toDisplay.placement[3]);
    }

    private string MakeString(Spawning toDisplay)
    {
        return "[" + toDisplay.placement[0] + ", " + toDisplay.placement[1] + ", " + toDisplay.placement[2] + ", " +
               toDisplay.placement[3] + "]";
    }

    private void Backtracking(int k, ref Spawning spawning)
    {
        //max unit is 5
        for (var i = 0; i <= 5; i++)
        {
            spawning.placement[k] = i;
            //no more space
            if (k == emptySlots)
            {
                if (IsValid(spawning.placement))
                {
                    var newSpawning = spawning.placement;

                    validSpawns.Add(new Spawning(newSpawning[0], newSpawning[1], newSpawning[2], newSpawning[3]));
                    spawningsCount++;

                    DisplaySpawning(spawning);
                }
            }
            else
            {
                Backtracking(k + 1, ref spawning);
            }
        }
    }

    private bool RandomSpawn(bool isRed)
    {
        weight = RedBlueTurn.maxPoints;
        // between 1 and 4
        var countEnemies = Random.Range(1 + weight / 6, Mathf.Min(weight, 5));
        positions = new List<UnitRenderer>(board.GetEmptySquares(isRed ? SquareType.BLUE : SquareType.RED));
        countEnemies = Mathf.Min(countEnemies, positions.Count);
        GetListOfPositions(ref positions, countEnemies);
        //Debug.Log(positions);

        var fogOfWar = isRed ? fogOfWarBlue : fogOfWarRed;
        foreach (var unitRenderer in positions) unitRenderer.SetUnitSettings(fogOfWar);


        weight = Mathf.Min(weight, positions.Count * 5);
        //no positions
        if (positions.Count == 0)
        {
            onEnemyEndTurn?.Invoke();
            return true;
        }

        enemyList = RedBlueTurn.IsPlayerFirst() ? blueUnits : redUnits;
        //choose some random enemies
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

        //we still have some weight with a full board
        if (weight > 0)
        {
            enemyIndicies = indexEnemy;
            enemyIndicies = enemyIndicies.Where(en => en < 5).ToList();
            //increase cost of the units until no more weight
            while (weight > 0)
            {
                var randomIndex = Random.Range(0, enemyIndicies.Count);
                if (enemyIndicies[randomIndex] + 1 > 5)
                    continue;
                enemyIndicies[randomIndex]++;
                //Debug.Log(enemyIndicies);
                weight--;
                //Debug.Log("Remaining weight:" + weight);
            }
        }

        return false;
    }

    private void ChooseSpawning(int index)
    {
        //clear previous placement
        var emptyInfo = new UnitBoardInfo();
        foreach (var position in positions)
        {
            position.SetUnitSettings(emptyInfo);
        }

        var placement = validSpawns[index];
        DisplaySpawning(placement);


        for (int i = 0; i < positions.Count; i++)
        {
            int placementIndex = placement.placement[i];
            if (placementIndex > 0)
            {
                SetUnitRenderer(positions[i], enemyList[placementIndex - 1]);
            }
        }
    }

    public void ChooseSpawningRate(float index)
    {
        //clear previous placement
        for (var i = 0; i < positions.Count; i++) positions[i].SetUnitSettings(new UnitBoardInfo());

        var placement = validSpawns[int.Parse(index.ToString())];
        DisplaySpawning(placement);

        var indexEnemy = 0;
        foreach (var unitSlot in positions)
        {
            if (placement.placement[indexEnemy] > 0)
                SetUnitRenderer(unitSlot, enemyList[placement.placement[indexEnemy] - 1]);

            indexEnemy++;
        }

        onRate?.Invoke(spawnsScore[int.Parse(index.ToString())].Item2.ToString());
    }

    public void PlaceEnemies()
    {
        if (enemyIndicies.Count > 0)
            for (var i = 0; i < indexEnemy.Count; i++)
                if (unitRenderers[i].GetUnitSettings().unitSettings.cost < 5)
                    SetUnitRenderer(unitRenderers[i], enemyList[enemyIndicies[i]]);
                else
                    //just spawn the high cost one
                    SetUnitRenderer(unitRenderers[i], enemyList[enemyList.Count - 1]);
        else
            for (var i = 0; i < unitRenderers.Count; i++)
                SetUnitRenderer(unitRenderers[i], enemyList[indexEnemy[i]]);

        onEnemyEndTurn?.Invoke();
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

    public StringEvent onRate;
    private int score = 0;

    public void RateBoard()
    {
        //gets the bot point of view
        isRed = RedBlueTurn.IsPlayerFirst();
        var aiSettings = isRed ? blueAI : redAI;
        var pointSystem = aiSettings.pointSystem;
        var currentBoard = board.pieces;

        var red = Board.GetAllPieces(SquareType.RED, ref currentBoard);
        var blue = Board.GetAllPieces(SquareType.BLUE, ref currentBoard);
        var redOverLine = UpdateWinCounts.winCounts.x;
        var blueOverLine = UpdateWinCounts.winCounts.y;

        //Debug.Log($"Red units: {red.Count}, Blue units: {blue.Count}");
        //Debug.Log($"Red units over line: {redOverLine}, Blue units over line: {blueOverLine}");

        score = 0;
        //Debug.Log($"AI is Red: {!isRed} ");

        if (!isRed)
        {
            var redScore = red.Count * pointSystem.alliedUnits;
            var blueScore = blue.Count * pointSystem.enemyUnits;

            var redOverLineScore = redOverLine * pointSystem.alliedUnitsOverLine;
            var blueOverLineScore = blueOverLine * pointSystem.enemyUnitsOverLine;

            score += redScore;
            score += blueScore;
            score += redOverLineScore;
            score += blueOverLineScore;

            //Debug.Log($"Score from red units: {redScore}");
            //Debug.Log($"Score from blue units: {blueScore}");
            //Debug.Log($"Score from red units over line: {redOverLineScore}");
            //Debug.Log($"Score from blue units over line: {blueOverLineScore}");

            // Assuming there's a win condition check that can be represented by a boolean
            var redWins = UpdateWinCounts.redBlueWinMaxCounts.x <= redOverLine;
            var blueWins = UpdateWinCounts.redBlueWinMaxCounts.y <= blueOverLine;


            if (redWins)
            {
                score += pointSystem.alliedWin;
                //Debug.Log($"Red wins! Adding {pointSystem.alliedWin} to score.");
            }

            if (blueWins)
            {
                score += pointSystem.enemyWin;
                //Debug.Log($"Blue wins! Subtracting {pointSystem.enemyWin} from score.");
            }

            //Debug.Log($"Final Board Score: {score}");
        }
        else
        {
            var blueScore = blue.Count * pointSystem.alliedUnits; // Now blue units are allied
            var redScore = red.Count * pointSystem.enemyUnits; // Red units are enemies

            var blueOverLineScore = blueOverLine * pointSystem.alliedUnitsOverLine; // Blue units over line
            var redOverLineScore = redOverLine * pointSystem.enemyUnitsOverLine; // Red units over line

            score += blueScore;
            score += redScore;
            score += blueOverLineScore;
            score += redOverLineScore;

            //Debug.Log($"Score from blue units: {blueScore}");
            //Debug.Log($"Score from red units: {redScore}");
            //Debug.Log($"Score from blue units over line: {blueOverLineScore}");
            //Debug.Log($"Score from red units over line: {redOverLineScore}");

            // Assuming there's a win condition check that can be represented by a boolean
            var blueWins = UpdateWinCounts.redBlueWinMaxCounts.y <= blueOverLine; // Blue win condition
            var redWins = UpdateWinCounts.redBlueWinMaxCounts.x <= redOverLine; // Red win condition

            if (blueWins)
            {
                score += pointSystem.alliedWin; // Blue wins
                //Debug.Log($"Blue wins! Adding {pointSystem.alliedWin} to score.");
            }

            if (redWins)
            {
                score += pointSystem.enemyWin; // Red wins
                //Debug.Log($"Red wins! Subtracting {pointSystem.enemyWin} from score.");
            }

            //Debug.Log($"Final Board Score: {score}");
        }

        onRate?.Invoke(score.ToString());
    }
}