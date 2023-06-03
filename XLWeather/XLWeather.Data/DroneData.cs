using UnityEngine;

namespace XLWeather.Data
{    
    public class DroneData
    {
        public class DroneLightSettings
        {
            public float Intensity { get; set; }
            public float Range { get; set; }
            public float Angle { get; set; }
            public float Radius { get; set; }
            public float Dimmer { get; set; }
            public Color LightColor { get; set; }

            // constructor to set initial values
            public DroneLightSettings(float intensity, float range, float angle, float radius, float dimmer, Color lightColor)
            {
                Intensity = intensity;
                Range = range;
                Angle = angle;
                Radius = radius;
                Dimmer = dimmer;
                LightColor = lightColor;
            }
        }
    }
}
