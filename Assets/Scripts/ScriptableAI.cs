using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
[CustomEditor(typeof(ScriptableAI))]
public class ScriptableAIEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var settings = (ScriptableAI)target;
        if (settings.random)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("random"));
            serializedObject.ApplyModifiedProperties();
            return;
        }

        // Draw default properties
        DrawDefaultInspector();

        // Check if boost is true
    }
}
#endif
[System.Serializable]
public struct AIPointSystem
{
    [Range(-100, 100)] public int alliedUnits;
    [Range(-100, 100)] public int enemyUnits;

    [Range(-100, 100)] public int alliedUnitsOverLine;
    [Range(-100, 100)] public int enemyUnitsOverLine;
    [Range(-100, 100)] public int alliedWin;
    [Range(-100, 100)] public int enemyWin;


    public AIPointSystem(int defaultAlliedUnits = 1, int defaultEnemyUnits = -1,
        int defaultAlliedUnitsOverLine = 5, int defaultEnemyUnitsOverLine = -5,
        int defaultAlliedWin = 100, int defaultEnemyWin = -100)
    {
        alliedUnits = defaultAlliedUnits;
        enemyUnits = defaultEnemyUnits;
        alliedUnitsOverLine = defaultAlliedUnitsOverLine;
        enemyUnitsOverLine = defaultEnemyUnitsOverLine;
        alliedWin = defaultAlliedWin;
        enemyWin = defaultEnemyWin;
    }
}

[CreateAssetMenu(fileName = "AISettings", menuName = "ScriptableObjects/AISettings", order = 1)]
public class ScriptableAI : ScriptableObject
{
    public bool random = true;

    public AIPointSystem pointSystem = new();

    //public int val;
    private void OnEnable()
    {
        // Initialize AIPointSystem with default values if it's the default instance
        if (pointSystem.Equals(default(AIPointSystem)))
            pointSystem = new AIPointSystem
            {
                alliedUnits = 1,
                enemyUnits = -1,
                alliedUnitsOverLine = 5,
                enemyUnitsOverLine = -5,
                alliedWin = 100,
                enemyWin = -100
            };
    }
}