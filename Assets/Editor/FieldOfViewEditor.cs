using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Artiface), true)]
public class FieldOfViewEditor : Editor
{
    public void OnSceneGUI()
    {
        Artiface fov = (Artiface)target;

        // FOV arc
        Handles.color = Color.white;
        Handles.DrawWireArc(fov.transform.position, Vector3.up, Vector3.forward, 360, fov.radius);

        Vector3 viewAngle01 = DirectionFromAngle(fov.transform.eulerAngles.y, -fov.angle / 2);
        Vector3 viewAngle02 = DirectionFromAngle(fov.transform.eulerAngles.y, fov.angle / 2);

        Handles.color = Color.yellow;
        Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngle01 * fov.radius);
        Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngle02 * fov.radius);

        // Sight line to current visual target only
        if (fov.canSeePlayer && fov.currentVisualTarget != null)
        {
            Handles.color = Color.green;
            Handles.DrawLine(fov.transform.position, fov.currentVisualTarget.transform.position);
        }

        // Sight line to current audio target
        if (fov.canHearPlayer && fov.currentAudioTarget != null)
        {
            Handles.color = Color.yellow;
            Handles.DrawLine(fov.transform.position, fov.currentAudioTarget.transform.position);
        }
    }

    public Vector3 DirectionFromAngle(float eulerY, float angleInDegrees)
    {
        angleInDegrees += eulerY;
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}