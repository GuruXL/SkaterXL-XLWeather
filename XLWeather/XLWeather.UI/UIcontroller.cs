using UnityEngine;
using RapidGUI;
using System.Collections;
using XLWeather.Utils;
using XLWeather.Data;
using System;

namespace XLWeather.UI
{
    public class UItab // UI dropdown tabs class
    {
        public bool isClosed;
        public string text;
        public int font;

        public UItab(bool isClosed, string text, int font)
        {
            this.isClosed = isClosed;
            this.text = text;
            this.font = font;
        }
    }

    public class UIcontroller : MonoBehaviour
    {

        public bool showUI;
        private bool setUp;
        private Rect MainWindowRect = new Rect(20, 20, Screen.width / 8, 20);
        private float lastUpdate = 0f;

        public static UIcontroller Instance { get; private set; }

        //readonly UItab Test_Tab = new UItab(true, "Test Stuff", 14);
        readonly UItab Drone_Tab = new UItab(true, "Drone", 14);
        readonly UItab droneFollow_Tab = new UItab(true, "Main Settings", 13);
        readonly UItab droneLight_Tab = new UItab(true, "Light Settings", 13);
        readonly UItab droneColor_Tab = new UItab(true, "Light Color", 12);

        readonly UItab Sky_Tab = new UItab(true, "Sky Options", 14);
        readonly UItab Fog_Tab = new UItab(true, "Fog", 14);

        readonly UItab vfx_Tab = new UItab(true, "VFX", 14);
        readonly UItab Leaf_Tab = new UItab(true, "Leaf Settings", 13);
        readonly UItab Snow_Tab = new UItab(true, "Snow Settings", 13);
        readonly UItab Rain_Tab = new UItab(true, "Rain Settings", 13);
        readonly UItab Cloud_Tab = new UItab(true, "Cloud Settings", 13);

        readonly UItab DayNight_Tab = new UItab(true, "Day/Night Cycle", 14);
        readonly UItab Time_Tab = new UItab(true, "Time settings", 13);
        readonly UItab Light_Tab = new UItab(true, "Light settings", 13);
        readonly UItab Shadow_Tab = new UItab(true, "Shadow settings", 13);
        readonly UItab Presets_Tab = new UItab(true, "Presets", 13);
        //readonly UItab Intensity_Tab = new UItab(true, "Intensity", 12);
        //readonly UItab LightColor_Tab = new UItab(true, "Color", 12);
        //readonly UItab IndirectLight_Tab = new UItab(true, "Indirect Light", 12);
        //readonly UItab Exposure_Tab = new UItab(true, "Exposure", 12);
        readonly UItab Day_Tab = new UItab(true, "Day", 12);
        readonly UItab Night_Tab = new UItab(true, "Night", 12);
        readonly UItab Other_Tab = new UItab(true, "Other Settings", 12);

        //public string white = "#e6ebe8";
        //public string LightBlue =  "#30e2e6";
        //public string green = "#7CFC00";
        //public string red = "#b71540";
        //public string TabColor;

        public string[] NightSkyStates = new string[] {
            "Cloudy Night",
            "Starry Night",
            "Galaxy Night",
            //"Custom Skys"

        };
        public string[] SunSetSkyStates = new string[] {
            "Sun Set 1",
            "Sun Set 2"
        };
        public string[] BlueSkyStates = new string[] {
            "Blue Sky",
            "Cloudy Sky"
        };

        public string[] DroneTargetStates = new string[] {
            "Skateboard",
            "Player"
        };
        public string[] NullState = new string[]
        {
            "none"
        };

        private void Start()
        {
            StartCoroutine(WaitForInput());
        }

        private IEnumerator WaitForInput()
        {
            while (!AssetHandler.Instance.IsPrefabsLoaded() && !AssetHandler.Instance.IsPrefabsSpawned())
            {
                yield return null;
            }

            while (true)
            {
                yield return new WaitForEndOfFrame();
                InputSwitch();
                yield return null;
            }
        }

        private void InputSwitch()
        {
            if (Main.settings.noneToggle)
            {
                if (Input.GetKeyDown(Main.settings.Hotkey.keyCode))
                {
                    ToggleUI();
                }
            }
            else if (Main.settings.ctrlToggle)
            {
                if ((Input.GetKey(KeyCode.LeftControl) | Input.GetKey(KeyCode.RightControl)) && Input.GetKeyDown(Main.settings.Hotkey.keyCode))
                {
                    ToggleUI();
                }
            }
            else if (Main.settings.altToggle)
            {
                if ((Input.GetKey(KeyCode.LeftAlt) | Input.GetKey(KeyCode.RightAlt)) && Input.GetKeyDown(Main.settings.Hotkey.keyCode))
                {
                    ToggleUI();
                }
            }
        }

        private void ToggleUI()
        {
            if (!showUI)
            {
                Open();
            }
            else
            {
                Close();
            }
        }
        private void Open()
        {
            showUI = true;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        private void Close()
        {
            showUI = false;
            Cursor.visible = false;
            Main.settings.Save(Main.modEntry);
        }

        private void OnGUI()
        {
            if (!setUp)
            {
                setUp = true;
            }
            if (!showUI)
                return;
            GUI.backgroundColor = Main.settings.BGColor;
            MainWindowRect = GUILayout.Window(74692, MainWindowRect, MainWindow, "<b> XLWeather </b>");

        }
        // Creates the GUI window
        private void MainWindow(int windowID)
        {
            GUI.backgroundColor = Main.settings.BGColor;
            GUI.DragWindow(new Rect(0, 0, 10000, 20));

            MainUI();
            if (!ToggleStateData.ModEnabled)
            return;

            //TestUI();
            SkyUI();
            FogUI();
            VfxUI();
            DroneUI();      
            DayNightUI();

        }

        private void MainToggle()
        {
            ToggleStateData.ModEnabled = !ToggleStateData.ModEnabled;
        }

        private void MainUI()
        {
            GUILayout.BeginHorizontal();
            UIextensions.Instance.FlexableButton(ToggleStateData.ModEnabled ? "<b> Enabled </b>" : "<b><color=#171717> Disabled </color></b>", MainToggle, UIextensions.Instance.ButtonColorSwitch(ToggleStateData.ModEnabled));
            GUILayout.EndHorizontal();

            if (ToggleStateData.ModEnabled)
            {
                UIextensions.Instance.FlexableButton("Reset All Settings", Main.settings.ResetAllSettings, Color.white);
            }
            // Resets Toggles and Active prefabs
            Main.settings.ResetActiveobjs();

        }

        private void Tabs(UItab obj, string color = "#e6ebe8")
        {
            if (GUILayout.Button($"<size={obj.font}><color={color}>" + (obj.isClosed ? "-" : "<b>+</b>") + obj.text + "</color>" + "</size>", "Label"))
            {
                obj.isClosed = !obj.isClosed;
                MainWindowRect.height = 20;
                MainWindowRect.width = Screen.width / 8;

            }
        }

        private void SkyUI()
        {
            Tabs(Sky_Tab, UIextensions.Instance.TabColorSwitch(Sky_Tab));
            if (Sky_Tab.isClosed)
                return;

            // ------ Night Sky Start -------

            GUILayout.BeginVertical("Box");
            if (RGUI.Button(ToggleStateData.NightSkyToggle, "Night Sky"))
            {
                ToggleStateData.NightSkyToggle = !ToggleStateData.NightSkyToggle;
                ToggleStateData.SunSetSkyToggle = false;
                ToggleStateData.BlueSkyToggle = false;
            }

            switch (ToggleStateData.NightSkyToggle)
            {
                case true:
                    // On toggle for Night Sky
                    AssetHandler.Instance.activeSky[0].SetActive(true);

                    // Creates fixed exposure UI Slider 
                    GUILayout.BeginVertical("Box");
                    Main.settings.NightFixedExposureFloat = RGUI.SliderFloat(Main.settings.NightFixedExposureFloat, 8f, 15f, DefaultSettings.NightFixedExposureFloat, " Exposure", ToolTips.exposure);
                    Main.Weatherctrl.exposure[0].fixedExposure.Override(Main.settings.NightFixedExposureFloat);

                    // UI slider for SkyBox Exposure control
                    Main.settings.NightSkyboxExposureFloat = RGUI.SliderFloat(Main.settings.NightSkyboxExposureFloat, 8f, 15f, DefaultSettings.NightSkyboxExposureFloat, " Sky Exposure", ToolTips.skyExposure);
                    Main.Weatherctrl.activeHDRI[0].exposure.Override(Main.settings.NightSkyboxExposureFloat);

                    // UI slider for SkyBox Rotation
                    Main.settings.NightRotateFloat = RGUI.SliderFloat(Main.settings.NightRotateFloat, 0f, 360f, DefaultSettings.NightRotateFloat, " Rotation", ToolTips.rotation);
                    Main.Weatherctrl.activeHDRI[0].rotation.Override(Main.settings.NightRotateFloat);
                    GUILayout.EndVertical();

                    // Pop up selection for changing skys
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("<b>SkyBox Type: </b>");
                    Main.settings.NightSkyState = RGUI.SelectionPopup(Main.settings.NightSkyState, NightSkyStates);
                    GUILayout.EndHorizontal();
                    break;
                case false:
                    AssetHandler.Instance.activeSky[0].SetActive(false);
                    break;
            }
            GUILayout.EndVertical();

            // check for current Night sky state
            switch (Main.settings.NightSkyState)
            {
                case "Cloudy Night":
                    Main.Weatherctrl.activeHDRI[0].hdriSky.Override(AssetHandler.Instance.activeCubeMap[0]);
                    break;
                case "Starry Night":
                    Main.Weatherctrl.activeHDRI[0].hdriSky.Override(AssetHandler.Instance.activeCubeMap[2]);
                    break;
                case "Galaxy Night":
                    Main.Weatherctrl.activeHDRI[0].hdriSky.Override(AssetHandler.Instance.activeCubeMap[1]);
                    break;
                    //case "Custom Skys":
                    //Main.Weatherctrl.activeHDRI[0].hdriSky.Override(Main.Imagectrl.imgToggle.cube);
                    //break:
            }
            // ------- Night sky End ---------

            // ------ SunSet Sky Start -------
            GUILayout.BeginVertical("Box");
            if (RGUI.Button(ToggleStateData.SunSetSkyToggle, "SunSet Sky"))
            {
                ToggleStateData.SunSetSkyToggle = !ToggleStateData.SunSetSkyToggle;
                ToggleStateData.NightSkyToggle = false;
                ToggleStateData.BlueSkyToggle = false;
            }

            switch (ToggleStateData.SunSetSkyToggle)
            {
                case true:
                    // On toggle for Sun Set
                    AssetHandler.Instance.activeSky[1].SetActive(true);

                    // Creates UI for Slider
                    GUILayout.BeginVertical("Box");
                    Main.settings.SunSetSkyExposureFloat = RGUI.SliderFloat(Main.settings.SunSetSkyExposureFloat, 8f, 15f, DefaultSettings.SunSetSkyExposureFloat, " Exposure", ToolTips.exposure);
                    Main.Weatherctrl.exposure[1].fixedExposure.Override(Main.settings.SunSetSkyExposureFloat);

                    // UI slider for SkyBox Exposure control
                    Main.settings.SunSetSkyboxExposureFloat = RGUI.SliderFloat(Main.settings.SunSetSkyboxExposureFloat, 8f, 15f, DefaultSettings.SunSetSkyboxExposureFloat, " Sky Exposure", ToolTips.skyExposure);
                    Main.Weatherctrl.activeHDRI[1].exposure.Override(Main.settings.SunSetSkyboxExposureFloat);

                    // UI slider for SkyBox Rotation
                    Main.settings.SunSetRotateFloat = RGUI.SliderFloat(Main.settings.SunSetRotateFloat, 0f, 360f, DefaultSettings.SunSetRotateFloat, " Rotation", ToolTips.rotation);
                    Main.Weatherctrl.activeHDRI[1].rotation.Override(Main.settings.SunSetRotateFloat);
                    GUILayout.EndVertical();

                    // Pop up selection for changing skys
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("<b>SkyBox Type: </b>");
                    Main.settings.SunSetSkyState = RGUI.SelectionPopup(Main.settings.SunSetSkyState, SunSetSkyStates);
                    GUILayout.EndHorizontal();
                    break;
                case false:
                    AssetHandler.Instance.activeSky[1].SetActive(false);
                    break;
            }
            GUILayout.EndVertical();

            // check for current Sun Set sky state
            switch (Main.settings.SunSetSkyState)
            {
                case "Sun Set 1":
                    Main.Weatherctrl.activeHDRI[1].hdriSky.Override(AssetHandler.Instance.activeCubeMap[3]);
                    break;
                case "Sun Set 2":
                    Main.Weatherctrl.activeHDRI[1].hdriSky.Override(AssetHandler.Instance.activeCubeMap[4]);
                    break;
            }
            // ------ SunSet Sky End -------

            // ------ Blue Sky Start -------
            GUILayout.BeginVertical("Box");
            if (RGUI.Button(ToggleStateData.BlueSkyToggle, "Clear/Overcast Sky"))
            {
                ToggleStateData.BlueSkyToggle = !ToggleStateData.BlueSkyToggle;
                ToggleStateData.NightSkyToggle = false;
                ToggleStateData.SunSetSkyToggle = false;
            }
            switch (ToggleStateData.BlueSkyToggle)
            {
                case true:
                    // On toggle for Blue sky
                    AssetHandler.Instance.activeSky[2].SetActive(true);

                    // Create Ui for Slider
                    GUILayout.BeginVertical("Box");
                    Main.settings.BlueSkyExposureFloat = RGUI.SliderFloat(Main.settings.BlueSkyExposureFloat, 8f, 15f, DefaultSettings.BlueSkyExposureFloat, " Exposure", ToolTips.exposure);
                    Main.Weatherctrl.exposure[2].fixedExposure.Override(Main.settings.BlueSkyExposureFloat);

                    // UI slider for SkyBox Exposure control
                    Main.settings.BlueSkyboxExposureFloat = RGUI.SliderFloat(Main.settings.BlueSkyboxExposureFloat, 8f, 15f, DefaultSettings.BlueSkyboxExposureFloat, " Sky Exposure", ToolTips.skyExposure);
                    Main.Weatherctrl.activeHDRI[2].exposure.Override(Main.settings.BlueSkyboxExposureFloat);

                    // UI slider for SkyBox Rotation
                    Main.settings.BlueRotateFloat = RGUI.SliderFloat(Main.settings.BlueRotateFloat, 0f, 360f, DefaultSettings.BlueRotateFloat, " Rotation", ToolTips.rotation);
                    Main.Weatherctrl.activeHDRI[2].rotation.Override(Main.settings.BlueRotateFloat);
                    GUILayout.EndVertical();

                    // Pop up selection for changing skys
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("<b>SkyBox Type: </b>");
                    Main.settings.BlueSkyState = RGUI.SelectionPopup(Main.settings.BlueSkyState, BlueSkyStates);
                    GUILayout.EndHorizontal();
                    break;
                case false:
                    AssetHandler.Instance.activeSky[2].SetActive(false);
                    break;
            }
            GUILayout.EndVertical();

            switch (Main.settings.BlueSkyState)
            {
                case "Blue Sky":
                    Main.Weatherctrl.activeHDRI[2].hdriSky.Override(AssetHandler.Instance.activeCubeMap[5]);
                    break;
                case "Cloudy Sky":
                    Main.Weatherctrl.activeHDRI[2].hdriSky.Override(AssetHandler.Instance.activeCubeMap[6]);
                    break;
            }
            // ------ Blue Sky End -------

            // ------ Map Light Control Start -------
            GUILayout.BeginVertical("Box");
            GUI.backgroundColor = Color.white;
            if (GUILayout.Button("Scan for Sun", RGUIStyle.button, GUILayout.Width(128)))
            {
                Main.MapLightctrl.GetMainSun();
            }
            Main.settings.MapLightIntensity = RGUI.SliderFloat(Main.settings.MapLightIntensity, 0f, 20000f, Main.settings.DefaultMapLightIntensity, " Sun Light Intensity", ToolTips.defaultLightIntensity);
            GUILayout.Label(Main.MapLightctrl.MainLight != null ? "<b><color=#7CFC00> Sun Light Found </color></b>" : "<b><color=#b30000> No Sun Light Found </color></b>", GUILayout.Width(184));
            GUILayout.EndVertical();
            // ------ Map Light Control End -------
        }

        private void AddFogSwitch()
        {
            ToggleStateData.addFogToggle = !ToggleStateData.addFogToggle;
            ToggleStateData.removeFogToggle = false;
        }
        private void RemoveFogSwitch()
        {
            ToggleStateData.removeFogToggle = !ToggleStateData.removeFogToggle;
            ToggleStateData.addFogToggle = false;
        }

        private void FogUI()
        {
            Tabs(Fog_Tab, UIextensions.Instance.TabColorSwitch(Fog_Tab));
            if (Fog_Tab.isClosed)
                return;

            GUILayout.BeginVertical("Box");
            if (RGUI.Button(ToggleStateData.FogToggle, "Fog Settings"))
            {
                ToggleStateData.FogToggle = !ToggleStateData.FogToggle;

                switch (ToggleStateData.FogToggle)
                {
                    case true:
                        // Off toggle for Fog
                        AssetHandler.Instance.activeSky[3].SetActive(true);
                        break;
                    case false:
                        // Off toggle for Fog
                        Main.Weatherctrl.ResetDataSettings();
                        AssetHandler.Instance.activeSky[3].SetActive(false);
                        ToggleStateData.addFogToggle = false;
                        ToggleStateData.removeFogToggle = false;
                        break;
                }
            }

            if (ToggleStateData.FogToggle)
            {
                // ------ fog start -------

                GUILayout.BeginVertical();

                UIextensions.Instance.FlexableButton("Remove Fog", RemoveFogSwitch, UIextensions.Instance.ButtonColorSwitch(ToggleStateData.removeFogToggle));

                if (ToggleStateData.removeFogToggle)
                {
                    Main.Weatherctrl.CustomFog.enabled.Override(false);
                    ToggleStateData.addFogToggle = false;
                }
                GUILayout.EndVertical();

                GUILayout.BeginVertical();

                UIextensions.Instance.FlexableButton("Add Fog", AddFogSwitch, UIextensions.Instance.ButtonColorSwitch(ToggleStateData.addFogToggle));

                if (ToggleStateData.addFogToggle)
                {
                    Main.Weatherctrl.CustomFog.enabled.Override(true);
                    ToggleStateData.removeFogToggle = false;

                    GUILayout.BeginVertical("Box");
                    Main.settings.fogMeanFreePath = RGUI.SliderFloat(Main.settings.fogMeanFreePath, 1f, 400f, DefaultSettings.fogMeanFreePath, " Distance Density", ToolTips.fogDistance);
                    Main.settings.fogBaseHeight = RGUI.SliderFloat(Main.settings.fogBaseHeight, 0f, 200f, DefaultSettings.fogBaseHeight, " Base Height", ToolTips.fogHeight);
                    Main.settings.fogMaxDistance = RGUI.SliderFloat(Main.settings.fogMaxDistance, 0f, 1000f, DefaultSettings.fogMaxDistance, " Max Distance", ToolTips.fogMaxDistance);
                    Main.settings.fogMaxHeight = RGUI.SliderFloat(Main.settings.fogMaxHeight, 0f, 2000f, DefaultSettings.fogMaxHeight, " Max Height", ToolTips.fogMaxHeight);
                    GUILayout.EndVertical();
                    GUILayout.BeginVertical("Box");
                    if (RGUI.Button(ToggleStateData.fogVolumetrics, "Fog Volumetrics"))
                    {
                        ToggleStateData.fogVolumetrics = !ToggleStateData.fogVolumetrics;
                    }
                    switch (ToggleStateData.fogVolumetrics)
                    {
                        case true:
                            Main.Weatherctrl.CustomFog.enableVolumetricFog.Override(true);
                            AssetHandler.Instance.ActiveDensityVol.SetActive(true);

                            GUILayout.BeginVertical("Box");
                            Main.settings.fogDensitySpeedX = RGUI.SliderFloat(Main.settings.fogDensitySpeedX, 0f, 0.5f, DefaultSettings.fogDensitySpeedX, "X speed");
                            Main.settings.fogDensitySpeedY = RGUI.SliderFloat(Main.settings.fogDensitySpeedY, 0f, 0.5f, DefaultSettings.fogDensitySpeedY, "Y speed");
                            Main.settings.fogDensitySpeedZ = RGUI.SliderFloat(Main.settings.fogDensitySpeedZ, 0f, 0.5f, DefaultSettings.fogDensitySpeedZ, "Z speed");
                            GUILayout.EndVertical();
                            GUILayout.BeginVertical("Box");
                            Main.settings.fogDensityDistance = RGUI.SliderFloat(Main.settings.fogDensityDistance, 1f, 100f, DefaultSettings.fogDensityDistance, " Volumetric Density", ToolTips.fogVolDistance);
                            GUILayout.EndVertical();
                            break;
                        case false:
                            Main.Weatherctrl.CustomFog.enableVolumetricFog.Override(false);
                            AssetHandler.Instance.ActiveDensityVol.SetActive(false);
                            break;
                    }
                    GUILayout.EndVertical();
                }
                else if (!ToggleStateData.addFogToggle)
                {
                    Main.Weatherctrl.CustomFog.enableVolumetricFog.Override(false);
                    AssetHandler.Instance.ActiveDensityVol.SetActive(false);
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndVertical();
            // ------- fog End -------
        }

        private void VfxUI()
        {
            Tabs(vfx_Tab, UIextensions.Instance.TabColorSwitch(vfx_Tab));
            if (vfx_Tab.isClosed)
                return;

            // ------ Leaves Start --------
            GUILayout.BeginVertical("Box");
            if (RGUI.Button(ToggleStateData.LeafvfxToggle, "<b>Leaves</b>"))
            {
                ToggleStateData.LeafvfxToggle = !ToggleStateData.LeafvfxToggle;
            }
            switch (ToggleStateData.LeafvfxToggle)
            {
                case true:
                    AssetHandler.Instance.activeVFX[0].SetActive(true);

                    Tabs(Leaf_Tab, UIextensions.Instance.TabColorSwitch(Leaf_Tab));
                    if (!Leaf_Tab.isClosed)
                    {
                        // Creates UI for Slider
                        GUILayout.BeginVertical("Box");
                        Main.settings.LeafDensityFloat = RGUI.SliderFloat((int)Main.settings.LeafDensityFloat, 0f, 80000f, DefaultSettings.LeafDensityFloat, "Density", ToolTips.VFXDensity);
                        Main.settings.LeafSizeFloat = RGUI.SliderFloat(Main.settings.LeafSizeFloat, 1f, 200f, DefaultSettings.LeafSizeFloat, "Area Size", ToolTips.VFXArea);
                        Main.settings.LeafGravityFloat = RGUI.SliderFloat(Main.settings.LeafGravityFloat, 1f, 6f, DefaultSettings.LeafGravityFloat, "Gravity", ToolTips.VFXGravity);
                        Main.settings.LeafWindFloat = RGUI.SliderFloat(Main.settings.LeafWindFloat, 1f, 12f, DefaultSettings.LeafWindFloat, "Wind", ToolTips.VFXWind);
                        Main.settings.LeafLifeFloat = RGUI.SliderFloat(Main.settings.LeafLifeFloat, 1f, 12f, DefaultSettings.LeafLifeFloat, "Lifetime", ToolTips.VFXLifetime);
                        GUILayout.EndVertical();
                    }    
                    break;
                case false:
                    Main.Weatherctrl.ResetDataSettings();
                    AssetHandler.Instance.activeVFX[0].SetActive(false);
                    break;
            }
            GUILayout.EndVertical();
            // ------ Leaves End --------

            // ------ Snow Start --------
            GUILayout.BeginVertical("Box");
            if (RGUI.Button(ToggleStateData.SnowvfxToggle, "<b>Snow</b>"))
            {
                ToggleStateData.SnowvfxToggle = !ToggleStateData.SnowvfxToggle;
            }

            switch (ToggleStateData.SnowvfxToggle)
            {
                case true:
                    AssetHandler.Instance.activeVFX[1].SetActive(true);

                    Tabs(Snow_Tab, UIextensions.Instance.TabColorSwitch(Snow_Tab));
                    if (!Snow_Tab.isClosed)
                    {
                        // Creates UI for Slider
                        GUILayout.BeginVertical("Box");
                        Main.settings.SnowDensityFloat = RGUI.SliderFloat((int)Main.settings.SnowDensityFloat, 0f, 285000f, DefaultSettings.SnowDensityFloat, "Density", ToolTips.VFXDensity);
                        Main.settings.SnowSizeFloat = RGUI.SliderFloat(Main.settings.SnowSizeFloat, 1f, 200f, DefaultSettings.SnowSizeFloat, "Area Size", ToolTips.VFXArea);
                        Main.settings.SnowGravityFloat = RGUI.SliderFloat(Main.settings.SnowGravityFloat, 1f, 4f, DefaultSettings.SnowGravityFloat, "Gravity", ToolTips.VFXGravity);
                        Main.settings.SnowWindFloat = RGUI.SliderFloat(Main.settings.SnowWindFloat, 0f, 4f, DefaultSettings.SnowWindFloat, "Wind", ToolTips.VFXWind);
                        Main.settings.SnowLifeFloat = RGUI.SliderFloat(Main.settings.SnowLifeFloat, 1f, 16f, DefaultSettings.SnowLifeFloat, "Lifetime", ToolTips.VFXLifetime);
                        GUILayout.EndVertical();
                    }   
                    break;
                case false:
                    Main.Weatherctrl.ResetDataSettings();
                    AssetHandler.Instance.activeVFX[1].SetActive(false);
                    break;
            }
            GUILayout.EndVertical();
            // ------ Snow End --------

            // ------ Rain Start --------
            GUILayout.BeginVertical("Box");
            if (RGUI.Button(ToggleStateData.RainvfxToggle, "<b>Rain</b>"))
            {
                ToggleStateData.RainvfxToggle = !ToggleStateData.RainvfxToggle;
            }

            switch (ToggleStateData.RainvfxToggle)
            {
                case true:
                    AssetHandler.Instance.activeVFX[2].SetActive(true);

                    Tabs(Rain_Tab, UIextensions.Instance.TabColorSwitch(Rain_Tab));
                    if (!Rain_Tab.isClosed)
                    {
                        // Creates UI for Slider
                        GUILayout.BeginVertical("Box");
                        Main.settings.RainVolumeFloat = RGUI.SliderFloat(Main.settings.RainVolumeFloat, 0f, 1f, 0.4f, "Volume", ToolTips.audioVolume);
                        GUILayout.EndVertical();
                        GUILayout.BeginVertical("Box");
                        Main.settings.RainDensityFloat = RGUI.SliderFloat((int)Main.settings.RainDensityFloat, 0f, 850000f, DefaultSettings.RainDensityFloat, "Density", ToolTips.VFXDensity);
                        Main.settings.RainSizeFloat = RGUI.SliderFloat(Main.settings.RainSizeFloat, 1f, 100f, DefaultSettings.RainSizeFloat, "Area Size", ToolTips.VFXArea);
                        Main.settings.RainGravityFloat = RGUI.SliderFloat(Main.settings.RainGravityFloat, 1f, 60f, DefaultSettings.RainGravityFloat, "Gravity", ToolTips.VFXGravity);
                        Main.settings.RainWindFloat = RGUI.SliderFloat(Main.settings.RainWindFloat, 0f, 4f, DefaultSettings.RainWindFloat, "Wind", ToolTips.VFXWind);
                        Main.settings.RainLifeFloat = RGUI.SliderFloat(Main.settings.RainLifeFloat, 0f, 4f, DefaultSettings.RainLifeFloat, "Lifetime", ToolTips.VFXLifetime);
                        GUILayout.EndVertical();
                        GUILayout.BeginVertical("Box");

                        if (RGUI.Button(ToggleStateData.LightningToggle, "<b>Lightning</b>"))
                        {
                            ToggleStateData.LightningToggle = !ToggleStateData.LightningToggle;
                        }

                        switch (ToggleStateData.LightningToggle)
                        {
                            case true:
                                AssetHandler.Instance.activeVFX[4].SetActive(true);
                                break;
                            case false:
                                AssetHandler.Instance.activeVFX[4].SetActive(false);
                                break;
                        }
                        GUILayout.EndVertical();
                    }             
                    break;

                case false:
                    Main.Weatherctrl.ResetDataSettings();
                    AssetHandler.Instance.activeVFX[2].SetActive(false);
                    AssetHandler.Instance.activeVFX[4].SetActive(false);
                    break;
            }
            GUILayout.EndVertical();
            // ------ Rain End --------

            // ------ Aroura start --------
            GUILayout.BeginVertical("Box");
            if (RGUI.Button(ToggleStateData.AuroravfxToggle, "<b>Aurora</b>"))
            {
                ToggleStateData.AuroravfxToggle = !ToggleStateData.AuroravfxToggle;
            }
            GUILayout.EndVertical();

            switch (ToggleStateData.AuroravfxToggle)
            {
                case true:
                    AssetHandler.Instance.activeVFX[3].SetActive(true);
                    break;

                case false:
                    AssetHandler.Instance.activeVFX[3].SetActive(false);
                    break;
            }
            // ------ Aurora End --------

            // ------ Clouds ----------
            GUILayout.BeginVertical("Box");
            if (RGUI.Button(ToggleStateData.CloudToggle, "Clouds"))
            {
                ToggleStateData.CloudToggle = !ToggleStateData.CloudToggle;
            }
            switch (ToggleStateData.CloudToggle)
            {
                case true:
                    AssetHandler.Instance.activeVFX[5].SetActive(true);

                    Tabs(Cloud_Tab, UIextensions.Instance.TabColorSwitch(Cloud_Tab));
                    if (!Cloud_Tab.isClosed)
                    {
                        // Creates UI for Slider
                        GUILayout.BeginVertical("Box");
                        GUILayout.Label($"<color=#ffe600><i> This effect is experimental. You may experience visual bugs.</i></color>");
                        GUILayout.Label("note: Clouds will look cut off if render distance is lower than 1000 in XLGraphics");
                        GUILayout.EndVertical();
                        GUILayout.BeginVertical("Box");
                        Main.settings.Cloud_BaseHeight = RGUI.SliderFloat(Main.settings.Cloud_BaseHeight, 0f, 400f, DefaultSettings.Cloud_BaseHeight, " BaseHeight", ToolTips.cloudBaseHeight);
                        Main.settings.Cloud_ParallexOffset = RGUI.SliderFloat(Main.settings.Cloud_ParallexOffset, 0f, -80f, DefaultSettings.Cloud_ParallexOffset, " Depth", ToolTips.cloudParallax);
                        Main.settings.Cloud_Iterations = RGUI.SliderFloat(Main.settings.Cloud_Iterations, 0f, 12f, DefaultSettings.Cloud_Iterations, " Cloud Layers", ToolTips.cloudIterations);
                        Main.settings.Cloud_NoiseScale = RGUI.SliderFloat(Main.settings.Cloud_NoiseScale, 0f, 4f, DefaultSettings.Cloud_NoiseScale, " Cloud Size", ToolTips.cloudNoiseScale);
                        Main.settings.Cloud_NoiseDepth = RGUI.SliderFloat(Main.settings.Cloud_NoiseDepth, 0f, 4f, DefaultSettings.Cloud_NoiseDepth, " Cloud Cover", ToolTips.cloudNoiseDepth);
                        Main.settings.Cloud_CrackTiling_x = RGUI.SliderFloat(Main.settings.Cloud_CrackTiling_x, 0f, 2f, DefaultSettings.Cloud_CrackTiling_x, " Tiling_X", ToolTips.cloudTiling);
                        Main.settings.Cloud_CrackTiling_y = RGUI.SliderFloat(Main.settings.Cloud_CrackTiling_y, 0f, 2f, DefaultSettings.Cloud_CrackTiling_y, " Tiling_Y", ToolTips.cloudTiling);
                        Main.settings.Cloud_Speed = RGUI.SliderFloat(Main.settings.Cloud_Speed, 0f, 0.1f, DefaultSettings.Cloud_Speed, " Speed", ToolTips.cloudSpeed);    
                        Main.settings.Cloud_Intensity = RGUI.SliderFloat(Main.settings.Cloud_Intensity, 0f, 80f, DefaultSettings.Cloud_Intensity, " Intensity", ToolTips.cloudIntensity);
                        GUILayout.EndVertical();
                    }  
                    break;
                case false:
                    Main.Weatherctrl.ResetDataSettings();
                    AssetHandler.Instance.activeVFX[5].SetActive(false);
                    break;
            }

            GUILayout.EndVertical();
            // ------ Clouds End ----------
        }
        private void DroneUI()
        {
            Tabs(Drone_Tab, UIextensions.Instance.TabColorSwitch(Drone_Tab));
            if (Drone_Tab.isClosed)
                return;

            GUILayout.BeginVertical("Box");
            if (RGUI.Button(ToggleStateData.DroneToggle, "Drone"))
            {
                ToggleStateData.DroneToggle = !ToggleStateData.DroneToggle;

                switch (ToggleStateData.DroneToggle)
                {
                    case true:
                        // On toggle for Drone         
                        AssetHandler.Instance.activeDrone.SetActive(true);
                        break;

                    case false:
                        // Off toggle for Drone
                        AssetHandler.Instance.activeDrone.SetActive(false);
                        break;
                }
            }
            GUILayout.EndVertical();

            if (ToggleStateData.DroneToggle)
            {
                GUILayout.BeginVertical("Box");
                // Drone Cam
                GUILayout.BeginVertical("Box");
                if (RGUI.Button(ToggleStateData.DroneCamtoggle, "Drone Cam"))
                {
                    ToggleStateData.DroneCamtoggle = !ToggleStateData.DroneCamtoggle;
                }

                switch (ToggleStateData.DroneCamtoggle)
                {
                    case true:
                        //Main.Dronectrl.DroneLightCam.enabled = true;
                        Main.Dronectrl.DroneLightCam.gameObject.SetActive(true);
                        GUILayout.BeginVertical("Box");
                        Main.settings.droneCamFov = RGUI.SliderFloat(Main.settings.droneCamFov, 10f, 130f, DefaultSettings.droneCamFov, " Cam FOV", ToolTips.droneCamFOV);
                        GUILayout.Label("<i>This is a work in progress, you may experience camera jitter or other bugs</i>");
                        GUILayout.Label("Drone Target");
                        Main.settings.DroneTargetState = RGUI.SelectionPopup(Main.settings.DroneTargetState, DroneTargetStates);
                        GUILayout.EndVertical();

                        GUILayout.BeginVertical("Box");
                        if (RGUI.Button(ToggleStateData.MultiTargetToggle, "Target Online Player"))
                        {
                            ToggleStateData.MultiTargetToggle = !ToggleStateData.MultiTargetToggle;
                        }
                        switch (ToggleStateData.MultiTargetToggle)
                        {
                            case true:
                                GUILayout.Label("Player Target");
                                Main.settings.multiplayer_target = RGUI.SelectionPopup(Main.settings.multiplayer_target, Main.Dronectrl.getPlayerList());

                                if (Main.Dronectrl.ReplayCheck())
                                {
                                    GUILayout.Label("<color=#FFFF00><i> Replay Targeting is Not functional at this time. </i></color>");
                                }
                                break;

                            case false:
                                Main.settings.multiplayer_target = "None";
                                break;
                        }
                        GUILayout.EndVertical();
                        break;

                    case false:
                        //Main.Dronectrl.DroneLightCam.enabled = false;
                        Main.Dronectrl.DroneLightCam.gameObject.SetActive(false);
                        break;
                }
                GUILayout.EndVertical();

                Tabs(droneFollow_Tab, UIextensions.Instance.TabColorSwitch(droneFollow_Tab));
                if (!droneFollow_Tab.isClosed)
                {
                    GUILayout.BeginVertical("Box");
                    Main.settings.droneVolume = RGUI.SliderFloat(Main.settings.droneVolume, 0f, 1f, DefaultSettings.droneVolume, " Drone Volume", ToolTips.audioVolume);
                    GUILayout.EndVertical();
                    GUILayout.BeginVertical("Box");
                    Main.settings.droneRayDistance = RGUI.SliderFloat(Main.settings.droneRayDistance, 1f, 20f, DefaultSettings.droneRayDistance, " Drone Height", ToolTips.droneHeight);
                    Main.settings.droneRotSpeed = RGUI.SliderFloat(Main.settings.droneRotSpeed, 0f, 10f, DefaultSettings.droneRotSpeed, " Rotation Speed", ToolTips.droneRotationSpeed);
                    Main.settings.dronefollowSharpness = RGUI.SliderFloat(Main.settings.dronefollowSharpness, 0f, 0.1f, DefaultSettings.dronefollowSharpness, " Follow Sharpness", ToolTips.droneFollowSharpness);
                    GUILayout.EndVertical();
                }

                Tabs(droneLight_Tab, UIextensions.Instance.TabColorSwitch(droneLight_Tab));
                if (!droneLight_Tab.isClosed)
                {
                    GUILayout.BeginVertical("Box");
                    Main.settings.droneLtIntesityFlt = RGUI.SliderFloat(Main.settings.droneLtIntesityFlt, 0f, 25f, DefaultSettings.droneLtIntesityFlt, " Intensity", ToolTips.droneLightIntensity);
                    Main.settings.droneLtRangeFlt = RGUI.SliderFloat(Main.settings.droneLtRangeFlt, 0f, 400f, DefaultSettings.droneLtRangeFlt, " Range", ToolTips.droneLightRange);
                    Main.settings.droneLtAngleFlt = RGUI.SliderFloat(Main.settings.droneLtAngleFlt, 0f, 200f, DefaultSettings.droneLtAngleFlt, " Angle", ToolTips.droneLightAngle);
                    Main.settings.droneLtRadiusFlt = RGUI.SliderFloat(Main.settings.droneLtRadiusFlt, 0f, 10f, DefaultSettings.droneLtRadiusFlt, " Radius", ToolTips.droneLightRadius);
                    Main.settings.droneLtDimmerFlt = RGUI.SliderFloat(Main.settings.droneLtDimmerFlt, 0f, 25f, DefaultSettings.droneLtDimmerFlt, " Dimmer", ToolTips.droneLightDimmer);
                    GUILayout.Space(3);
                    Tabs(droneColor_Tab, UIextensions.Instance.TabColorSwitch(droneColor_Tab));
                    if (!droneColor_Tab.isClosed)
                    {
                        GUILayout.BeginVertical("Box");
                        Main.settings.DroneLightColor.r = RGUI.SliderFloat(Main.settings.DroneLightColor.r, 0.0f, 1.0f, 1.0f, "Red");
                        Main.settings.DroneLightColor.g = RGUI.SliderFloat(Main.settings.DroneLightColor.g, 0.0f, 1.0f, 1.0f, "Green");
                        Main.settings.DroneLightColor.b = RGUI.SliderFloat(Main.settings.DroneLightColor.b, 0.0f, 1.0f, 1.0f, "Blue");
                        GUILayout.EndVertical();
                        GUILayout.BeginVertical("Box");
                        if (RGUI.Button(ToggleStateData.DroneColorLerptoggle, "RGB"))
                        {
                            ToggleStateData.DroneColorLerptoggle = !ToggleStateData.DroneColorLerptoggle;

                            if (ToggleStateData.DroneColorLerptoggle)
                            {
                                StartCoroutine(Main.Dronectrl.ColorLerpRoutine);
                            }
                            if (!ToggleStateData.DroneColorLerptoggle)
                            {
                                StopCoroutine(Main.Dronectrl.ColorLerpRoutine);
                                Main.Dronectrl.droneLight.color = Color.white;
                            }
                        }
                        GUILayout.EndVertical();
                    }
                    GUILayout.EndVertical();
                }    
                GUILayout.EndVertical();
            }
        }
        private void DayNightUI()
        {
            Tabs(DayNight_Tab, UIextensions.Instance.TabColorSwitch(DayNight_Tab));
            if (DayNight_Tab.isClosed)
                return;
            GUILayout.BeginVertical("Box");
            if (RGUI.Button(ToggleStateData.DayNightToggle, "Day/Night Cycle"))
            {
                ToggleStateData.DayNightToggle = !ToggleStateData.DayNightToggle;

                switch (ToggleStateData.DayNightToggle)
                {
                    case true:
                        // On toggle for Day/Night Cycle
                        AssetHandler.Instance.activeDayNight.SetActive(true);
                        Main.MapLightctrl.ToggleMainLights(false);
                        break;

                    case false:
                        // Off toggle for Day/Night Cycle
                        AssetHandler.Instance.activeDayNight.SetActive(false);
                        Main.MapLightctrl.ToggleMainLights(true);

                        if (Main.settings.MapLayersToggle)
                        {
                            Main.MapLightctrl.ResetLayerToggles();
                        }

                        ToggleStateData.ResetVolWeight = true;

                        break;
                }
            }
            GUILayout.EndVertical();

            if (ToggleStateData.DayNightToggle)
            {
                GUILayout.BeginHorizontal("Box");
                GUILayout.Label("Time of Day: " + Main.Cyclectrl.timeText);
                GUILayout.EndHorizontal();

                GUILayout.BeginVertical("Box"); // start Day Night Tabs
                Tabs(Time_Tab, UIextensions.Instance.TabColorSwitch(Time_Tab));
                if (!Time_Tab.isClosed)
                {
                    // Time Setting Control
                    GUILayout.BeginVertical("Box");
                    UIextensions.Instance.CenteredLabel("Time of Day Settings");
                    Main.settings.timeMulipiler = RGUI.SliderFloat(Main.settings.timeMulipiler, 0f, 2500f, DefaultSettings.timeMulipiler, "Cycle Speed", ToolTips.cycleSpeed);
                    Main.settings.startHour = RGUI.SliderFloat(Main.settings.startHour, 0f, 24f, DefaultSettings.startHour, "Start Time", ToolTips.cycleStartTime);
                    Main.settings.sunriseHour = RGUI.SliderFloat(Main.settings.sunriseHour, 0f, 24f, DefaultSettings.sunriseHour, "Sun Rise Time", ToolTips.cycleSunRiseTime);
                    Main.settings.sunsetHour = RGUI.SliderFloat(Main.settings.sunsetHour, 0f, 24f, DefaultSettings.sunsetHour, "Sun Set Time", ToolTips.cycleSunSetTime);
                    Main.settings.CycleXrotFloat = RGUI.SliderFloat(Main.settings.CycleXrotFloat, 0f, 360f, DefaultSettings.CycleXrotFloat, "Rotation", ToolTips.cycleRotation);
                    GUILayout.EndVertical();

                }
                Tabs(Light_Tab, UIextensions.Instance.TabColorSwitch(Light_Tab));
                if (!Light_Tab.isClosed)
                {
                    GUILayout.BeginVertical("Box"); // start of Light Layout

                    Tabs(Day_Tab, UIextensions.Instance.TabColorSwitch(Day_Tab));
                    if (!Day_Tab.isClosed)
                    {
                        GUILayout.BeginVertical(); // Start Day Layout

                        GUILayout.BeginVertical("Box");
                        UIextensions.Instance.CenteredLabel("Sun Size");
                        Main.settings.sunAngularDiameter = RGUI.SliderFloat(Main.settings.sunAngularDiameter, -5f, 5f, DefaultSettings.sunAngularDiameter, "Sun Size", ToolTips.cycleSize);
                        GUILayout.EndVertical();

                        GUILayout.BeginVertical("Box");
                        UIextensions.Instance.CenteredLabel("Sun Intensity");
                        Main.settings.sunMinIntensity = RGUI.SliderFloat(Main.settings.sunMinIntensity, 0f, 18000f, DefaultSettings.sunMinIntensity, "Sunrise/Sunset", ToolTips.cycleMinIntensity);
                        Main.settings.sunMaxIntensity = RGUI.SliderFloat(Main.settings.sunMaxIntensity, 0f, 28000f, DefaultSettings.sunMaxIntensity, "Noon", ToolTips.cycleMaxIntensity);
                        Main.settings.sunDimmerFloat = RGUI.SliderFloat(Main.settings.sunDimmerFloat, 0.1f, 2f, DefaultSettings.sunDimmerFloat, "Sun Dimmer", ToolTips.cycleDimmer);
                        GUILayout.EndVertical();

                        GUILayout.BeginVertical("Box");
                        UIextensions.Instance.CenteredLabel("Color Temperature");
                        Main.settings.SunColorFloat = RGUI.SliderFloat(Main.settings.SunColorFloat, 1000f, 20000f, DefaultSettings.SunColorFloat, "Sun Color Temp", ToolTips.cycleSunColor);
                        GUILayout.EndVertical();

                        GUILayout.BeginVertical("Box");
                        UIextensions.Instance.CenteredLabel("Sun Indirect Light");
                        Main.settings.SunIndirectLight = RGUI.SliderFloat(Main.settings.SunIndirectLight, 0f, 4f, DefaultSettings.SunIndirectLight, "Sun Indirect Light", ToolTips.cycleIndirectLight);
                        Main.settings.SunIndirectSpecular = RGUI.SliderFloat(Main.settings.SunIndirectSpecular, 0f, 4f, DefaultSettings.SunIndirectSpecular, "Sun Specular Light", ToolTips.cycleSpecularLight);
                        GUILayout.EndVertical();

                        GUILayout.BeginVertical("Box");
                        UIextensions.Instance.CenteredLabel("Sun Exposure");
                        GUILayout.Label("<i> Brightness</i>");
                        Main.settings.SunMinExFloat = RGUI.SliderFloat(Main.settings.SunMinExFloat, 0f, 20f, DefaultSettings.SunMinExFloat, "Min Exposure", ToolTips.cycleMinExposure);
                        GUILayout.Label("<i> Darkness</i>");
                        Main.settings.SunMaxExFloat = RGUI.SliderFloat(Main.settings.SunMaxExFloat, 0f, 20f, DefaultSettings.SunMaxExFloat, "Max Exposure", ToolTips.cycleMaxExposure);
                        Main.settings.SunExCompFlt = RGUI.SliderFloat(Main.settings.SunExCompFlt, -2.0f, 2.0f, DefaultSettings.SunExCompFlt, "Compensation", ToolTips.cycleCompensation);
                        Main.settings.sunSkyExFloat = RGUI.SliderFloat(Main.settings.sunSkyExFloat, -2f, 2f, DefaultSettings.sunSkyExFloat, "Sky Exposure", ToolTips.skyExposure);
                        GUILayout.EndVertical();

                        GUILayout.EndVertical(); // End Day Layout
                    }

                    Tabs(Night_Tab, UIextensions.Instance.TabColorSwitch(Night_Tab));
                    if (!Night_Tab.isClosed)
                    {
                        GUILayout.BeginVertical(); // Start Night Layout

                        GUILayout.BeginVertical("Box");
                        UIextensions.Instance.CenteredLabel("Moon Size");
                        Main.settings.moonAngularDiameter = RGUI.SliderFloat(Main.settings.moonAngularDiameter, 1f, 10f, DefaultSettings.moonAngularDiameter, "Moon Size", ToolTips.cycleSize);
                        GUILayout.EndVertical();

                        GUILayout.BeginVertical("Box");
                        UIextensions.Instance.CenteredLabel("Moon Intensity");
                        Main.settings.moonIntensity = RGUI.SliderFloat(Main.settings.moonIntensity, 0f, 8500f, DefaultSettings.moonIntensity, "Moon Intensity", ToolTips.cycleMoonIntensity);
                        Main.settings.moonDimmerFloat = RGUI.SliderFloat(Main.settings.moonDimmerFloat, 0.1f, 2f, DefaultSettings.moonDimmerFloat, "Moon Dimmer", ToolTips.cycleDimmer);
                        GUILayout.EndVertical();

                        GUILayout.BeginVertical("Box");
                        UIextensions.Instance.CenteredLabel("Color Temperature");
                        Main.settings.MoonColorFloat = RGUI.SliderFloat(Main.settings.MoonColorFloat, 1000f, 20000f, DefaultSettings.MoonColorFloat, "Moon Color Temp", ToolTips.cycleMoonColor);
                        GUILayout.EndVertical();

                        GUILayout.BeginVertical("Box");
                        UIextensions.Instance.CenteredLabel("Moon Indirect Light");
                        Main.settings.MoonIndirectLight = RGUI.SliderFloat(Main.settings.MoonIndirectLight, 0f, 4f, DefaultSettings.MoonIndirectLight, "Moon Indirect Light", ToolTips.cycleIndirectLight);
                        Main.settings.MoonIndirectSpecular = RGUI.SliderFloat(Main.settings.MoonIndirectSpecular, 0f, 4f, DefaultSettings.MoonIndirectSpecular, "Moon Specular Light", ToolTips.cycleSpecularLight);
                        GUILayout.EndVertical();

                        GUILayout.BeginVertical("Box");
                        UIextensions.Instance.CenteredLabel("Moon Exposure");
                        GUILayout.Label("<i> Brightness</i>");
                        Main.settings.MoonMinExFloat = RGUI.SliderFloat(Main.settings.MoonMinExFloat, 0f, 20f, DefaultSettings.MoonMinExFloat, "Min Exposure", ToolTips.cycleMinExposure);
                        GUILayout.Label("<i> Darkness</i>");
                        Main.settings.MoonMaxExFloat = RGUI.SliderFloat(Main.settings.MoonMaxExFloat, 0f, 20f, DefaultSettings.MoonMaxExFloat, "Max Exposure", ToolTips.cycleMaxExposure);
                        Main.settings.MoonExCompFlt = RGUI.SliderFloat(Main.settings.MoonExCompFlt, -2.0f, 2.0f, DefaultSettings.MoonExCompFlt, "Compensation", ToolTips.cycleCompensation);
                        Main.settings.moonSkyExFloat = RGUI.SliderFloat(Main.settings.moonSkyExFloat, -2f, 2f, DefaultSettings.moonSkyExFloat, "Sky Exposure", ToolTips.skyExposure);
                        GUILayout.EndVertical();

                        GUILayout.BeginVertical("Box");
                        if (RGUI.Button(ToggleStateData.DisableStarFXToggle, "Disable Moving Stars"))
                        {
                            ToggleStateData.DisableStarFXToggle = !ToggleStateData.DisableStarFXToggle;
                        }
                        GUILayout.EndVertical();

                        GUILayout.EndVertical(); // End Night Layout
                    }

                    Tabs(Other_Tab, UIextensions.Instance.TabColorSwitch(Other_Tab));
                    if (!Other_Tab.isClosed)
                    {
                        GUILayout.BeginVertical("Box");
                        UIextensions.Instance.CenteredLabel("Other Settings");
                        Main.settings.AmbientLightFloat = RGUI.SliderFloat(Main.settings.AmbientLightFloat, 0f, 4500f, DefaultSettings.AmbientLightFloat, "Ambient Light", ToolTips.cycleAmbientLight);
                        Main.settings.VolWeightfloat = RGUI.SliderFloat(Main.settings.VolWeightfloat, 0.0f, 1.0f, Main.settings.DefaultVolWeight, "Cycle Weight", ToolTips.cycleVolWeight);
                        GUILayout.EndVertical();              
                    }

                    GUILayout.EndVertical(); // End of Light Layout
                }

                Tabs(Shadow_Tab, UIextensions.Instance.TabColorSwitch(Shadow_Tab));
                if (!Shadow_Tab.isClosed)
                {
                    GUILayout.BeginVertical(); // start shadow tab

                    GUILayout.BeginVertical("Box");
                    Main.settings.ShadowDistFloat = RGUI.SliderFloat(Main.settings.ShadowDistFloat, 1f, 285f, DefaultSettings.ShadowDistFloat, "Shadow Distance", ToolTips.cycleShadowDistance);
                    GUILayout.EndVertical();

                    GUILayout.BeginVertical("Box");
                    Main.settings.ShadowHighlights = RGUI.SliderFloat(Main.settings.ShadowHighlights, 0.0f, 0.2f, DefaultSettings.ShadowHighlights, "Shadow Highlights", ToolTips.cycleShadowHighLights);
                    GUILayout.EndVertical();

                    GUILayout.BeginVertical("Box");
                    UIextensions.Instance.CenteredLabel("Sun Shadows");
                    Main.settings.sunShadowFloat = RGUI.SliderFloat(Main.settings.sunShadowFloat, 0f, 1f, DefaultSettings.sunShadowFloat, "Shadow Strength", ToolTips.cycleShadowStrength);
                    GUILayout.EndVertical();

                    GUILayout.BeginVertical("Box");
                    UIextensions.Instance.CenteredLabel("Moon Shadows");
                    Main.settings.moonShadowFloat = RGUI.SliderFloat(Main.settings.moonShadowFloat, 0f, 1f, DefaultSettings.moonShadowFloat, "Shadow Strength", ToolTips.cycleShadowStrength);
                    GUILayout.EndVertical();    

                    GUILayout.EndVertical(); // end shadow tab
                }

                Tabs(Presets_Tab, UIextensions.Instance.TabColorSwitch(Presets_Tab));
                if (!Presets_Tab.isClosed)
                {
                    GUILayout.BeginHorizontal();
                    RGUI.BeginBackgroundColor(Color.cyan);
                    if (GUILayout.Button("Save Preset", RGUIStyle.button, GUILayout.Width(98)))
                    {
                        Main.presetManager.SavePreset();
                    }
                    Main.presetManager.PresetName = RGUI.Field(Main.presetManager.PresetName, "");
                    //GUILayout.FlexibleSpace();
                    RGUI.EndBackgroundColor();
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    RGUI.BeginBackgroundColor(Color.white);
                    if (GUILayout.Button("Apply Preset", RGUIStyle.button, GUILayout.Width(98)))
                    {
                        Main.presetManager.ApplyPreset();
                        Main.presetManager.ResetPresetList();
                    }
                    RGUI.EndBackgroundColor();
                    GUILayout.FlexibleSpace();
                    Main.presetManager.PresetToLoad = RGUI.SelectionPopup(Main.presetManager.PresetToLoad, Main.presetManager.GetPresetNames());
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();
                }

                GUILayout.EndVertical(); // end day night tabs

                if (Time.time - lastUpdate >= 0.2f)
                {
                    UpdateMinMax();
                    lastUpdate = Time.time;
                }
            }
        }
        private void UpdateMinMax()
        {
            float newsunMin = Mathf.Min(Main.settings.SunMinExFloat, Main.settings.SunMaxExFloat);
            float newsunMax = Mathf.Max(Main.settings.SunMinExFloat, Main.settings.SunMaxExFloat);
            float newmoonMin = Mathf.Min(Main.settings.MoonMinExFloat, Main.settings.MoonMaxExFloat);
            float newmoonMax = Mathf.Max(Main.settings.MoonMinExFloat, Main.settings.MoonMaxExFloat);

            float newsunIntensityMin = Mathf.Min(Main.settings.sunMinIntensity, Main.settings.sunMaxIntensity);
            float newsunIntensityMax = Mathf.Max(Main.settings.sunMinIntensity, Main.settings.sunMaxIntensity);

            Main.settings.SunMinExFloat = newsunMin;
            Main.settings.SunMaxExFloat = newsunMax;
            Main.settings.MoonMinExFloat = newmoonMin;
            Main.settings.MoonMaxExFloat = newmoonMax;

            Main.settings.sunMinIntensity = newsunIntensityMin;
            Main.settings.sunMaxIntensity = newsunIntensityMax;
        }

    }
}



