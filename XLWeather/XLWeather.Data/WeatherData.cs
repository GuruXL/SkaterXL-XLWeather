using UnityEngine.VFX.Utility;
using UnityEngine;

namespace XLWeather.Data
{
    public class WeatherData
    {
        public class LeafSettings
        {
            public float Density { get; set; }
            public float AreaSize { get; set; }
            public float Gravity { get; set; }
            public float Wind { get; set; }
            public float Lifetime { get; set; }

            // constructor to set initial values
            public LeafSettings(float density, float area, float gravity, float wind, float lifetime)
            {
                Density = density;
                AreaSize = area;
                Gravity = gravity;
                Wind = wind;
                Lifetime = lifetime;
            }
        }
        public class RainSettings
        {
            public float Density { get; set; }
            public float AreaSize { get; set; }
            public float Gravity { get; set; }
            public float Wind { get; set; }
            public float Lifetime { get; set; }

            // constructor to set initial values
            public RainSettings(float density, float area, float gravity, float wind, float lifetime)
            {
                Density = density;
                AreaSize = area;
                Gravity = gravity;
                Wind = wind;
                Lifetime = lifetime;
            }
        }

        public class SnowSettings
        {
            public float Density { get; set; }
            public float AreaSize { get; set; }
            public float Gravity { get; set; }
            public float Wind { get; set; }
            public float Lifetime { get; set; }

            // constructor to set initial values
            public SnowSettings(float density, float area, float gravity, float wind, float lifetime)
            {
                Density = density;
                AreaSize = area;
                Gravity = gravity;
                Wind = wind;
                Lifetime = lifetime;
            }
        }

        public class FogSettings
        {
            public float FogMFP { get; set; }
            public float FogBH { get; set; }
            public float FogMD { get; set; }
            public float FogMH { get; set; }

            // constructor to set initial values
            public FogSettings(float fogMFP, float fogBH, float fogMD, float fogMH)
            {
                FogMFP = fogMFP;
                FogBH = fogBH;
                FogMD = fogMD;
                FogMH = fogMH;
            }
        }

        public class CloudSettings
        {
            public float ParallexOffset { get; set; }
            public float Parallex2 { get; set; }
            public float Iterations { get; set; }
            public float NoiseScale { get; set; }
            public float NoiseDepth { get; set; }
            public Vector3 CrackTiling { get; set; }
            public float Speed { get; set; }
            public float Intensity { get; set; }

            // constructor to set initial values
            public CloudSettings(float parallexOffset, float parallex2, float iterations, float noiseScale, float noiseDepth, Vector3 crackTiling, float speed, float intensity)
            {
                ParallexOffset = parallexOffset;
                Parallex2 = parallex2;
                Iterations = iterations;
                NoiseScale = noiseScale;
                NoiseDepth = noiseDepth;
                CrackTiling = crackTiling;
                Speed = speed;
                Intensity = intensity;
            }
        }
    }
}