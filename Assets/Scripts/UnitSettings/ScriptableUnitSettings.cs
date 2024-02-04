using UnityEngine;

[CreateAssetMenu(fileName = "UnitSpriteSettings", menuName = "UnitSettings")]
public class ScriptableUnitSettings : ScriptableObject
{
    public Sprite sprite;
    public Color color = Color.red;
    public bool flipY = true;
    public int cost = 1;
}