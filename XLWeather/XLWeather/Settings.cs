using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityModManagerNet;
using XLWeather.Utils;
using XLWeather.Data;

namespace XLWeather
{
    [Serializable]
    public class Settings : UnityModManager.ModSettings, IDrawable
    {
        public static Settings Instance { get; set; }

        // ----- Start Set KeyBindings ------
        public KeyBinding Hotkey = new KeyBinding { keyCode = KeyCode.W };

        private static readonly KeyCode[] keyCodes = Enum.GetValues(typeof(KeyCode)).Cast<KeyCode>().Where(k => ((int)k < (int)KeyCode.Mouse0)).ToArray();

        public bool ctrlToggle = false;
        public bool altToggle = false;
        public bool noneToggle = true;

        // Get Key on KeyPress
        public static KeyCode? GetCurrentKeyDown()
        {
            if (!Input.anyKeyDown)
            {
                return null;
            }

            for (int i = 0; i < keyCodes.Length; i++)
            {
                KeyCode keyCode = keyCodes[i];

                if (keyCode == KeyCode.LeftControl ||
                    keyCode == KeyCode.RightControl ||
                    keyCode == KeyCode.LeftAlt ||
                    keyCode == KeyCode.RightAlt ||
                    keyCode == KeyCode.AltGr ||
                    keyCode == KeyCode.LeftCommand ||
                    keyCode == KeyCode.RightCommand)
                {
                    continue;
                }

                if (Input.GetKey(keyCode))
                {
                    return keyCode;
                }
            }

            return null;
        }
        public string GetAltKey()
        {
            if (ctrlToggle)
            {
                return "Ctrl + ";
            }
            else if (altToggle)
            {
                return "Alt + ";
            }
            return "";
        }

        // ----- End Set KeyBindings ------

        public Color BGColor = new Color(0.85f, 0.90f, 1.0f);

        public bool MapLayersToggle = false;

        public float NightFixedExposureFloat = DefaultSettings.NightFixedExposureFloat;
        public float NightSkyboxExposureFloat = DefaultSettings.NightSkyboxExposureFloat;
        public float NightRotateFloat = DefaultSettings.NightRotateFloat;
        public float SunSetSkyExposureFloat = DefaultSettings.SunSetSkyExposureFloat;
        public float SunSetSkyboxExposureFloat = DefaultSettings.SunSetSkyboxExposureFloat;
        public float SunSetRotateFloat = DefaultSettings.SunSetRotateFloat;
        public float BlueSkyExposureFloat = DefaultSettings.BlueSkyExposureFloat;
        public float BlueSkyboxExposureFloat = DefaultSettings.BlueSkyboxExposureFloat;
        public float BlueRotateFloat = DefaultSettings.BlueRotateFloat;

        public string NightSkyState = "Cloudy Night";
        public string SunSetSkyState = "Sun Set 1";
        public string BlueSkyState = "Blue Sky";

        public string DroneTargetState = "Skateboard";
        public string multiplayer_target = "None";

        public float timeMulipiler = DefaultSettings.timeMulipiler;
        public float startHour = DefaultSettings.startHour;
        public float sunriseHour = DefaultSettings.sunriseHour;
        public float sunsetHour = DefaultSettings.sunsetHour;

        public float sunMaxIntensity = DefaultSettings.sunMaxIntensity;
        public float sunMinIntensity = DefaultSettings.sunMinIntensity;
        public float moonIntensity = DefaultSettings.moonIntensity;
        public float sunShadowFloat = DefaultSettings.sunShadowFloat;
        public float moonShadowFloat = DefaultSettings.moonShadowFloat;
        public float ShadowDistFloat = DefaultSettings.ShadowDistFloat;
        public float ShadowHighlights = DefaultSettings.ShadowHighlights;
        public float SunMinExFloat = DefaultSettings.SunMinExFloat;
        public float SunMaxExFloat = DefaultSettings.SunMaxExFloat;
        public float SunExCompFlt = DefaultSettings.SunExCompFlt;
        public float MoonMinExFloat = DefaultSettings.MoonMinExFloat;
        public float MoonMaxExFloat = DefaultSettings.MoonMaxExFloat;
        public float MoonExCompFlt = DefaultSettings.MoonExCompFlt;
        public float sunSkyExFloat = DefaultSettings.sunSkyExFloat;
        public float moonSkyExFloat = DefaultSettings.moonSkyExFloat;
        public float CycleXrotFloat = DefaultSettings.CycleXrotFloat;
        public float sunDimmerFloat = DefaultSettings.sunDimmerFloat;
        public float moonDimmerFloat = DefaultSettings.moonDimmerFloat;
        public float SunColorFloat = DefaultSettings.SunColorFloat;
        public float MoonColorFloat = DefaultSettings.MoonColorFloat;
        public float AmbientLightFloat = DefaultSettings.AmbientLightFloat;
        public float sunSpaceEmission = DefaultSettings.sunSpaceEmission;
        public float moonSpaceEmission = DefaultSettings.moonSpaceEmission;
        public float SunIndirectLight = DefaultSettings.SunIndirectLight;
        public float SunIndirectSpecular = DefaultSettings.SunIndirectSpecular;
        public float sunAngularDiameter = DefaultSettings.sunAngularDiameter;
        public float MoonIndirectLight = DefaultSettings.MoonIndirectLight;
        public float MoonIndirectSpecular = DefaultSettings.MoonIndirectSpecular;
        public float moonAngularDiameter = DefaultSettings.moonAngularDiameter;

        public float DefaultVolWeight = 1f;
        public float VolWeightfloat = 1f;

        public float DefaultMapLightIntensity = 0f;
        public float MapLightIntensity = 0f;

        public float fogMeanFreePath = DefaultSettings.fogMeanFreePath;
        public float fogBaseHeight = DefaultSettings.fogBaseHeight;
        public float fogMaxDistance = DefaultSettings.fogMaxDistance;
        public float fogMaxHeight = DefaultSettings.fogMaxHeight;
        public float fogDensityDistance = DefaultSettings.fogDensityDistance;
        public float fogDensitySpeedX = DefaultSettings.fogDensitySpeedX;
        public float fogDensitySpeedY = DefaultSettings.fogDensitySpeedY;
        public float fogDensitySpeedZ = DefaultSettings.fogDensitySpeedZ;

        public float LeafDensityFloat = DefaultSettings.LeafDensityFloat;
        public float LeafSizeFloat = DefaultSettings.LeafSizeFloat;
        public float LeafGravityFloat = DefaultSettings.LeafGravityFloat;
        public float LeafWindFloat = DefaultSettings.LeafWindFloat;
        public float LeafLifeFloat = DefaultSettings.LeafLifeFloat;

        public float SnowDensityFloat = DefaultSettings.SnowDensityFloat;
        public float SnowSizeFloat = DefaultSettings.SnowSizeFloat;
        public float SnowGravityFloat = DefaultSettings.SnowGravityFloat;
        public float SnowWindFloat = DefaultSettings.SnowWindFloat;
        public float SnowLifeFloat = DefaultSettings.SnowLifeFloat;

        public float RainDensityFloat = DefaultSettings.RainDensityFloat;
        public float RainSizeFloat = DefaultSettings.RainSizeFloat;
        public float RainGravityFloat = DefaultSettings.RainGravityFloat;
        public float RainWindFloat = DefaultSettings.RainWindFloat;
        public float RainLifeFloat = DefaultSettings.RainLifeFloat;
        public float RainVolumeFloat = DefaultSettings.RainVolumeFloat;

        public float Cloud_ParallexOffset = DefaultSettings.Cloud_ParallexOffset;
        public float Cloud_Iterations = DefaultSettings.Cloud_Iterations;
        public float Cloud_NoiseScale = DefaultSettings.Cloud_NoiseScale;
        public float Cloud_NoiseDepth = DefaultSettings.Cloud_NoiseDepth;
        public float Cloud_CrackTiling_x = DefaultSettings.Cloud_CrackTiling_x;
        public float Cloud_CrackTiling_y = DefaultSettings.Cloud_CrackTiling_y;
        public float Cloud_Speed = DefaultSettings.Cloud_Speed;
        public float Cloud_BaseHeight = DefaultSettings.Cloud_BaseHeight;
        public float Cloud_Intensity = DefaultSettings.Cloud_Intensity;

        public float droneLtIntesityFlt = DefaultSettings.droneLtIntesityFlt;
        public float droneLtRangeFlt = DefaultSettings.droneLtRangeFlt;
        public float droneLtAngleFlt = DefaultSettings.droneLtAngleFlt;
        public float droneLtRadiusFlt = DefaultSettings.droneLtRadiusFlt;
        public float droneLtDimmerFlt = DefaultSettings.droneLtDimmerFlt;
        public Color DroneLightColor = DefaultSettings.DroneLightColor;

        public float droneRayDistance = DefaultSettings.droneRayDistance;
        public float dronefollowSharpness = DefaultSettings.dronefollowSharpness;
        public float droneRotSpeed = DefaultSettings.droneRotSpeed;
        public float droneVolume = DefaultSettings.droneVolume;
        public float droneCamFov = DefaultSettings.droneCamFov;

        public void ResetActiveobjs()
        {
            // Resets Toggles and Active prefabs
            bool settingsEnabled = ToggleStateData.ModEnabled;
            if (!settingsEnabled)
            {
                Main.settings.resetToggles();

                AssetHandler.Instance.activeVFX.Where(obj => obj.activeSelf).ToList().ForEach(obj => obj.SetActive(false));

                AssetHandler.Instance.activeSky.Where(obj => obj.activeSelf).ToList().ForEach(obj => obj.SetActive(false));

                if (AssetHandler.Instance.activeDayNight.activeSelf == true)
                {
                    AssetHandler.Instance.activeDayNight.SetActive(false);
                    Main.MapLightctrl.ToggleMainLights(true);
                }
                AssetHandler.Instance.activeDrone?.SetActive(false);
                AssetHandler.Instance.ActiveDensityVol?.SetActive(false);

                Main.MapLightctrl.ResetMainLight();
            }
        }
        public void ResetIfEnabled()
        {
            if (ToggleStateData.ModEnabled)
            {
                Main.MapLightctrl.ToggleMainLights(true);
                Main.settings.resetToggles();
            }
        }
        public void resetToggles()
        {
            ToggleStateData.ModEnabled = false;
            ToggleStateData.HotKeyToggle = false;
            ToggleStateData.NightSkyToggle = false;
            ToggleStateData.SunSetSkyToggle = false;
            ToggleStateData.BlueSkyToggle = false;
            ToggleStateData.FogToggle = false;
            ToggleStateData.removeFogToggle = false;
            ToggleStateData.addFogToggle = false;
            ToggleStateData.fogVolumetrics = false;
            ToggleStateData.DroneToggle = false;
            ToggleStateData.DroneCamtoggle = false;
            ToggleStateData.MultiTargetToggle = false;
            ToggleStateData.LeafvfxToggle = false;
            ToggleStateData.SnowvfxToggle = false;
            ToggleStateData.RainvfxToggle = false;
            ToggleStateData.AuroravfxToggle = false;
            ToggleStateData.LightningToggle = false;
            ToggleStateData.DayNightToggle = false;
            ToggleStateData.CloudToggle = false;
        }
        public void ResetAllSettings()
        {

            NightFixedExposureFloat = DefaultSettings.NightFixedExposureFloat;
            NightSkyboxExposureFloat = DefaultSettings.NightSkyboxExposureFloat;
            NightRotateFloat = DefaultSettings.NightRotateFloat;
            SunSetSkyExposureFloat = DefaultSettings.SunSetSkyExposureFloat;
            SunSetSkyboxExposureFloat = DefaultSettings.SunSetSkyboxExposureFloat;
            SunSetRotateFloat = DefaultSettings.SunSetRotateFloat;
            BlueSkyExposureFloat = DefaultSettings.BlueSkyExposureFloat;
            BlueSkyboxExposureFloat = DefaultSettings.BlueSkyboxExposureFloat;
            BlueRotateFloat = DefaultSettings.BlueRotateFloat;

            timeMulipiler = DefaultSettings.timeMulipiler;
            startHour = DefaultSettings.startHour;
            sunriseHour = DefaultSettings.sunriseHour;
            sunsetHour = DefaultSettings.sunsetHour;
            Main.Cyclectrl.ResetCurrentTime();

            sunMaxIntensity = DefaultSettings.sunMaxIntensity;
            sunMinIntensity = DefaultSettings.sunMinIntensity;
            moonIntensity = DefaultSettings.moonIntensity;
            sunShadowFloat = DefaultSettings.sunShadowFloat;
            moonShadowFloat = DefaultSettings.moonShadowFloat;
            ShadowDistFloat = DefaultSettings.ShadowDistFloat;
            ShadowHighlights = DefaultSettings.ShadowHighlights;
            SunMinExFloat = DefaultSettings.SunMinExFloat;
            SunMaxExFloat = DefaultSettings.SunMaxExFloat;
            SunExCompFlt = DefaultSettings.SunExCompFlt;
            MoonMinExFloat = DefaultSettings.MoonMinExFloat;
            MoonMaxExFloat = DefaultSettings.MoonMaxExFloat;
            MoonExCompFlt = DefaultSettings.MoonExCompFlt;
            sunSkyExFloat = DefaultSettings.sunSkyExFloat;
            moonSkyExFloat = DefaultSettings.moonSkyExFloat;
            CycleXrotFloat = DefaultSettings.CycleXrotFloat;
            sunDimmerFloat = DefaultSettings.sunDimmerFloat;
            moonDimmerFloat = DefaultSettings.moonDimmerFloat;
            SunColorFloat = DefaultSettings.SunColorFloat;
            MoonColorFloat = DefaultSettings.MoonColorFloat;
            AmbientLightFloat = DefaultSettings.AmbientLightFloat;
            sunSpaceEmission = DefaultSettings.sunSpaceEmission;
            moonSpaceEmission = DefaultSettings.moonSpaceEmission;
            SunIndirectLight = DefaultSettings.SunIndirectLight;
            SunIndirectSpecular = DefaultSettings.SunIndirectSpecular;
            MoonIndirectLight = DefaultSettings.MoonIndirectLight;
            MoonIndirectSpecular = DefaultSettings.MoonIndirectSpecular;

            fogMeanFreePath = DefaultSettings.fogMeanFreePath;
            fogBaseHeight = DefaultSettings.fogBaseHeight;
            fogMaxDistance = DefaultSettings.fogMaxDistance;
            fogMaxHeight = DefaultSettings.fogMaxHeight;
            fogDensityDistance = DefaultSettings.fogDensityDistance;
            fogDensitySpeedX = DefaultSettings.fogDensitySpeedX;
            fogDensitySpeedY = DefaultSettings.fogDensitySpeedY;
            fogDensitySpeedZ = DefaultSettings.fogDensitySpeedZ;

            LeafDensityFloat = DefaultSettings.LeafDensityFloat;
            LeafSizeFloat = DefaultSettings.LeafSizeFloat;
            LeafGravityFloat = DefaultSettings.LeafGravityFloat;
            LeafWindFloat = DefaultSettings.LeafWindFloat;
            LeafLifeFloat = DefaultSettings.LeafLifeFloat;

            SnowDensityFloat = DefaultSettings.SnowDensityFloat;
            SnowSizeFloat = DefaultSettings.SnowSizeFloat;
            SnowGravityFloat = DefaultSettings.SnowGravityFloat;
            SnowWindFloat = DefaultSettings.SnowWindFloat;
            SnowLifeFloat = DefaultSettings.SnowLifeFloat;

            RainDensityFloat = DefaultSettings.RainDensityFloat;
            RainSizeFloat = DefaultSettings.RainSizeFloat;
            RainGravityFloat = DefaultSettings.RainGravityFloat;
            RainWindFloat = DefaultSettings.RainWindFloat;
            RainLifeFloat = DefaultSettings.RainLifeFloat;
            RainVolumeFloat = DefaultSettings.RainVolumeFloat;

            Cloud_ParallexOffset = DefaultSettings.Cloud_ParallexOffset;
            Cloud_Iterations = DefaultSettings.Cloud_Iterations;
            Cloud_NoiseScale = DefaultSettings.Cloud_NoiseScale;
            Cloud_NoiseDepth = DefaultSettings.Cloud_NoiseDepth;
            Cloud_CrackTiling_x = DefaultSettings.Cloud_CrackTiling_x;
            Cloud_CrackTiling_y = DefaultSettings.Cloud_CrackTiling_y;
            Cloud_Speed = DefaultSettings.Cloud_Speed;
            Cloud_BaseHeight = DefaultSettings.Cloud_BaseHeight;
            Cloud_Intensity = DefaultSettings.Cloud_Intensity;

            droneLtIntesityFlt = DefaultSettings.droneLtIntesityFlt;
            droneLtRangeFlt = DefaultSettings.droneLtRangeFlt;
            droneLtAngleFlt = DefaultSettings.droneLtAngleFlt;
            droneLtRadiusFlt = DefaultSettings.droneLtRadiusFlt;
            droneLtDimmerFlt = DefaultSettings.droneLtDimmerFlt;

            droneRayDistance = DefaultSettings.droneRayDistance;
            dronefollowSharpness = DefaultSettings.dronefollowSharpness;
            droneRotSpeed = DefaultSettings.droneRotSpeed;
            droneVolume = DefaultSettings.droneVolume;
            droneCamFov = DefaultSettings.droneCamFov;
            DroneLightColor = DefaultSettings.DroneLightColor;

            VolWeightfloat = DefaultVolWeight;
            MapLightIntensity = DefaultMapLightIntensity;
        }

        public void OnChange()
        {
            throw new NotImplementedException();
        }
        public override void Save(UnityModManager.ModEntry modEntry)
        {
            Save(this, modEntry);
        }
    }
}
