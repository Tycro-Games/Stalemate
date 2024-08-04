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


    public AIPointSystem(int defaultAlliedUnits , int defaultEnemyUnits ,
        int defaultAlliedUnitsOverLine, int defaultEnemyUnitsOverLine ,
        int defaultAlliedWin, int defaultEnemyWin)
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

}