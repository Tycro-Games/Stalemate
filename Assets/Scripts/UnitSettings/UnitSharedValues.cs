using UnityEngine;

[CreateAssetMenu(fileName = "UnitSharedValues", menuName = "ScriptableObjects/UnitStats", order = 1)]
public class UnitSharedValues : ScriptableObject
{
    [Tooltip("The unit will try to move to the specified positions in order, they are relative to the unit positions")]
    public Vector2Int[] movePositions;

    public Vector2Int[] atackPositions;

    public Vector2Int[] boostPositions;
    public int hitsToDiePerTurn = 1;
    public bool boost = false;
    public int cost;
}