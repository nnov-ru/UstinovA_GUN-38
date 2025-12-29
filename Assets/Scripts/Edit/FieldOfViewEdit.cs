using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.GraphView;

[CustomEditor(typeof(FieldOfView))]
public class FieldOfViewEdit : Editor
{
    private void OnSceneGUI()
    {
        FieldOfView fov = (FieldOfView)target;

        Handles.color = Color.white;
        Handles.DrawWireArc(fov.transform.position, Vector3.forward, Vector3.right, 360, fov.Radius);

        Vector3 viewAngleLeft = DirectionFromAngle(fov.transform.eulerAngles.z, -fov.Angle / 2);
        Vector3 viewAngleRight = DirectionFromAngle(fov.transform.eulerAngles.z, fov.Angle / 2);
        
        Handles.color = Color.yellow;
        Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngleLeft * fov.Radius);
        Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngleRight * fov.Radius);
        
        if (fov.CanSeePlayer && fov.Player != null)
        {
            Handles.color = Color.green;
            Handles.DrawLine(fov.transform.position, fov.Player.transform.position);
        }
    }
    private Vector3 DirectionFromAngle(float eulerZ, float angleInDegrees)
    {
        angleInDegrees += eulerZ;
        return new Vector3(
            Mathf.Cos(angleInDegrees * Mathf.Deg2Rad), 
            Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 
            0
            );
    }
}
