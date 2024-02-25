using UnityEngine;
using RapidGUI;
using System.Collections;
using XLWeather.Controller;
using XLWeather.Data;
using System;

namespace XLWeather.UI
{
    public class UIextensions
    {
        public static UIextensions __instance { get; private set; }
        public static UIextensions Instance => __instance ?? (__instance = new UIextensions());

        private string white = "#e6ebe8";
        private string LightBlue = "#30e2e6";
        private string TabColor;

        public string TabColorSwitch(UItab Tab)
        {
            switch (Tab.isClosed)
            {
                case true:
                    TabColor = white;
                    break;

                case false:
                    TabColor = LightBlue;
                    break;
            }
            return TabColor;
        }
        public Color ButtonColorSwitch(bool toggle)
        {
            if (toggle)
            {
                return Color.cyan;
            }
            else
            {
                return Color.white;
            }
        }
        public void CenteredLabel(string label)
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label($"<i><b>{label}</b></i>", GUILayout.ExpandWidth(true));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
        public void FlexableButton(string label, Action buttonAction, Color color)
        {
            GUILayout.BeginHorizontal();
            GUI.backgroundColor = color;
            if (GUILayout.Button($"{label}", RGUIStyle.button, GUILayout.ExpandWidth(true)))
            {
                buttonAction?.Invoke();
            }
            GUILayout.EndHorizontal();
        }

    }
}
