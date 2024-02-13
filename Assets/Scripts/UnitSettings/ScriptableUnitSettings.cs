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
    public bool boost => sharedValues.boost;
    public Vector2Int[] movePositions => sharedValues.movePositions;
    public Vector2Int[] attackPositions => sharedValues.atackPositions;
}