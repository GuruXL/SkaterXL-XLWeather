using System;
using UnityEngine;

namespace XLWeather.Presets
{
    [Serializable]
    public class PresetSettings
    {
        [SerializeField]
        public float timeMulipiler;
        [SerializeField]
        public float startHour;
        [SerializeField]
        public float sunriseHour;
        [SerializeField]
        public float sunsetHour;
        [SerializeField]
        public float sunMaxIntensity;
        [SerializeField]
        public float sunMinIntensity;
        [SerializeField]
        public float moonIntensity;
        [SerializeField]
        public float sunShadowFloat;
        [SerializeField]
        public float moonShadowFloat;
        [SerializeField]
        public float ShadowDistFloat;
        [SerializeField]
        public float ShadowHighlights;
        [SerializeField]
        public float SunMinExFloat;
        [SerializeField]
        public float SunMaxExFloat;
        [SerializeField]
        public float SunExCompFlt;
        [SerializeField]
        public float MoonMinExFloat;
        [SerializeField]
        public float MoonMaxExFloat;
        [SerializeField]
        public float MoonExCompFlt;
        [SerializeField]
        public float sunSkyExFloat;
        [SerializeField]
        public float moonSkyExFloat;
        [SerializeField]
        public float CycleXrotFloat;
        [SerializeField]
        public float sunDimmerFloat;
        [SerializeField]
        public float moonDimmerFloat;
        [SerializeField]
        public float SunColorFloat;
        [SerializeField]
        public float MoonColorFloat;
        [SerializeField]
        public float AmbientLightFloat;
        [SerializeField]
        public float sunSpaceEmission;
        [SerializeField]
        public float moonSpaceEmission;
        [SerializeField]
        public float SunIndirectLight;
        [SerializeField]
        public float SunIndirectSpecular;
        [SerializeField]
        public float sunAngularDiameter;
        [SerializeField]
        public float MoonIndirectLight;
        [SerializeField]
        public float MoonIndirectSpecular;
        [SerializeField]
        public float moonAngularDiameter;
        [SerializeField]
        public float VolWeightfloat;
    }
}
