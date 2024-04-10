using UnityEngine;

[CreateAssetMenu(fileName = "UnitSpriteSettings", menuName = "ScriptableObjects/UnitSettings", order = 0)]
public class ScriptableUnitSettings : ScriptableObject
{
    public Sprite sprite;

    public bool isRed = true;

    public UnitSharedValues sharedValues;

    //**to delete
    public Color color = Color.red;

    public bool flipY = true;

    //**
    public int cost => sharedValues.cost;
    public int hp => sharedValues.hitsToDiePerTurn;
    public bool boost => sharedValues.boost;
    public Vector2Int[] movePositions => sharedValues.movePositions;
    public Vector2Int[] attackPositions => sharedValues.atackPositions;
    public Vector2Int[] boostPositions => sharedValues.boostPositions;
}