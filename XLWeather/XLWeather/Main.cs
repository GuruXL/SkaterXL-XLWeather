using HarmonyLib;
using System.Reflection;
using System;
using RapidGUI;
using UnityEngine;
using UnityModManagerNet;
using ModIO.UI;
using XLWeather.Utils;
using XLWeather.Data;
using XLWeather.Controller;

namespace XLWeather
{
    public class Main
    {
        public static bool enabled;
        public static Settings settings;
        public static Harmony harmonyInstance;
        public static string modId = "XLWeather";
        public static UnityModManager.ModEntry modEntry;
        public static GameObject scriptManager;
        public static SceneChangeManager SceneUtils;
        public static WeatherController Weatherctrl;
        public static UIcontroller UIctrl;
        public static CycleController Cyclectrl;
        public static DroneController Dronectrl;
        public static MapLightController MapLightctrl;
        //public static MaterialUtil MatUtil;

        private static bool Load(UnityModManager.ModEntry modEntry)
        {
            settings = UnityModManager.ModSettings.Load<Settings>(modEntry);
            modEntry.OnGUI = OnGUI;
            modEntry.OnSaveGUI = new Action<UnityModManager.ModEntry>(OnSaveGUI);
            modEntry.OnToggle = new Func<UnityModManager.ModEntry, bool, bool>(OnToggle);
            modEntry.OnUnload = new Func<UnityModManager.ModEntry, bool>(Unload);
            Main.modEntry = modEntry;
            Logger.Log(nameof(Load));

            return true;
        }
        private static void OnGUI(UnityModManager.ModEntry modEntry)
        {
            
            GUILayout.BeginVertical(GUILayout.Width(284));
            if (settings.MapLayersToggle)
            {
                GUILayout.Label($"<b>Experimental. May cause extra lag during day/night transitions.</b>");
                GUILayout.Label($"<color=#ffe600><i>Please note: Dynamic lights only work on maps that are set up to use this feature.</i></color>");
            }
            if (RGUI.Button(settings.MapLayersToggle, "Dynamic Map Lights"))
            {
                settings.MapLayersToggle = !settings.MapLayersToggle;

                if (settings.MapLayersToggle)
                {
                    MapLightctrl.GetLayerObjects();
                }
                if (!settings.MapLayersToggle)
                {
                    MapLightctrl.ResetLayerToggles();
                }
            }
            GUILayout.EndVertical();
            

            GUILayout.BeginVertical(GUILayout.Width(284));
            if (RGUI.Button(ToggleStateData.HotKeyToggle, "Change HotKey"))
            {
                ToggleStateData.HotKeyToggle = !ToggleStateData.HotKeyToggle;
            }
            if (ToggleStateData.HotKeyToggle)
            {
                GUILayout.Label("<b>Press any Key to change HotKey</b>");
                GUILayout.Box("<b>Current HotKey: </b>" + settings.GetAltKey() + settings.Hotkey.keyCode.ToString(""), GUILayout.Height(25f));
                if (Settings.GetCurrentKeyDown() != null)
                {
                    settings.Hotkey = new KeyBinding { keyCode = (KeyCode)Settings.GetCurrentKeyDown() };
                    MessageSystem.QueueMessage(MessageDisplayData.Type.Success, $"XLWeather HotKey Changed to: " + settings.GetAltKey() + settings.Hotkey.keyCode.ToString(""), 2.5f);
                }

                GUILayout.BeginHorizontal("Box", GUILayout.Width(284));
                if (RGUI.Button(settings.ctrlToggle, "Ctrl:"))
                {
                    settings.ctrlToggle = !settings.ctrlToggle;

                    if (settings.ctrlToggle)
                    {
                        settings.altToggle = false;
                        settings.noneToggle = false;
                    }
                    else if (!settings.ctrlToggle)
                    {
                        settings.noneToggle = true;
                    }
                }
                GUILayout.Space(4);
                if (RGUI.Button(settings.altToggle, "Alt:"))
                {
                    settings.altToggle = !settings.altToggle;

                    if (settings.altToggle)
                    {
                        settings.ctrlToggle = false;
                        settings.noneToggle = false;
                    }
                    else if (!settings.altToggle)
                    {
                        settings.noneToggle = true;
                    }
                }
                GUILayout.Space(4);
                if (RGUI.Button(settings.noneToggle, "None:"))
                {
                    settings.noneToggle = true;

                    if (settings.noneToggle)
                    {
                        settings.ctrlToggle = false;
                        settings.altToggle = false;
                    }
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();

            GUILayout.BeginVertical(GUILayout.Width(256));
            GUILayout.Box("<b>Background Colour</b>", GUILayout.Height(21f));
            settings.BGColor.r = RGUI.SliderFloat(settings.BGColor.r, 0.0f, 1f, 0.85f, "Red");
            settings.BGColor.g = RGUI.SliderFloat(settings.BGColor.g, 0.0f, 1f, 0.90f, "Green");
            settings.BGColor.b = RGUI.SliderFloat(settings.BGColor.b, 0.0f, 1f, 1.0f, "Blue");
            GUILayout.EndVertical();
        }

        private static void OnSaveGUI(UnityModManager.ModEntry modEntry)
        {
            settings.Save(modEntry);
        }
        private static bool OnToggle(UnityModManager.ModEntry modEntry, bool value)
        {
            if (enabled == value)
                return true;

            enabled = value;

            if (enabled)
            {
                if (scriptManager != null)
                {
                    UnityEngine.Object.Destroy(scriptManager);
                }
                harmonyInstance = new Harmony((modEntry.Info).Id);
                harmonyInstance.PatchAll(Assembly.GetExecutingAssembly());
                scriptManager = new GameObject("XLWeather");
                UnityEngine.Object.DontDestroyOnLoad(scriptManager);

                SceneUtils = scriptManager.AddComponent<SceneChangeManager>();
                MapLightctrl = scriptManager.AddComponent<MapLightController>();
                Weatherctrl = scriptManager.AddComponent<WeatherController>();
                Cyclectrl = scriptManager.AddComponent<CycleController>();
                Dronectrl = scriptManager.AddComponent<DroneController>();
                UIctrl = scriptManager.AddComponent<UIcontroller>();
                //MatUtil = scriptManager.AddComponent<MaterialUtil>();

                EnableCheck();
                AssetHandler.Instance.LoadBundles();
            }
            else
            {
                settings.ResetIfEnabled();
                settings.ResetActiveobjs();
                AssetHandler.Instance.UnloadAssetBundle();
                harmonyInstance.UnpatchAll(harmonyInstance.Id);
                UnityEngine.Object.Destroy(scriptManager);
            }

            return true;
        }
        private static bool Unload(UnityModManager.ModEntry modEntry)
        {
            settings.ResetIfEnabled();
            settings.ResetActiveobjs();
            AssetHandler.Instance.UnloadAssetBundle();
            harmonyInstance.UnpatchAll(harmonyInstance.Id);
            UnityEngine.Object.Destroy(scriptManager);
            Logger.Log(nameof(Unload));
            return true;
        }
        public static UnityModManager.ModEntry.ModLogger Logger => modEntry.Logger;

        private static void EnableCheck()
        {
            Weatherctrl.enabled |= true;
            Cyclectrl.enabled |= true;
            Dronectrl.enabled |= true;
            UIctrl.enabled |= true;
            MapLightctrl.enabled |= true;
        }
    }
}