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

            // Create the label content separately from the tooltip.
            GUIContent labelContent = new GUIContent(label, tooltip);
            GUILayout.Label(labelContent, new GUILayoutOption[] { GUILayout.MinWidth(150) });

            if (!string.IsNullOrEmpty(tooltip) && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
            {
                // Create a style for tooltips
                GUIStyle tooltipStyle = new GUIStyle(GUI.skin.box)
                {
                    fontSize = 14,
                    alignment = TextAnchor.UpperCenter,
                    wordWrap = true,
                    padding = new RectOffset(8, 8, 8, 12),
                };

                // solid black texture for background
                Texture2D blackTexture = new Texture2D(1, 1);
                blackTexture.SetPixel(0, 0, Color.black);
                blackTexture.Apply();
                tooltipStyle.normal.background = blackTexture;

                // fixed width for tooltip box with a max width limit
                float maxWidth = 350f;
                float fixedWidth = Mathf.Min(Screen.width / 6, maxWidth);

                // Calculate the height needed for the tooltip text given the fixed width
                GUIContent tooltipContent = new GUIContent(tooltip);
                float calcHeight = tooltipStyle.CalcHeight(tooltipContent, fixedWidth);
                float x = Event.current.mousePosition.x;
                float y = Event.current.mousePosition.y - calcHeight - 12;
                x = Mathf.Max(x, 0);
                x = Mathf.Min(x, Screen.width - fixedWidth);

                // Display tooltip box
                GUI.Box(new Rect(x, y, fixedWidth, calcHeight), tooltipContent, tooltipStyle);
            }

            float num = GUILayout.HorizontalSlider(v, min, max, new GUILayoutOption[] { GUILayout.MinWidth(SliderSetting.minWidth) });
            num = (float)StandardField(num, v.GetType(), GUILayout.Width(SliderSetting.fieldWidth));
            GUI.backgroundColor = Color.white; // Reset background color

            if (GUILayout.Button("Reset", new GUILayoutOption[] { GUILayout.Height(21f), GUILayout.ExpandWidth(false) }))
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
