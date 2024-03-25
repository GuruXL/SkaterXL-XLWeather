namespace XLWeather.UI
{
    public static class ToolTips
    {
        public static readonly string audioVolume = "Loudness of the audio. Range: 0 (mute) to 1 (max volume).";

        // Skies
        public static readonly string exposure = "Brightness of the scene. Increase for darker, decrease for brighter.";
        public static readonly string skyExposure = "Brightness of the skybox. Increase for brighter, decrease for darker.";
        public static readonly string rotation = "Controls the skybox orientation. Range: 0 to 360 degrees.";
        public static readonly string defaultLightIntensity = "Brightness of the main directional light (Sun). Increase for more intensity, decrease for less.";

        // Fog
        public static readonly string fogDistance = "Adjusts fog thickness. Decrease distance for thicker fog, increase for clearer view.";
        public static readonly string fogHeight = "Sets the starting height of fog. Increase to raise fog level, decrease to lower it.";
        public static readonly string fogMaxDistance = "Max Distance the fog extends to. Decrease for denser near fog, increase to extend visibility.";
        public static readonly string fogMaxHeight = "Determines the upper limit of fog. Increase for higher fog coverage, decrease to lower fog ceiling.";
        public static readonly string fogVolDistance = "Controls how quickly fog thickens with distance. Lower values result in thicker fog closer to the viewer.";

        // VFX
        public static readonly string VFXDensity = "The amount of particles to be spawned.";
        public static readonly string VFXArea = "The distance around the player that the particles will spawn.";
        public static readonly string VFXGravity = "The speed and force that particles will fall.";
        public static readonly string VFXWind = "The amount of wind force that gets applied to the particles.";
        public static readonly string VFXLifetime = "Determines how long each particle will exist before disappearing.";

        // Clouds
        public static readonly string cloudBaseHeight = "Sets the starting height of clouds. Increase for more height, decrease for lower clouds.";
        public static readonly string cloudParallax = "Shifts cloud layers to create depth. Higher negative values increase depth effect.";
        public static readonly string cloudIterations = "Controls how many cloud layers will be visible. Increase for more layers, decrease for fewer.";
        public static readonly string cloudNoiseScale = "Adjusts the scaling of the clouds. Increase for smaller clouds, decrease for bigger.";
        public static readonly string cloudNoiseDepth = "Controls the thickness of cloud cover. Increase for thicker clouds, decrease for thinner.";
        public static readonly string cloudTiling = "Controls cloud spacing. Increase to compress, decrease to spread out.";
        public static readonly string cloudSpeed = "Controls cloud speed. Increase for faster, decrease for slower movement.";
        public static readonly string cloudIntensity = "Controls cloud brightness. Increase for brighter clouds, decrease for darker clouds.";

        // Drone
        public static readonly string droneCamFOV = "Sets the field of view angle. Increase for wider view, decrease for narrower.";
        public static readonly string droneHeight = "The elevation of the drone. Increase for higher, decrease for lower.";
        public static readonly string droneRotationSpeed = "The speed at which the drone will rotate to face the target. Increase for more responsiveness, decrease for more delay.";
        public static readonly string droneFollowSharpness = "The speed at which the drone will follow the target. Increase for more responsiveness, decrease for more delay.";

        public static readonly string droneLightIntensity = "Brightness of the drone spotlight. Increase for more intensity, decrease for less.";
        public static readonly string droneLightRange = "The distance the light will reach. Increase for more distance, decrease for less.";
        public static readonly string droneLightAngle = "Adjusts the spread angle of the light. Increase for wider, decrease for narrower.";
        public static readonly string droneLightRadius = "Adjusts light edge softness, affecting how sharply it fades at the edges.";
        public static readonly string droneLightDimmer = "Adjusts light's overall intensity without changing its range. Lower for softer light, higher for brighter.";

        // Day/Night Cycle
        public static readonly string cycleSize = "Sets the size of the light source. Increase for bigger, decrease for smaller.";
        public static readonly string cycleSpeed = "Controls the speed of the day/night transition. Increase for faster cycle, decrease for slower.";
        public static readonly string cycleStartTime = "Sets the initial time for the day/night cycle. Adjust to start at a specific time of day.";
        public static readonly string cycleSunRiseTime = "Sets the time of day for sunrise. Adjust to change what time of day the sun rises.";
        public static readonly string cycleSunSetTime = "Sets the time of day for sunset. Adjust to change what time of day the sun sets.";
        public static readonly string cycleRotation = "Controls the horizontal orientation of the sun/moon. Range: 0 to 360 degrees.";

        public static readonly string cycleMinIntensity = "Sets the intensity of the sun during sunrise and sunset. Increase for more intensity, decrease for less.";
        public static readonly string cycleMaxIntensity = "Sets the intensity of the sun during noon (when the sun is directly overhead). Increase for more intensity, decrease for less.";
        public static readonly string cycleMoonIntensity = "Sets the intensity of the moon. Increase for more intensity, decrease for less.";
        public static readonly string cycleDimmer = "Adjusts light's overall intensity without changing its range. Lower for softer light, higher for brighter.";

        public static readonly string cycleSunColor = "Adjusts the sun's color. Increase for blue cooler tones, decrease for red warmer tones.";
        public static readonly string cycleMoonColor = "Adjusts the moon's color. Increase for blue cooler tones, decrease for red warmer tones.";

        public static readonly string cycleIndirectLight = "Scales the brightness of global lighting from lightmaps and Light Probes. Increase to brighten, decrease to dim.";
        public static readonly string cycleSpecularLight = "Adjusts the brightness of Reflection Probes. Increase to enhance, decrease to reduce.";
        public static readonly string cycleAmbientLight = "Brightness of the ambient light. Increase for more intensity, decrease for less.";

        public static readonly string cycleMinExposure = "Sets the minimum brightness level for exposure. Lower values are birghter, higher values are darker.";
        public static readonly string cycleMaxExposure = "Sets the maximum brightness level for exposure. Lower values are birghter, higher values are darker.";
        public static readonly string cycleCompensation = "Adjustment for exposure to make scenes brighter or darker. Increase to brighten, decrease to darken.";

        public static readonly string cycleVolWeight = "Sets the influence level of the default post-processing effects. Increase for more effect, decrease for less.";

        public static readonly string cycleShadowDistance = "Sets how far shadows are cast. Increase for distant shadows, decrease for shorter range. (Less range will result in higher quality shadows)";
        public static readonly string cycleShadowHighLights = "Adjusts the brightness of highlighted areas in shadows. Increase to lighten, decrease to darken highlights.";
        public static readonly string cycleShadowStrength = "Sets the opacity of shadows. Increase for darker shadows, decrease for lighter shadows.";
    }
}
