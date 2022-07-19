using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Line))]
public class LineInspector : Editor
{
    // Start is called before the first frame update
    private void OnSceneGUI()
    {
        Line line = target as Line;

        Transform handleTransform = line.transform;
        Vector3 p0 = handleTransform.TransformPoint(line.p0);
        Vector3 p1 = handleTransform.TransformPoint(line.p1);

        Handles.color = Color.white;
        Handles.DrawLine(p0, p1);
    }
}
