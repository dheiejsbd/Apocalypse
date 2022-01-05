using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Apocalypse;
[CustomEditor(typeof(FOVEvent))]
public class FOVEditor : Editor
{
    FOVEvent FOV;
    Material mat;
    void OnEnable()
    {
        FOV = target as FOVEvent;
        var shader = Shader.Find("Hidden/Internal-Colored");
        mat = new Material(shader);
    }

    public override void OnInspectorGUI()
    {

        base.OnInspectorGUI();
        Rect rect = GUILayoutUtility.GetRect(10, 1000, 200, 200);
        if (Event.current.type == EventType.Repaint)
        {
            GUI.BeginClip(rect);
            GL.Clear(true, false, Color.black);
            mat.SetPass(0);

            DrawCircle();
            DrawFOV();
            DrawArc();

            EditorGUI.LabelField(new Rect(rect.size/2 + Vector2.up * -35 + Vector2.left * 10, new Vector2(50,30)), FOV.Angle.ToString() + "¡Æ");

            GUI.EndClip();
        }

        void DrawCircle()
        {

            GL.Begin(GL.LINE_STRIP);
            GL.Color(Color.green);

            Vector2 midPoint = rect.size / 2;
            float X, Y;
            for (int i = 0; i <= 360; i += 10)
            {
                X = 100 * Mathf.Cos(i * Mathf.PI / 180);
                Y = 100 * Mathf.Sin(i * Mathf.PI / 180);
                GL.Vertex(new Vector2(X, Y) + midPoint);
            }
            GL.End();
        }
        void DrawFOV()
        {
            Vector2 midPoint = rect.size / 2;
            float X, Y;

            GL.Begin(GL.LINE_STRIP);
            X = 100 * Mathf.Cos((FOV.Angle / 2 - 90) * Mathf.PI / 180) + midPoint.x;
            Y = 100 * Mathf.Sin((FOV.Angle / 2 - 90) * Mathf.PI / 180) + midPoint.y;
            GL.Vertex3(X, Y, 0);

            GL.Vertex(midPoint);
            X = 100 * Mathf.Cos((-FOV.Angle / 2 - 90) * Mathf.PI / 180) + midPoint.x;
            Y = 100 * Mathf.Sin((-FOV.Angle / 2 - 90) * Mathf.PI / 180) + midPoint.y;
            GL.Vertex3(X, Y, 0);

            GL.End();
        }

        void DrawArc()
        {
            GL.Begin(GL.LINE_STRIP);
            float size = 10;
            GL.Color(Color.white);

            Vector2 midPoint = rect.size / 2;
            float X,Y;

            X = size * Mathf.Cos((FOV.Angle/2-90) * Mathf.PI / 180);
            Y = size * Mathf.Sin((FOV.Angle/2-90) * Mathf.PI / 180);
            GL.Vertex(new Vector2(X, Y) + midPoint);

            for (int i = (int)(FOV.Angle/ 2 - FOV.Angle % 10) - 90; i > -90; i -= 10)
            {
                X = size * Mathf.Cos(i * Mathf.PI / 180);
                Y = size * Mathf.Sin(i * Mathf.PI / 180);
                GL.Vertex(new Vector2(X, Y) + midPoint);
            }

            X = size * Mathf.Cos(-90 * Mathf.PI / 180);
            Y = size * Mathf.Sin(-90 * Mathf.PI / 180);
            GL.Vertex(new Vector2(X, Y) + midPoint);


            for (int i = -90; -(int)(FOV.Angle / 2 - FOV.Angle % 10) - 90 < i; i -= 10)
            {
                X = size * Mathf.Cos(i * Mathf.PI / 180);
                Y = size * Mathf.Sin(i * Mathf.PI / 180);
                GL.Vertex(new Vector2(X, Y) + midPoint);
            }

            X = size * Mathf.Cos((-FOV.Angle / 2 - 90) * Mathf.PI / 180);
            Y = size * Mathf.Sin((-FOV.Angle / 2 - 90) * Mathf.PI / 180);
            GL.Vertex(new Vector2(X, Y) + midPoint);

            GL.End();
        }
    }
}
