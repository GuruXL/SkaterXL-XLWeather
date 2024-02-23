using System;
using UnityEngine;

namespace RapidGUI
{
    public static partial class RGUI
    {
        delegate object SliderFunc(object v, object min, object max);

        public static class SliderSetting
        {
            public static float minWidth = 180f;
            public static float fieldWidth = 45f;
        }

        public static object Slider(object obj, object min, object max, Type type, string label, params GUILayoutOption[] options)
        {
            using (new GUILayout.VerticalScope(options))
            using (new GUILayout.HorizontalScope())
            {
                GUILayout.Label($"<b><color=#ecf0f1>{label}</color></b>");
            }

            return obj;
        }
        public static float SliderFloat(float v, float min, float max, float defaultValue, string label = null, string tooltip = null)
        {
            GUI.backgroundColor = Color.white;
            GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());

            // Combine label and tooltip for display.
            string fullLabel = string.IsNullOrEmpty(tooltip) ? $"<b>{label}</b>" : new GUIContent($"<b>{label}</b>", tooltip).tooltip;
            // Create a style for tooltips
            var tooltipStyle = GUI.skin.box;
            tooltipStyle.alignment = TextAnchor.MiddleLeft;
            tooltipStyle.wordWrap = true;

            // Display label with tooltip.
            GUILayout.Label(new GUIContent(fullLabel, tooltip), new GUILayoutOption[] { GUILayout.MinWidth(150) });

            // Display tooltip if available and mouse is over the label.
            if (!string.IsNullOrEmpty(tooltip) && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
            {
                GUIContent tooltipContent = new GUIContent(tooltip);
                Vector2 tooltipSize = tooltipStyle.CalcSize(tooltipContent);
                GUI.Box(new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y - tooltipSize.y, tooltipSize.x, tooltipSize.y), tooltip, tooltipStyle);
            }

            float num = GUILayout.HorizontalSlider(v, min, max, new GUILayoutOption[] { GUILayout.MinWidth(SliderSetting.minWidth) });
            num = (float)StandardField(num, v.GetType(), GUILayout.Width(SliderSetting.fieldWidth));
            GUI.backgroundColor = Color.white; // reset button color

            if (GUILayout.Button("Reset", new GUILayoutOption[] { GUILayout.Height(21f), GUILayout.ExpandWidth(true) }))
            {
                num = defaultValue;
            }

            GUILayout.EndHorizontal();
            return num;
        }

        /* ORIGINAL SLIDER FUNCTION
        public static float SliderFloat(float v, float min, float max, float defaultValue, string label = null)
        {
            GUI.backgroundColor = Color.white;
            GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
            GUILayout.Label("<b>" + label + "</b>", new GUILayoutOption[]
            {
        GUILayout.MinWidth(150)
            });
            float num = GUILayout.HorizontalSlider(v, min, max, new GUILayoutOption[]
            {
        GUILayout.MinWidth(SliderSetting.minWidth)
            });
            num = (float)StandardField(num, v.GetType(), GUILayout.Width(SliderSetting.fieldWidth));
            GUI.backgroundColor = Color.white; // reset button color
            if (GUILayout.Button("Reset", new GUILayoutOption[]
            {
        GUILayout.Height(21f),
        GUILayout.ExpandWidth(true)
            }))
            {
                num = defaultValue;
            }
            GUILayout.EndHorizontal();
            return num;
        }
        */
    }
}
