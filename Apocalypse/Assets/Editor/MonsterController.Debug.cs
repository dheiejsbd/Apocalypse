using UnityEditor;
using UnityEngine;

namespace Apocalypse
{
    [CustomEditor(typeof(MonsterController))]
    public class MonsterFovEditor : Editor
    {
        private void OnSceneGUI()
        {
            MonsterController monster = target as MonsterController;
            Handles.DrawWireDisc(monster.owner.transform.position, monster.owner.transform.up, monster.data.Dist);
            Handles.color = Color.red;
            DrawFOV(monster.owner.transform.position, -monster.owner.transform.eulerAngles.y, monster.data.Angle, monster.data.Dist);
            Handles.DrawWireArc(monster.owner.transform.position, monster.owner.transform.up, monster.transform.forward, monster.data.Angle/2, monster.data.Dist);
            Handles.DrawWireArc(monster.owner.transform.position, monster.owner.transform.up, monster.transform.forward, -monster.data.Angle/2, monster.data.Dist);
        }

        void DrawFOV(Vector3 origin, float OriginAngle, float Angle, float Dist)
        {
            float X, Z;

            X = Dist * Mathf.Cos((Angle / 2 + 90 + OriginAngle) * Mathf.PI / 180) + origin.x;
            Z = Dist * Mathf.Sin((Angle / 2 + 90 + OriginAngle) * Mathf.PI / 180) + origin.z;
            
            Handles.DrawLine(origin, new Vector3(X, origin.y, Z));

            X = Dist * Mathf.Cos((-Angle / 2 + 90 + OriginAngle) * Mathf.PI / 180) + origin.x;
            Z = Dist * Mathf.Sin((-Angle / 2 + 90 + OriginAngle) * Mathf.PI / 180) + origin.z;
            Handles.DrawLine(origin, new Vector3(X, origin.y, Z));
        }
    }
}