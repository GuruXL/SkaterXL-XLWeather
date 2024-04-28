using UnityEngine.VFX.Utility;
using UnityEngine;

namespace XLWeather.Data
{
    public class SkyData
    {
        public class NightSkySettings
        {
            public float Exposure { get; set; }
            public float Skyexposure { get; set; }
            public float Rotation { get; set; }
            public float IndirectDiffuse { get; set; }
            public float IndirectSpecular { get; set; }

            // constructor to set initial values
            public NightSkySettings(float exposure, float skyexposure, float roation, float indirectdiffuse, float indirectspecular)
            {
                Exposure = exposure;
                Skyexposure = skyexposure;
                Rotation = roation;
                IndirectDiffuse = indirectdiffuse;
                IndirectSpecular = indirectspecular;
            }
        }

        public class SunSetSkySettings
        {
            public float Exposure { get; set; }
            public float Skyexposure { get; set; }
            public float Rotation { get; set; }
            public float IndirectDiffuse { get; set; }
            public float IndirectSpecular { get; set; }

            // constructor to set initial values
            public SunSetSkySettings(float exposure, float skyexposure, float roation, float indirectdiffuse, float indirectspecular)
            {
                Exposure = exposure;
                Skyexposure = skyexposure;
                Rotation = roation;
                IndirectDiffuse = indirectdiffuse;
                IndirectSpecular = indirectspecular;
            }
        }

        public class BlueSkySettings
        {
            public float Exposure { get; set; }
            public float Skyexposure { get; set; }
            public float Rotation { get; set; }
            public float IndirectDiffuse { get; set; }
            public float IndirectSpecular { get; set; }

            // constructor to set initial values
            public BlueSkySettings(float exposure, float skyexposure, float roation, float indirectdiffuse, float indirectspecular)
            {
                Exposure = exposure;
                Skyexposure = skyexposure;
                Rotation = roation;
                IndirectDiffuse = indirectdiffuse;
                IndirectSpecular = indirectspecular;
            }
        }

    }
}