using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(ZombieBase))]
public class CustomEditors : Editor
{
    public void OnSceneGUI()
    {
        ZombieBase fow = (ZombieBase)target;
        Handles.color = Color.white;
        Handles.DrawWireArc(fow.transform.position, Vector3.up, Vector3.forward, 360, fow.ViewRadius);
        Vector3 viewAngleA = fow.DirFromAngle(-fow.ViewAngle / 2, false);
        Vector3 viewAngleB = fow.DirFromAngle(fow.ViewAngle / 2, false);

        Handles.DrawLine(fow.transform.position, fow.transform.position + viewAngleA * fow.ViewRadius);
        Handles.DrawLine(fow.transform.position, fow.transform.position + viewAngleB * fow.ViewRadius);

        Handles.color = Color.red;
        if(fow.VisibleTargets != null)
        {
            Handles.DrawLine(fow.transform.position, fow.VisibleTargets.position);
        }

    }


}
