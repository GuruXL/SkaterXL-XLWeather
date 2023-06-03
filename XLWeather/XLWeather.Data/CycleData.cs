﻿using XLWeather;

namespace XLWeather.Data
{
    public class CycleData
    {
        public class TimeSettings
        {
            public float StartHour { get; set; }
            public float SunRise { get; set; }
            public float SunSet { get; set; }

            public TimeSettings(float startHour, float sunRise, float sunSet)
            {
                StartHour = startHour;
                SunRise = sunRise;
                SunSet = sunSet;
            }
        }
        public class SunVolSettings
        {
            public float Min { get; set; }
            public float Max { get; set; }
            public float SkyExposure { get; set; }
            public float SpaceEmission { get; set; }
            public float Dimmer { get; set; }
            public float IndirectLight { get; set; }
            public float SpecularLight { get; set; }

            public SunVolSettings(float min, float max, float skyExposure, float emission, float dimmer, float indirectLight, float specularLight)
            {
                Min = min;
                Max = max;
                SkyExposure = skyExposure;
                SpaceEmission = emission;
                Dimmer = dimmer;
                IndirectLight = indirectLight;
                SpecularLight = specularLight;
            }
        }
        public class SunLightSettings
        {
            public float Color { get; set; }

            public SunLightSettings(float color)
            {
                Color = color;
            }
        }

        public class MoonVolSettings
        {
            public float Min { get; set; }
            public float Max { get; set; }
            public float SkyExposure { get; set; }
            public float SpaceEmission { get; set; }
            public float Dimmer { get; set; }
            public float IndirectLight { get; set; }
            public float SpecularLight { get; set; }

            public MoonVolSettings(float min, float max, float skyExposure, float emission, float dimmer, float indirectLight, float specularLight)
            {
                Min = min;
                Max = max;
                SkyExposure = skyExposure;
                SpaceEmission = emission;
                Dimmer = dimmer;
                IndirectLight = indirectLight;
                SpecularLight = specularLight;
            }
        }

        public class MoonLightSettings
        {
            public float Color { get; set; }
            public float Intensity { get; set; }

            public MoonLightSettings(float color, float intensity)
            {
                Color = color;
                Intensity = intensity;
            }
        }
        public class ShadowSettings
        {
            public float SunShadowStr { get; set; }
            public float MoonShadowStr { get; set; }
            public float Distance { get; set; }
            public float Highlights { get; set; }

            public ShadowSettings(float sunShadowStr, float moonShadowStr, float distance, float highlights)
            {
                SunShadowStr = sunShadowStr;
                MoonShadowStr = moonShadowStr;
                Distance = distance;
                Highlights = highlights;
            }
        }
    }
}