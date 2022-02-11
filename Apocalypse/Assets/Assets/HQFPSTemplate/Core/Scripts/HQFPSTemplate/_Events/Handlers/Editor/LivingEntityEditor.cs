using System;
using System.Reflection;
using UnityEngine;
using UnityEditor;

namespace HQFPSTemplate
{
    [CustomEditor(typeof(Entity), true)]
    public class LivingEntityEditor : Editor
    {
        public override bool RequiresConstantRepaint()
        {
            if (Application.isPlaying)
                return true;
            else
                return false;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space();
            EditorGUICustom.Separator();

            Type eventHandlerType = target.GetType();

            var fields = eventHandlerType.GetFields(BindingFlags.Public | BindingFlags.Instance);

            EditorGUILayout.LabelField("Values: ", EditorStyles.boldLabel);
            GUI.enabled = false;

            foreach (var field in fields)
            {
                Type fieldType = field.FieldType;

                if (fieldType.IsGenericType && fieldType.GetGenericTypeDefinition() == typeof(Value<>))
                {
                    object valueObj = field.GetValue(target);
                    var currentValueField = fieldType.GetField("m_CurrentValue", BindingFlags.NonPublic | BindingFlags.Instance);

                    object currentValue = currentValueField.GetValue(valueObj);

                    EditorGUILayout.LabelField(field.Name.DoUnityLikeNameFormat() + ": " + currentValue);
                }
            }

            GUI.enabled = true;

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Activities: ", EditorStyles.boldLabel);
            GUI.enabled = false;

            foreach (var field in fields)
            {
                Type fieldType = field.FieldType;

                if ((fieldType.IsGenericType && fieldType.GetGenericTypeDefinition() == typeof(Activity<>)) || fieldType == typeof(Activity))
                {
                    object activityObj = field.GetValue(target);

                    var activeField = fieldType.GetField("m_Active", BindingFlags.NonPublic | BindingFlags.Instance);
                    object activeValue = activeField.GetValue(activityObj);

                    EditorGUILayout.LabelField(field.Name.DoUnityLikeNameFormat() + (((bool)activeValue) ? " (Active)" : " (Inactive)"));
                }
            }

            GUI.enabled = true;
        }
    }
}