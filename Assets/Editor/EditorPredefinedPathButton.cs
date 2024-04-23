#if UNITY_EDITOR
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using Utility;

[CustomEditor(typeof(PredefinedPathMover))]
[InitializeOnLoad]
public class EditorPredefinedPathButton : Editor
{
    private PredefinedPathMover path;


    private const float segmentSelectDistanceThreshold = .1f;
    private int selectedSegmentIndex = -1;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        path = (PredefinedPathMover)target;

        // Add a button to the Inspector
        if (GUILayout.Button("Randomize Deviation Axis")) path.RandomizeDirection();
        if (GUILayout.Button("GetWayPoints")) path.GetWayPoints();
    }

    private void OnSceneGUI()
    {
        Input();
        Draw();
    }

    private void Input()
    {
        var guiEvent = Event.current;
        var mousePos = HandleUtility.GUIPointToWorldRay(guiEvent.mousePosition).origin;

        if (guiEvent.type == EventType.MouseMove)
        {
            var minDstToSegment = segmentSelectDistanceThreshold;
            var newSelectedSegmentIndex = -1;

            for (var i = 0; i < path.waypoints.Count / 3; i++)
            {
                var points = path.GetPointsInSegment(i);
                var dst = HandleUtility.DistancePointBezier(mousePos, points[0], points[3], points[1], points[2]);
                if (dst < minDstToSegment)
                {
                    minDstToSegment = dst;
                    newSelectedSegmentIndex = i;
                }
            }

            if (newSelectedSegmentIndex != selectedSegmentIndex)
            {
                selectedSegmentIndex = newSelectedSegmentIndex;
                HandleUtility.Repaint();
            }
        }

        HandleUtility.AddDefaultControl(0);
    }

    private void Draw()
    {
        if (path == null) path = (PredefinedPathMover)target;

        if (path != null)
            for (var i = 0; i < path.waypoints.Count; i++)
            {
                Handles.color = path.anchorCol;
                var handleSize = path.anchorDiameter;
                var newPos = Handles.FreeMoveHandle(path.waypoints[i].position, handleSize, Vector3.zero,
                    Handles.CylinderHandleCap);
                if (path.waypoints[i].position != newPos) Undo.RecordObject(path, "Move point");
                path.MovePoint(i, newPos);
            }
    }

    static EditorPredefinedPathButton()
    {
        // Subscribe to the EditorApplication.update event
        EditorApplication.update += UpdateGizmos;
    }

    private static void UpdateGizmos()
    {
        // Force a repaint of the Scene view to continuously update gizmos
        SceneView.RepaintAll();
    }
}
#endif