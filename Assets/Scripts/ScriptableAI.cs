
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
[CreateAssetMenu(fileName = "AISettings", menuName = "ScriptableObjects/AISettings", order = 1)]
public class ScriptableAI : ScriptableObject
{
    public bool random = true;
    public int val;
}
