using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(ZombieBase))]
public class CustomEditors : Editor
{
#if UNITY_EDITOR
    public void OnSceneGUI() // Editor Draw
    {
        ZombieBase fow = (ZombieBase)target; // Draw Target
        Handles.color = Color.white; // White Color Circle
        Handles.DrawWireArc(fow.transform.position, Vector3.up, Vector3.forward, 360, fow.ViewRadius); // Draw Circle
        Vector3 viewAngleA = fow.DirFromAngle(-fow.ViewAngle / 2, false); // Left Angle
        Vector3 viewAngleB = fow.DirFromAngle(fow.ViewAngle / 2, false); // Right Angle

        Handles.DrawLine(fow.transform.position, fow.transform.position + viewAngleA * fow.ViewRadius); // Draw Line
        Handles.DrawLine(fow.transform.position, fow.transform.position + viewAngleB * fow.ViewRadius);

        Handles.color = Color.red; // Set red
        if(fow.VisibleTarget != null) // When Player detected,
        {
            Handles.DrawLine(fow.transform.position, fow.VisibleTarget.transform.position); // Draw red line
        }
    }
#endif
}
