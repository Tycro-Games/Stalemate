using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
[CustomEditor(typeof(ScriptableUnitSettings))]
public class ScriptableUnitSettingsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var settings = (ScriptableUnitSettings)target;

        // Draw default properties
        DrawDefaultInspector();

        // Check if boost is true
        if (settings.boost)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("boostPositions"));
            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif
[CreateAssetMenu(fileName = "UnitSpriteSettings", menuName = "ScriptableObjects/UnitSettings", order = 0)]
public class ScriptableUnitSettings : ScriptableObject
{
    public string title = "title";
    public string description = "description";

    public Sprite sprite;

    [Tooltip("The unit will try to move to the specified positions in order, they are relative to the unit positions")]
    public Vector2Int[] movePositions;

    public Vector2Int[] attackPositions;

    public int cost;

    public int hitsToDiePerTurn = 1;
    public bool boost = false;
    public bool flip = false;
    [HideInInspector] public Vector2Int[] boostPositions;
}