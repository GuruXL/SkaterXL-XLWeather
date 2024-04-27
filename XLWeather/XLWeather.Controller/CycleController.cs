using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.VFX.Utility;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using System;
using System.Collections;
using XLWeather.Data;
using XLWeather.Utils;

namespace XLWeather.Controller
{
    public class CycleController : MonoBehaviour
    {
        private bool IsDay;
        private Light cyclesunLight;
        private Light cyclemoonLight;
        private Light AmbientLight;
        public HDAdditionalLightData SunLightData;
        public HDAdditionalLightData MoonLightData;
        private Volume sunVolume;
        private Volume moonVolume;
        public VolumeProfile sunProfile;
        public VolumeProfile moonProfile;
        private PhysicallyBasedSky sunPBS;
        private PhysicallyBasedSky moonPBS;
        private Exposure sunExposure;
        private Exposure moonExposure;
        private IndirectLightingController sunLC;
        private IndirectLightingController moonLC;
        private HDShadowSettings sunShadow;
        private HDShadowSettings moonShadow;
        private ShadowsMidtonesHighlights shadowHighlights;
        private Transform starFx;
        
        private VisualEffect starFxComponent;
        private ExposedProperty StarFXlifetime = "LifeTime";
        private ExposedProperty StarFXrate = "Rate";

        private float StarFxLifeTimeFloat = 8f;
        private float StarFxRateFloat = 20f;
        private float SunflareFloat = 6f;
        private float SunIntensityFloat = 16000f;

        public string timeText = "";

        private DateTime currentTime;
        private TimeSpan sunriseTime;
        private TimeSpan sunsetTime;

        private void Start()
        {
            currentTime = DateTime.Now.Date + TimeSpan.FromHours(Main.settings.startHour);
            sunriseTime = TimeSpan.FromHours(Main.settings.sunriseHour);
            sunsetTime = TimeSpan.FromHours(Main.settings.sunsetHour);

            StartCoroutine(GetSunMoonLights());
            StartCoroutine(GetCycleVolumes());
            StartCoroutine(GetCycleVolComponents());
            StartCoroutine(GetCyleObjects());

            ResetDataSettings();
        }
        private void Update()
        {
            if (!ToggleStateData.DayNightToggle)
                return;

            CheckforTimechanges();
            UpdateProperties();

            if (cyclesunLight == null || cyclemoonLight == null)
                return;

            UpdateTimeOfDay();
            UpdateGameobjects();
            UpdateShadowSettings();
            RotateSun();
        }

        public DateTime GetCurrentTime()
        {
            return currentTime;
        }
        public void ResetCurrentTime()
        {
            currentTime = DateTime.Now.Date + TimeSpan.FromHours(DefaultSettings.startHour);
        }
        public bool GetIsDay()
        {
            return IsDay;
        }
        public float GetSunVolumePrio()
        {
            return sunVolume.priority;
        }
        public void SetCycleVolPrio(float prio)
        {
            sunVolume.priority = prio + 1;
            moonVolume.priority = prio + 1;
        }
        public void UpdateTimeOfDay()
        {
            currentTime = currentTime.AddSeconds(Time.deltaTime * Main.settings.timeMulipiler);

            if (timeText != null)
            {
                timeText = currentTime.ToString("HH:mm");
            }
        }
        private TimeSpan GetTimeDifference(TimeSpan fromTime, TimeSpan toTime)
        {
            TimeSpan diff = toTime - fromTime;

            if (diff.TotalSeconds < 0)
            {
                diff += TimeSpan.FromHours(24);
            }

            return diff;
        }
        private void RotateSun()
        {
            float sunRotation;
            float moonRotation;  
            //float moonSpaceRot;

            // if sun is out
            if (currentTime.TimeOfDay > sunriseTime && currentTime.TimeOfDay < sunsetTime)
            {
                IsDay = true;
                TimeSpan sunDuration = GetTimeDifference(sunriseTime, sunsetTime);
                TimeSpan timesinceSunrise = GetTimeDifference(sunriseTime, currentTime.TimeOfDay);
                TimeSpan timetoSunSet = GetTimeDifference(currentTime.TimeOfDay, sunsetTime);
           
                double percentage = timesinceSunrise.TotalMinutes / sunDuration.TotalMinutes;
                //double percentagex2 = timesinceSunrise.TotalMinutes / (sunDuration.TotalMinutes * 2);
                double FadeInTime = timesinceSunrise.TotalMinutes / (sunDuration.TotalMinutes / 4);
                double FadeoutTime = timetoSunSet.TotalMinutes / (sunDuration.TotalMinutes / 4);

                sunRotation = Mathf.Lerp(0, 180, (float)percentage);
                moonRotation = Mathf.Lerp(180, 360, (float)percentage);
                //sunSpaceRot = Mathf.Lerp(180, 360, (float)percentagex2);

                // controls fading in and out of effects at certain percentages of the cycle
                // Fade In
                if (percentage <= 0.2f)
                {
                    SunflareFloat = Mathf.Lerp(10, 5, (float)FadeInTime);
                }
                if (percentage <= 0.45f)
                {
                    SunIntensityFloat = Mathf.Lerp(Main.settings.sunMinIntensity, Main.settings.sunMaxIntensity, (float)FadeInTime);
                    Main.settings.sunSpaceEmission = Mathf.Lerp(0, 1, (float)FadeInTime);
                }
                if (percentage >= 0.6f)
                {
                    SunIntensityFloat = Mathf.Lerp(Main.settings.sunMinIntensity, Main.settings.sunMaxIntensity, (float)FadeoutTime);
                    Main.settings.sunSpaceEmission = Mathf.Lerp(0, 1, (float)FadeoutTime);
                }

                // Fade Out
                if (percentage >= 0.8f)
                {
                    SunflareFloat = Mathf.Lerp(10, 5, (float)FadeoutTime);
                }

            }
            // if moon is out
            else
            {
                IsDay = false;
                TimeSpan moonDuration = GetTimeDifference(sunsetTime, sunriseTime);
                TimeSpan timesinceSunset = GetTimeDifference(sunsetTime, currentTime.TimeOfDay);
                TimeSpan timetoSunrise = GetTimeDifference(currentTime.TimeOfDay, sunriseTime);

                double percentage = timesinceSunset.TotalMinutes / moonDuration.TotalMinutes;
                //double percentagex2 = timesinceSunset.TotalMinutes / (moonDuration.TotalMinutes * 2);
                double FadeInTime = timesinceSunset.TotalMinutes / (moonDuration.TotalMinutes / 4);
                double FadeoutTime = timetoSunrise.TotalMinutes / (moonDuration.TotalMinutes / 4);

                sunRotation = Mathf.Lerp(180, 360, (float)percentage);
                moonRotation = Mathf.Lerp(0, 180, (float)percentage);
                //moonSpaceRot = Mathf.Lerp(0, 360, (float)percentagex2);

                // controls fading in and out of effects at certain percentages of the cycle
                // fade in
                if (percentage <= 0.2f)
                {
                    // Fades starDome in
                    //StarOffsetFloat = Mathf.Lerp(0.95f, 0.8f, (float)FadeInTime);
                    StarFxLifeTimeFloat = Mathf.Lerp(0f, 10f, (float)FadeInTime);
                    StarFxRateFloat = Mathf.Lerp(0f, 25f, (float)FadeInTime);
                    Main.settings.moonSpaceEmission = Mathf.Lerp(0f, 1.8f, (float)FadeInTime);
                }
                // fade out
                if (percentage >= 0.8f)
                {
                    // Fades starDome Out
                    //StarOffsetFloat = Mathf.Lerp(0.95f, 0.8f, (float)FadeoutTime);
                    StarFxLifeTimeFloat = Mathf.Lerp(0f, 10f, (float)FadeoutTime);
                    StarFxRateFloat = Mathf.Lerp(0f, 25f, (float)FadeoutTime);
                    Main.settings.moonSpaceEmission = Mathf.Lerp(0f, 1.8f, (float)FadeoutTime);
                }
            }

            // This is for control of horizontal rotation

            float oldXrot = 0.0f;
            if (oldXrot != Main.settings.CycleXrotFloat)
            {
                cyclesunLight.transform.rotation = Quaternion.Euler(sunRotation, Main.settings.CycleXrotFloat, 0);
                cyclemoonLight.transform.rotation = Quaternion.Euler(moonRotation, Main.settings.CycleXrotFloat, 0);
                oldXrot = Main.settings.CycleXrotFloat;
            }
            else
            {
                cyclesunLight.transform.rotation = Quaternion.AngleAxis(sunRotation, Vector3.right);
                cyclemoonLight.transform.rotation = Quaternion.AngleAxis(moonRotation, Vector3.right);
            }

        }

        private CycleData.TimeSettings TimeSettings = new CycleData.TimeSettings(Main.settings.startHour, Main.settings.sunriseHour, Main.settings.sunsetHour);
        private void CheckforTimechanges()
        {
            // Update currentTime if the startHour has changed
            currentTime = TimeSettings.StartHour != Main.settings.startHour ? DateTime.Now.Date + TimeSpan.FromHours(Main.settings.startHour) : currentTime;
            TimeSettings.StartHour = Main.settings.startHour;

            // Update sunriseTime if the sunriseHour has changed
            sunriseTime = TimeSettings.SunRise != Main.settings.sunriseHour ? TimeSpan.FromHours(Main.settings.sunriseHour) : sunriseTime;
            TimeSettings.SunRise = Main.settings.sunriseHour;

            // Update sunsetTime if the sunsetHour has changed
            sunsetTime = TimeSettings.SunSet != Main.settings.sunsetHour ? TimeSpan.FromHours(Main.settings.sunsetHour) : sunsetTime;
            TimeSettings.SunSet = Main.settings.sunsetHour;
        }

        protected bool hasSkyUpdated = false;
        private void UpdateGameobjects()
        {
            // Set sun and moon active states based on whether it is day or night
            var sunActive = IsDay;
            var moonActive = !IsDay;
        
            cyclesunLight.enabled = sunActive ? true : cyclesunLight.enabled; // Enable/disable the sun light component based on the sun's active state
            cyclesunLight.gameObject.SetActive(sunActive); // Set the sun game object active state based on the sun's active state           
            cyclemoonLight.enabled = moonActive ? true : cyclemoonLight.enabled; // Enable/disable the moon light component based on the moon's active state          
            cyclemoonLight.gameObject.SetActive(moonActive); // Set the moon game object active state based on the moon's active state          
            sunVolume.gameObject.SetActive(sunActive); // Set the sun volume game object active state based on the sun's active state           
            moonVolume.gameObject.SetActive(moonActive); // Set the moon volume game object active state based on the moon's active state

            if (!ToggleStateData.DisableStarFXToggle)
            {
                starFx.gameObject.SetActive(!sunActive); // Set the starFx game object active state based on the opposite of the sun's active state
            }
            else if (ToggleStateData.DisableStarFXToggle && starFx.gameObject.activeSelf)
            {
                starFx.gameObject.SetActive(false);
            }

            if (moonActive && hasSkyUpdated)
            {
                hasSkyUpdated = false;
            }
            else if (sunActive && !hasSkyUpdated)
            {
                sunPBS.spaceEmissionTexture.Override(SetRandomSky());
                hasSkyUpdated = true;
            }

            //starDome.gameObject.SetActive(!sunActive);
        }

        private System.Random random = new System.Random();
        protected int currentskyindex = 0;
        private Cubemap SetRandomSky()
        {
            var sky1 = AssetHandler.Instance.cycleSkys[0];
            var sky2 = AssetHandler.Instance.cycleSkys[1];
            var sky3 = AssetHandler.Instance.cycleSkys[2];
            var sky4 = AssetHandler.Instance.cycleSkys[3];
            var sky5 = AssetHandler.Instance.cycleSkys[4];
            var sky6 = AssetHandler.Instance.cycleSkys[5];

            int randomNumber = random.Next(1, 7);

            // this is so the next sky is not the same as the current. there is probably a better way to do this.
            while (currentskyindex == randomNumber)
            {
                randomNumber = random.Next(1, 7);
            }

            currentskyindex = randomNumber;

            switch (randomNumber)
            {
                case 1:
                    return sky1;
                case 2:
                    return sky2;
                case 3:
                    return sky3;
                case 4:
                    return sky4;
                case 5:
                    return sky5;
                case 6:
                    return sky6;
                default:
                    // Handle unexpected random number
                    return sky1;
            }
        }
        // -------------- Get Components ------------------
        private Light[] cycleLights;
        private IEnumerator GetSunMoonLights()
        {
            yield return new WaitUntil(() => AssetHandler.Instance.IsPrefabsSpawned());

            cycleLights = AssetHandler.Instance.activeDayNight.GetComponentsInChildren<Light>();

            foreach (Light LightObj in cycleLights)
            {
                switch (LightObj.name)
                {
                    case"CycleSun":
                        cyclesunLight = LightObj;
                        SunLightData = cyclesunLight.GetComponent<HDAdditionalLightData>();
                        break;
                    case"CycleMoon":
                        cyclemoonLight = LightObj;
                        MoonLightData = cyclemoonLight.GetComponent<HDAdditionalLightData>();
                        MoonLightData.surfaceTexture = AssetHandler.Instance.MoonCookie;
                        cyclemoonLight.gameObject.SetActive(false);
                        break;
                    case"AmbientLight":
                        AmbientLight = LightObj;
                        break;
                }
            }

        }
        private Volume[] cycleVolumes;
        private IEnumerator GetCycleVolumes()
        {
            yield return new WaitUntil(() => AssetHandler.Instance.IsPrefabsSpawned());

            cycleVolumes = AssetHandler.Instance.activeDayNight.GetComponentsInChildren<Volume>();

            foreach (Volume VolObj in cycleVolumes)
            {
                switch (VolObj.name)
                {
                    case"SunSky Volume":
                        sunVolume = VolObj;
                        break;
                    case"MoonSky Volume":
                        moonVolume = VolObj;
                        moonVolume.gameObject.SetActive(false);
                        break;
                }
            }
        }
       
        private IEnumerator GetCycleVolComponents()
        {
            yield return new WaitUntil(() => AssetHandler.Instance.IsPrefabsSpawned());

            sunProfile = sunVolume.profile;
            moonProfile = moonVolume.profile;

            // get exposure
            sunProfile.TryGet(out sunExposure);
            if (!sunProfile.TryGet(out sunExposure))
            {
                sunExposure = sunProfile.Add<Exposure>();
            }
            sunExposure.SetAllOverridesTo(true);

            moonProfile.TryGet(out moonExposure);
            if (!moonProfile.TryGet(out moonExposure))
            {
                moonExposure = moonProfile.Add<Exposure>();
            }
            moonExposure.SetAllOverridesTo(true);

            // Get Physically based sky
            sunProfile.TryGet(out sunPBS);
            if (!sunProfile.TryGet(out sunPBS))
            {
                sunPBS = sunProfile.Add<PhysicallyBasedSky>();
            }
            moonProfile.TryGet(out moonPBS);
            if (!moonProfile.TryGet(out moonPBS))
            {
                moonPBS = moonProfile.Add<PhysicallyBasedSky>();
            }
            sunProfile.TryGet(out sunLC);
            if (!sunProfile.TryGet(out sunLC))
            {
                sunLC = sunProfile.Add<IndirectLightingController>();
            }
            moonProfile.TryGet(out moonLC);
            if (!moonProfile.TryGet(out moonLC))
            {
                moonLC = moonProfile.Add<IndirectLightingController>();
            }
            sunProfile.TryGet(out sunShadow);
            if (!sunProfile.TryGet(out sunShadow))
            {
                sunShadow = sunProfile.Add<HDShadowSettings>();
            }
            moonProfile.TryGet(out moonShadow);
            if (!moonProfile.TryGet(out moonShadow))
            {
                moonShadow = moonProfile.Add<HDShadowSettings>();
            }
            sunProfile.TryGet(out shadowHighlights);
            if (!sunProfile.TryGet(out shadowHighlights))
            {
                shadowHighlights = sunProfile.Add<ShadowsMidtonesHighlights>();
            }
        }

        private IEnumerator GetCyleObjects()
        {
            yield return new WaitUntil(() => AssetHandler.Instance.IsPrefabsSpawned());

            starFx = AssetHandler.Instance.activeDayNight.transform.Find("Stars");
            starFxComponent = starFx.GetComponent<VisualEffect>();
        }
        // ------------- End Get Components ---------------

        // -------------- Reset Data at script Start ----------
        private void ResetDataSettings()
        {
            TimeSettings = new CycleData.TimeSettings(0, 0, 0);
            ShadowSettings = new CycleData.ShadowSettings(0, 0, 0, 0);
            sunSettings = new CycleData.SunVolSettings(0, 0, 0, 0, 0, 0, 0, 0, 0);
            moonSettings = new CycleData.MoonVolSettings(0, 0, 0, 0, 0, 0, 0, 0, 0);
            sunLightSettings = new CycleData.SunLightSettings(0);
            moonLightSettings = new CycleData.MoonLightSettings(0, 0);
    }
        // ----------------------------------------------------

        // ------------- Set properties -------------
        private CycleData.ShadowSettings ShadowSettings = new CycleData.ShadowSettings(
            Main.settings.sunShadowFloat,
            Main.settings.moonShadowFloat,
            Main.settings.ShadowDistFloat,
            Main.settings.ShadowHighlights);
        private void UpdateShadowSettings()
        {
            if (sunProfile == null && moonProfile == null)
                return;

            if (sunShadow != null && moonShadow != null)
            {
                if (ShadowSettings.Distance != Main.settings.ShadowDistFloat)
                {
                    sunShadow.maxShadowDistance.Override(Main.settings.ShadowDistFloat);
                    moonShadow.maxShadowDistance.Override(Main.settings.ShadowDistFloat);
                    ShadowSettings.Distance = Main.settings.ShadowDistFloat;
                }
            }

            if (shadowHighlights != null)
            {
                if (ShadowSettings.Highlights != Main.settings.ShadowHighlights)
                {
                    Vector4 newShadowHighlight = new Vector4(
                        shadowHighlights.shadows.value.x,
                        shadowHighlights.shadows.value.y,
                        shadowHighlights.shadows.value.z,
                        Main.settings.ShadowHighlights);

                    shadowHighlights.shadows.Override(newShadowHighlight);
                    ShadowSettings.Highlights = Main.settings.ShadowHighlights;
                }
            }

            if (ShadowSettings.SunShadowStr != Main.settings.sunShadowFloat)
            {
                SunLightData.SetShadowDimmer(Main.settings.sunShadowFloat, 1f);
                ShadowSettings.SunShadowStr = Main.settings.sunShadowFloat;
            }

            if (ShadowSettings.MoonShadowStr != Main.settings.moonShadowFloat)
            {
                MoonLightData.SetShadowDimmer(Main.settings.moonShadowFloat, 1f);
                ShadowSettings.MoonShadowStr = Main.settings.moonShadowFloat;
            }
        }

        private CycleData.SunVolSettings sunSettings = new CycleData.SunVolSettings(
            Main.settings.SunMinExFloat,
            Main.settings.SunMaxExFloat,
            Main.settings.SunExCompFlt,
            Main.settings.sunSkyExFloat,
            Main.settings.sunSpaceEmission,
            Main.settings.sunDimmerFloat,
            Main.settings.SunIndirectLight,
            Main.settings.SunIndirectSpecular,
            Main.settings.sunAngularDiameter);

        private CycleData.MoonVolSettings moonSettings = new CycleData.MoonVolSettings(
            Main.settings.MoonMinExFloat,
            Main.settings.MoonMaxExFloat,
            Main.settings.MoonExCompFlt,
            Main.settings.moonSkyExFloat,
            Main.settings.moonSpaceEmission,
            Main.settings.moonDimmerFloat,
            Main.settings.MoonIndirectLight,
            Main.settings.MoonIndirectSpecular,
            Main.settings.moonAngularDiameter);

        private CycleData.SunLightSettings sunLightSettings = new CycleData.SunLightSettings(Main.settings.SunColorFloat);
        private CycleData.MoonLightSettings moonLightSettings = new CycleData.MoonLightSettings(Main.settings.MoonColorFloat, Main.settings.moonIntensity);

        private float oldAmbientLight = 400f;
        private void UpdateProperties()
        {
            if (cyclesunLight == null || cyclemoonLight == null)
                return;

            UpdateLightSettings();

            if (sunVolume == null || moonVolume == null)
                return;

            UpdateVolumeSettings();       
        }

        private void UpdateLightSettings()
        {
            // Sun
            if (cyclesunLight.gameObject?.activeSelf == true)
            {
                if (SunLightData.flareSize != SunflareFloat) // not stored becasue updated in rotate sun
                {
                    SunLightData.flareSize = SunflareFloat;
                }

                if (SunLightData.intensity != SunIntensityFloat) // not stored becasue updated in rotate sun
                {
                    SunLightData.intensity = SunIntensityFloat;
                }

                // sun color temp
                if (sunLightSettings.Color != Main.settings.SunColorFloat)
                {
                    cyclesunLight.colorTemperature = Main.settings.SunColorFloat;
                    sunLightSettings.Color = Main.settings.SunColorFloat;
                }
            }

            if (cyclemoonLight.gameObject?.activeSelf == true)
            {
                if (moonLightSettings.Intensity != Main.settings.moonIntensity)
                {
                    MoonLightData.intensity = Main.settings.moonIntensity;
                    moonLightSettings.Intensity = Main.settings.moonIntensity;
                }

                // moon color temp
                if (moonLightSettings.Color != Main.settings.MoonColorFloat)
                {
                    cyclemoonLight.colorTemperature = Main.settings.MoonColorFloat;
                    moonLightSettings.Color = Main.settings.MoonColorFloat;
                }
            }

            // Ambient Light
            if (AmbientLight != null)
            {
                if (oldAmbientLight != Main.settings.AmbientLightFloat)
                {
                    AmbientLight.intensity = Main.settings.AmbientLightFloat;
                    oldAmbientLight = Main.settings.AmbientLightFloat;
                }
            }
        }

        private void UpdateVolumeSettings()
        {
            // Sun volume exposure options
            if (sunVolume.gameObject?.activeSelf == true)
            {
                if (sunSettings.Min != Main.settings.SunMinExFloat)
                {
                    sunExposure.limitMin.Override(Main.settings.SunMinExFloat);
                    sunSettings.Min = Main.settings.SunMinExFloat;
                    UpdateMinMax();
                }
                if (sunSettings.Max != Main.settings.SunMaxExFloat)
                {
                    sunExposure.limitMax.Override(Main.settings.SunMaxExFloat);
                    sunSettings.Max = Main.settings.SunMaxExFloat;
                    UpdateMinMax();
                }

                if (sunSettings.SkyExposure != Main.settings.sunSkyExFloat)
                {
                    sunPBS.exposure.Override(Main.settings.sunSkyExFloat);
                    sunSettings.SkyExposure = Main.settings.sunSkyExFloat;
                }

                if (sunSettings.Compensation != Main.settings.SunExCompFlt)
                {
                    sunExposure.compensation.Override(Main.settings.SunExCompFlt);
                    sunSettings.Compensation = Main.settings.SunExCompFlt;
                }

                if (sunSettings.SpaceEmission != Main.settings.sunSpaceEmission)
                {
                    sunPBS.spaceEmissionMultiplier.Override(Main.settings.sunSpaceEmission);
                    sunSettings.SpaceEmission = Main.settings.sunSpaceEmission;
                }

                if (sunSettings.Dimmer != Main.settings.sunDimmerFloat)
                {
                    SunLightData.lightDimmer = Main.settings.sunDimmerFloat;
                    sunSettings.Dimmer = Main.settings.sunDimmerFloat;
                }

                if (sunSettings.IndirectLight != Main.settings.SunIndirectLight)
                {
                    sunLC.indirectDiffuseIntensity.Override(Main.settings.SunIndirectLight);
                    sunSettings.IndirectLight = Main.settings.SunIndirectLight;
                }
                if (sunSettings.SpecularLight != Main.settings.SunIndirectSpecular)
                {
                    sunLC.indirectSpecularIntensity.Override(Main.settings.SunIndirectSpecular);
                    sunSettings.SpecularLight = Main.settings.SunIndirectSpecular;
                }
                if (sunSettings.AngularDiameter != Main.settings.sunAngularDiameter)
                {
                    SunLightData.angularDiameter = Main.settings.sunAngularDiameter;
                    sunSettings.AngularDiameter = Main.settings.sunAngularDiameter;
                }
            }

            // moon
            if (moonVolume.gameObject?.activeSelf == true)
            {
                if (moonSettings.Min != Main.settings.MoonMinExFloat)
                {
                    moonExposure.limitMin.Override(Main.settings.MoonMinExFloat);
                    moonSettings.Min = Main.settings.MoonMinExFloat;
                    UpdateMinMax();
                }
                if (moonSettings.Max != Main.settings.MoonMaxExFloat)
                {
                    moonExposure.limitMax.Override(Main.settings.MoonMaxExFloat);
                    moonSettings.Max = Main.settings.MoonMaxExFloat;
                    UpdateMinMax();
                }

                if (moonSettings.SkyExposure != Main.settings.moonSkyExFloat)
                {
                    moonPBS.exposure.Override(Main.settings.moonSkyExFloat);
                    moonSettings.SkyExposure = Main.settings.moonSkyExFloat;
                }

                if (moonSettings.Compensation != Main.settings.MoonExCompFlt)
                {
                    moonExposure.compensation.Override(Main.settings.MoonExCompFlt);
                    moonSettings.Compensation = Main.settings.MoonExCompFlt;
                }

                if (moonSettings.SpaceEmission != Main.settings.moonSpaceEmission)
                {
                    moonPBS.spaceEmissionMultiplier.Override(Main.settings.moonSpaceEmission);
                    moonSettings.SpaceEmission = Main.settings.moonSpaceEmission;
                }

                if (moonSettings.Dimmer != Main.settings.moonDimmerFloat)
                {
                    MoonLightData.lightDimmer = Main.settings.moonDimmerFloat;
                    moonSettings.Dimmer = Main.settings.moonDimmerFloat;
                }

                if (moonSettings.IndirectLight != Main.settings.MoonIndirectLight)
                {
                    moonLC.indirectDiffuseIntensity.Override(Main.settings.MoonIndirectLight);
                    moonSettings.IndirectLight = Main.settings.MoonIndirectLight;
                }
                if (moonSettings.SpecularLight != Main.settings.MoonIndirectSpecular)
                {
                    moonLC.indirectSpecularIntensity.Override(Main.settings.MoonIndirectSpecular);
                    moonSettings.SpecularLight = Main.settings.MoonIndirectSpecular;
                }
                if (moonSettings.AngularDiameter != Main.settings.moonAngularDiameter)
                {
                   MoonLightData.angularDiameter = Main.settings.moonAngularDiameter;
                    moonSettings.AngularDiameter = Main.settings.moonAngularDiameter;
                }
            }
        }
        // --------------- End set Properites -----------------

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