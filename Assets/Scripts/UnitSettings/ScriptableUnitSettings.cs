using UnityEngine;

[CreateAssetMenu(fileName = "UnitSpriteSettings", menuName = "ScriptableObjects/UnitSettings", order = 0)]
public class ScriptableUnitSettings : ScriptableObject
{
    public Sprite sprite;

    [Tooltip("The unit will try to move to the specified positions in order, they are relative to the unit positions")]
    public Vector2Int[] movePositions;

    public Vector2Int[] attackPositions;

    public Vector2Int[] boostPositions;
    public int hitsToDiePerTurn = 1;
    public bool boost = false;
    public int cost;
}