using UnityEditor;
using UnityEngine;

namespace Dynamics.Editor
{
    public class SODCurveEditor : EditorWindow
    {
        static readonly Color presetBackgroundColor = new(0.16f, 0.16f, 0.16f, 1f);
        static readonly Color presetEdgeColor = new(0.06f, 0.06f, 0.06f, 1f);

        static GUIStyle editableLabelStyle;

        static readonly SODCurve[] presets = new SODCurve[]
        {
            SODCurve.Default,
            SODCurve.Damped,
            SODCurve.Elastic,
            SODCurve.Undershoot,
        };

        SerializedProperty property;

        SerializedProperty fProperty;
        SerializedProperty zProperty;
        SerializedProperty rProperty;

        SerializedProperty kXProperty;
        SerializedProperty kYProperty;
        SerializedProperty kZProperty;

        float fMin;
        float fMax;
        float zMin;
        float zMax;
        float rMin;
        float rMax;

        public SODCurve CurrentCurveValue => new
        (
            fProperty.floatValue,
            zProperty.floatValue,
            rProperty.floatValue
        );

        private void OnEnable()
        {
            editableLabelStyle ??= new GUIStyle(EditorStyles.label) { alignment = TextAnchor.MiddleRight };

            Undo.undoRedoPerformed += Repaint;
        }

        private void OnDisable()
        {
            Undo.undoRedoPerformed -= Repaint;
        }

        private void UpdateWindowSize()
        {
            minSize = new Vector2(340f, 288f);
            maxSize = new Vector2(3400f, minSize.y);
        }

        public static void Open(Rect rect, SerializedProperty property)
        {
            var window = CreateInstance<SODCurveEditor>();

            window.titleContent = new GUIContent("Second Order Dynamics Curve Editor");
            window.UpdateWindowSize();
            window.position = GUIUtility.GUIToScreenRect(rect);

            window.property = property;

            window.fProperty = property.FindPropertyRelative("frequency");
            window.zProperty = property.FindPropertyRelative("damping");
            window.rProperty = property.FindPropertyRelative("response");

            var spKValues = property.FindPropertyRelative("kValues");

            window.kXProperty = spKValues.FindPropertyRelative("x");
            window.kYProperty = spKValues.FindPropertyRelative("y");
            window.kZProperty = spKValues.FindPropertyRelative("z");

            window.fMin = Mathf.Min(window.fProperty.floatValue, 0f);
            window.fMax = Mathf.Max(window.fProperty.floatValue, 10f);

            window.zMin = Mathf.Min(window.zProperty.floatValue, 0f);
            window.zMax = Mathf.Max(window.zProperty.floatValue, 4f);

            window.rMin = Mathf.Min(window.rProperty.floatValue, -5f);
            window.rMax = Mathf.Max(window.rProperty.floatValue, 5f);

            window.ShowAuxWindow();
        }

        void OnGUI()
        {
            if (!IsValid())
            {
                Close();
                return;
            }

            var graphRect = GUILayoutUtility.GetRect(100f, 150f, GUILayout.ExpandWidth(true));

            DrawGraph(graphRect);

            EditorGUILayout.Space(10f);

            using (new EditorGUILayout.VerticalScope(GUILayout.Width(position.width - 4f)))
            using (new SODEditorUtils.LabelWidthScope(90f))
            using (var propertyScope = new EditorGUI.ChangeCheckScope())
            {
                EditableSliderField(fProperty, ref fMin, ref fMax);
                EditableSliderField(zProperty, ref zMin, ref zMax);
                EditableSliderField(rProperty, ref rMin, ref rMax);

                if (propertyScope.changed) UpdatePropertyValues();
            }

            EditorGUILayout.Space(10f);

            var presetsRect = GUILayoutUtility.GetRect(200f, 58f, GUILayout.ExpandWidth(true));
            var presetLabelRect = new Rect(presetsRect.position, new Vector2(presetsRect.width, 18f));

            EditorGUI.DrawRect(presetsRect, presetBackgroundColor);
            EditorGUI.DrawRect(new Rect(presetsRect.position, new Vector2(presetsRect.width, 1f)), presetEdgeColor);

            EditorGUI.LabelField(presetLabelRect, "Presets", EditorStyles.centeredGreyMiniLabel);

            for (int i = 0; i < presets.Length; i++)
            {
                var preset = presets[i];

                var presetRect = new Rect(presetsRect.position + new Vector2(8f + 68f * i, 20f), new Vector2(60f, 32f));

                SODEditorUtils.DrawGraph(presetRect, preset, Color.green, true);

                if ((Event.current.button == 0) && (Event.current.type == EventType.MouseDown) && presetRect.Contains(Event.current.mousePosition))
                {
                    Undo.RecordObject(property.serializedObject.targetObject, "Apply Second Order Curve Preset");

                    fProperty.floatValue = preset.Frequency;
                    zProperty.floatValue = preset.Damping;
                    rProperty.floatValue = preset.Response;

                    UpdatePropertyValues();

                    GUI.changed = true;
                }
            }
        }

        private void UpdatePropertyValues()
        {
            var curve = property.GetSODCurveValue();

            fProperty.floatValue = curve.Frequency;
            zProperty.floatValue = curve.Damping;
            rProperty.floatValue = curve.Response;

            kXProperty.floatValue = curve.KValues.x;
            kYProperty.floatValue = curve.KValues.y;
            kZProperty.floatValue = curve.KValues.z;

            property.serializedObject.ApplyModifiedProperties();

            EditorUtility.SetDirty(property.serializedObject.targetObject);
        }

        bool IsValid()
        {
            try
            {
                if (property == null || property.serializedObject == null) return false;
                if (!property.serializedObject.targetObject) return false;
            }
            catch
            {
                return false;
            }

            return true;
        }

        void DrawGraph(Rect rect)
        {
            var innerRect = new Rect(rect.position + new Vector2(16f, 8f), rect.size + new Vector2(-22f, -30f));

            SODEditorUtils.DrawGraph(innerRect, CurrentCurveValue, Color.green, false);

            EditorGUI.LabelField(new Rect(innerRect.position + new Vector2(0f, innerRect.height + 2f), new Vector2(20f, 16f)), "0s");
            EditorGUI.LabelField(new Rect(innerRect.position + new Vector2(innerRect.width - 8f, innerRect.height + 2f), new Vector2(8f, 16f)), $"s");

            using (var propertyScope = new EditorGUI.ChangeCheckScope())
            using (new SODEditorUtils.LabelWidthScope(32f))
            {
                var previewTimeRect = new Rect(innerRect.position + new Vector2(innerRect.width - 77f, innerRect.height + 2f), new Vector2(70f, 16f));
                var newPreviewTime = EditorGUI.FloatField(previewTimeRect, "Time:", SODEditorUtils.GraphPreviewTime, editableLabelStyle);
                if (propertyScope.changed) SODEditorUtils.GraphPreviewTime = Mathf.Clamp(newPreviewTime, 0.1f, 10f);
            }
        }

        public void EditableSliderField(SerializedProperty property, ref float leftValue, ref float rightValue)
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.PrefixLabel(new GUIContent(property.displayName, property.tooltip));

                var leftValueRect = GUILayoutUtility.GetRect(26f, EditorGUIUtility.singleLineHeight, GUILayout.MaxWidth(26f));

                property.floatValue = EditorGUILayout.Slider(property.floatValue, leftValue, rightValue, GUILayout.ExpandWidth(true));

                var rightValueRect = GUILayoutUtility.GetRect(26f, EditorGUIUtility.singleLineHeight, GUILayout.MaxWidth(26f));

                leftValueRect.y += EditorGUIUtility.standardVerticalSpacing;
                rightValueRect.y += EditorGUIUtility.standardVerticalSpacing;

                leftValue = EditorGUI.FloatField(leftValueRect, leftValue);
                rightValue = EditorGUI.FloatField(rightValueRect, rightValue);
            }

            EditorGUILayout.Space(EditorGUIUtility.standardVerticalSpacing);
        }
    }
}