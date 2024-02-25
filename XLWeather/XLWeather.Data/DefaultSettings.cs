using UnityEngine;
using XLWeather;

namespace XLWeather.Data
{
    public static class DefaultSettings
    {
        public static float NightFixedExposureFloat = 11.8f;
        public static float NightSkyboxExposureFloat = 11.25f;
        public static float NightRotateFloat = 0f;
        public static float SunSetSkyExposureFloat = 11f;
        public static float SunSetSkyboxExposureFloat = 10.8f;
        public static float SunSetRotateFloat = 0f;
        public static float BlueSkyExposureFloat = 9.8f;
        public static float BlueSkyboxExposureFloat = 11f;
        public static float BlueRotateFloat = 0f;

        public static float timeMulipiler = 100f;
        public static float startHour = 7f;
        public static float sunriseHour = 6.5f;
        public static float sunsetHour = 20.5f;
        public static float sunMaxIntensity = 18500f;
        public static float sunMinIntensity = 14000f;
        public static float moonIntensity = 2500f;
        public static float sunShadowFloat = 1f;
        public static float moonShadowFloat = 0.75f;
        public static float ShadowDistFloat = 145f;
        public static float ShadowHighlights = 0.04f;
        public static float SunMinExFloat = 9.8f;
        public static float SunMaxExFloat = 10.85f;
        public static float SunExCompFlt = 0.42f;
        public static float MoonMinExFloat = 10.6f;
        public static float MoonMaxExFloat = 15f;
        public static float MoonExCompFlt = 0.42f;
        public static float sunSkyExFloat = 1f;
        public static float moonSkyExFloat = -0.6f;
        public static float CycleXrotFloat = 0f;
        public static float sunDimmerFloat = 1.0f;
        public static float moonDimmerFloat = 1.0f;
        public static float SunColorFloat = 5845f;
        public static float MoonColorFloat = 7834f;
        public static float AmbientLightFloat = 400f;
        public static float sunSpaceEmission = 0f;
        public static float moonSpaceEmission = 0f;
        public static float SunIndirectLight = 1.0f;
        public static float SunIndirectSpecular = 1.25f;
        public static float sunAngularDiameter = 0.6f;
        public static float MoonIndirectLight = 0.75f;
        public static float MoonIndirectSpecular = 0.4f;
        public static float moonAngularDiameter = 7.2f;

        public static float fogMeanFreePath = 100f;
        public static float fogBaseHeight = 20f;
        public static float fogMaxDistance = 200f;
        public static float fogMaxHeight = 800f;
        public static float fogDensityDistance = 20f;
        public static float fogDensitySpeedX = 0.1f;
        public static float fogDensitySpeedY = 0.08f;
        public static float fogDensitySpeedZ = 0.1f;

        public static float LeafDensityFloat = 120f;
        public static float LeafSizeFloat = 80f;
        public static float LeafGravityFloat = 3f;
        public static float LeafWindFloat = 2f;
        public static float LeafLifeFloat = 5f;

        public static float SnowDensityFloat = 20000f;
        public static float SnowSizeFloat = 60f;
        public static float SnowGravityFloat = 2f;
        public static float SnowWindFloat = 0.4f;
        public static float SnowLifeFloat = 8f;

        public static float RainDensityFloat = 120000f;
        public static float RainSizeFloat = 25f;
        public static float RainGravityFloat = 35f;
        public static float RainWindFloat = 1.2f;
        public static float RainLifeFloat = 0.4f;
        public static float RainVolumeFloat = 0.4f;

        public static float Cloud_ParallexOffset = -32f;
        public static float Cloud_Iterations = 7.8f;
        public static float Cloud_NoiseScale = 1.8f;
        public static float Cloud_NoiseDepth = 1.2f;
        public static float Cloud_CrackTiling_x = 1f;
        public static float Cloud_CrackTiling_y = 1f;
        public static float Cloud_Speed = 0.01f;
        public static float Cloud_BaseHeight = 120f;
        public static float Cloud_Intensity = 10f;

        public static float droneLtIntesityFlt = 20f;
        public static float droneLtRangeFlt = 100f;
        public static float droneLtAngleFlt = 60f;
        public static float droneLtRadiusFlt = 0.4f;
        public static float droneLtDimmerFlt = 10f;

        public static float droneRayDistance = 8;
        public static float dronefollowSharpness = 0.01f;
        public static float droneRotSpeed = 2f;
        public static float droneVolume = 0.2f;
        public static float droneCamFov = 80f;
        public static Color DroneLightColor = new Color(1.0f, 1.0f, 1.0f);
    }
}