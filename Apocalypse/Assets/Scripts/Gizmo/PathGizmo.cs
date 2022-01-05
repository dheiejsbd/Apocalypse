using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathGizmo : MonoBehaviour
{
    public void OnDrawGizmos()
    {
        Transform[] path = transform.GetComponentsInChildren<Transform>();
        Gizmos.color = Color.yellow;
        for (int i = 1; i < path.Length-1; i++)
        {
            Gizmos.DrawSphere(path[i].position, 1);
            Gizmos.DrawLine(path[i].position, path[i + 1].position);
        }
        Gizmos.DrawSphere(path[path.Length - 1].position, 1);
        Gizmos.DrawLine(path[1].position, path[path.Length-1].position);
    }
}
