using UnityEditor;
using UnityEngine;

namespace Dynamics.Editor
{
    [CustomPropertyDrawer(typeof(SODValue<,>), true)]
    [CustomPropertyDrawer(typeof(SODFloat))]
    [CustomPropertyDrawer(typeof(SODAngle))]
    [CustomPropertyDrawer(typeof(SODFloat2))]
    [CustomPropertyDrawer(typeof(SODFloat3))]
    [CustomPropertyDrawer(typeof(SODFloat4))]
    [CustomPropertyDrawer(typeof(SODQuaternion))]
    public class SODValueDrawer : PropertyDrawer
    {
        static float LineHeight => EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return !property.isExpanded ? EditorGUIUtility.singleLineHeight : LineHeight * 4;
        }

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(rect, label, property);

            var labelRect = new Rect(rect.x, rect.y, EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight);

            property.isExpanded = EditorGUI.Foldout(labelRect, property.isExpanded, label, true);

            var graphRect = new Rect(rect.x + EditorGUIUtility.labelWidth + 2f, rect.y, rect.width - EditorGUIUtility.labelWidth - 2f, EditorGUIUtility.singleLineHeight);

            var spCurve = property.FindPropertyRelative("curve");
            var spState = property.FindPropertyRelative("state");

            SODCurveDrawer.DrawProperty(graphRect, spCurve);

            if (property.isExpanded)
            {                
                using (new SODEditorUtils.LabelWidthScope(EditorGUIUtility.labelWidth - 15f))
                {
                    var spPreviousValue = spState.FindPropertyRelative("previousValue");
                    var spVelocity = spState.FindPropertyRelative("velocity");
                    var spTarget = spState.FindPropertyRelative("target");

                    var propertyRect = new Rect(rect.x + 15f, rect.y, rect.width - 15f, EditorGUIUtility.singleLineHeight);

                    propertyRect.y += LineHeight;
                    EditorGUI.PropertyField(propertyRect, spPreviousValue, new GUIContent("Value"));

                    propertyRect.y += LineHeight;
                    EditorGUI.PropertyField(propertyRect, spTarget);

                    propertyRect.y += LineHeight;
                    EditorGUI.PropertyField(propertyRect, spVelocity);
                }
            }

            spState.FindPropertyRelative("kValues.x").floatValue = spCurve.FindPropertyRelative("kValues.x").floatValue;
            spState.FindPropertyRelative("kValues.y").floatValue = spCurve.FindPropertyRelative("kValues.y").floatValue;
            spState.FindPropertyRelative("kValues.z").floatValue = spCurve.FindPropertyRelative("kValues.z").floatValue;

            EditorGUI.EndProperty();
        }
    }
}