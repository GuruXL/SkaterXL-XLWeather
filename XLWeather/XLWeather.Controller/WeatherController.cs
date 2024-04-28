using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.VFX.Utility;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using Cinemachine;
using GameManagement;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System;
using System.Collections;
using XLWeather.Utils;
using XLWeather.Data;
using ModIO.UI;
using System.Linq;

namespace XLWeather.Controller
{
    public class WeatherController : MonoBehaviour
    {
        public MeshRenderer cloudRenderer;
        public HDRISky[] activeHDRI = new HDRISky[3];
        private Volume[] volume = new Volume[5];
        private VolumeProfile[] profile = new VolumeProfile[4];
        public Exposure[] exposure = new Exposure[4];
        public IndirectLightingController[] indirectLight = new IndirectLightingController[3];
        public Fog CustomFog;
        private DensityVolume fogDensityVol;
        public Type state;
        //private int delay = 0;
        //public string scene = "";
        //private bool sceneChanged = false;
        private Transform MasterPrefab;
        private Transform replay_skater;
        private Transform Replay;
        public Transform pelvis;
        public Transform pelvis_replay;
        public Transform board_replay;
        public Transform board;
        private Transform mainCam;
        private CinemachineBrain camBrain;
        public Transform camBrainPos;

        private delegate void UpdateSettings();
        private UpdateSettings[] updateFunctions;

        private void Awake()
        {
            MasterPrefab = GameStateMachine.Instance.transform;

            //MasterPrefab = PlayerController.Instance.skaterController.transform.parent.transform.parent; // 1.2.2.8
            //MasterPrefab = PlayerController.Main.transform.parent; //1.2.6.0
            Main.settings.ResetIfEnabled();
        }

        public void Start()
        {
            StartCoroutine(GetVfxComponent());
            StartCoroutine(GetVolumeComponent());

            ResetDataSettings();
           
            updateFunctions = new UpdateSettings[]
            {
                new UpdateSettings(UpdateNightSkySettings),
                new UpdateSettings(UpdateSunSetSkySettings),
                new UpdateSettings(UpdateBlueSkySettings),
                new UpdateSettings(UpdateLeafSettings),
                new UpdateSettings(UpdateRainSettings),
                new UpdateSettings(UpdateSnowSettings),
                new UpdateSettings(UpdateCloudSettings),
                new UpdateSettings(UpdateFogSettings)
            };

            GetPlayerTarget();

            if (MasterPrefab != null)
            {
                replay_skater = GetReplayTarget();
                GetMainCamera();
            }
        }
        bool hasReset;
        private void Update()
        {
            UpdateState();
            UpdateFunctions();

            if (ToggleStateData.DayNightToggle)
            {
                //UpdateShadowSettings();
                UpdateVolSettings();
                if (hasReset)
                {
                    hasReset = false;
                }
            }
            else if (!ToggleStateData.DayNightToggle && !hasReset)
            {
                ResetVolWeight();
                if (!hasReset)
                {
                    hasReset = true;
                }
            }
        }

        public void FixedUpdate()
        {
            if (!ToggleStateData.ModEnabled)
                return;

            AttachVFX();
            AttachVol();
        }

        private void UpdateFunctions()
        {
            bool[] toggles = new bool[] {
                ToggleStateData.NightSkyToggle,
                ToggleStateData.SunSetSkyToggle,
                ToggleStateData.BlueSkyToggle,
                ToggleStateData.LeafvfxToggle,
                ToggleStateData.RainvfxToggle,
                ToggleStateData.SnowvfxToggle,
                ToggleStateData.CloudToggle,
                ToggleStateData.FogToggle };

            for (int i = 0; i < updateFunctions.Length; i++)
            {
                if (toggles[i])
                {
                    //ResetDataSettings();
                    updateFunctions[i]?.Invoke();
                }
            }
        }
        public void UpdateState()
        {
            // Gets the Game State name
            var currentstate = GameStateMachine.Instance.CurrentState.GetType();
            if (currentstate != state)
            {
                state = currentstate;
            }
        }
       
        // --------- Start of Get Target functions ----------
        private Transform GetReplayTarget()
        {
            //Transform main = PlayerController.Instance.skaterController.transform.parent.transform.parent;
            Replay = MasterPrefab.Find("ReplayEditor");
            Transform playback = Replay.Find("Playback Skater Root");
            Transform skater = playback.Find("NewSkater");
            Transform joints = skater.Find("Skater_Joints");
            pelvis_replay = joints.FindChildRecursively("Skater_pelvis");
            board_replay = playback.Find("Skateboard");

            return skater;
        }
        private void GetPlayerTarget()
        {
            //Transform parent = PlayerController.Instance.skaterController.gameObject.transform; // 1.2.2.8
            Transform parent = PlayerController.Main.gameplay.skaterController.gameObject.transform; // 1.2.7.8
            Transform joints = parent.Find("Skater_Joints");
            pelvis = joints.FindChildRecursively("Skater_pelvis");

            //board = PlayerController.Instance.boardController.boardTransform; // 1.2.2.8
            board = PlayerController.Main.gameplay.boardController.gameObject.transform; // 1.2.7.8
        }
        private void GetMainCamera()
        {
            mainCam = MasterPrefab.Find("Main Camera");
            camBrain = mainCam.GetComponent<CinemachineBrain>();
        }

        private void AttachVFX() // Attaches Active VFX to active ccinemachine camera.
        {
            //var bailed = PlayerController.Instance.currentStateEnum == PlayerController.CurrentState.Bailed;
            var camBrainPos = camBrain.ActiveVirtualCamera.VirtualCameraGameObject.transform;

            if (AssetHandler.Instance.activeVFX == null)
                return;

            List<GameObject> activeVFX = AssetHandler.Instance.activeVFX.ToList();

            for (int i = 0; i < activeVFX.Count; i++)
            {
                GameObject activeVFXObj = activeVFX[i];

                if (!activeVFXObj.activeSelf)
                    continue;

                if (activeVFXObj.name.Contains("CloudPlane"))
                {
                    activeVFXObj.transform.position = new Vector3(camBrainPos.position.x, Main.settings.Cloud_BaseHeight, camBrainPos.position.z);
                }
                else
                {
                    activeVFXObj.transform.position = camBrainPos.position;
                }
            }
        }

        private void AttachVol()
        {
            //var bailed = PlayerController.Instance.currentStateEnum == PlayerController.CurrentState.Bailed;
            var camBrainPos = camBrain.ActiveVirtualCamera.VirtualCameraGameObject.transform;

            if (AssetHandler.Instance.ActiveDensityVol == null && !AssetHandler.Instance.ActiveDensityVol.activeSelf)
                return;

            AssetHandler.Instance.ActiveDensityVol.transform.position = camBrainPos.position;
        }

        // --------- End of Get Target functions ----------

        // --------- start Get Components ---------------
        private IEnumerator GetVolumeComponent()
        {
            yield return new WaitUntil(() => AssetHandler.Instance.IsPrefabsSpawned());
            yield return new WaitForFixedUpdate();

            volume[0] = AssetHandler.Instance.activeSky[0].GetComponent<Volume>();  // Night Sky
            volume[1] = AssetHandler.Instance.activeSky[1].GetComponent<Volume>();  // SunSet Sky
            volume[2] = AssetHandler.Instance.activeSky[2].GetComponent<Volume>();  // Blue Sky
            volume[3] = AssetHandler.Instance.activeSky[3].GetComponent<Volume>();  // Fog Remove

            // gets Volumes profile
            profile[0] = volume[0].profile;
            profile[1] = volume[1].profile;
            profile[2] = volume[2].profile;
            profile[3] = volume[3].profile;

            // gets exposure
            profile[0].TryGet(out exposure[0]);
            if (!profile[0].TryGet(out exposure[0]))
            {
                exposure[0] = profile[0].Add<Exposure>();
            }
            profile[1].TryGet(out exposure[1]);
            if (!profile[1].TryGet(out exposure[1]))
            {
                exposure[1] = profile[1].Add<Exposure>();
            }
            profile[2].TryGet(out exposure[2]);
            if (!profile[2].TryGet(out exposure[2]))
            {
                exposure[2] = profile[2].Add<Exposure>();
            }
            // Get indirect Light Controller
            profile[0].TryGet(out indirectLight[0]);
            if (!profile[0].TryGet(out indirectLight[0]))
            {
                indirectLight[0] = profile[0].Add<IndirectLightingController>();
            }
            profile[1].TryGet(out indirectLight[1]);
            if (!profile[1].TryGet(out indirectLight[1]))
            {
                indirectLight[1] = profile[1].Add<IndirectLightingController>();
            }
            profile[2].TryGet(out indirectLight[2]);
            if (!profile[2].TryGet(out indirectLight[2]))
            {
                indirectLight[2] = profile[2].Add<IndirectLightingController>();
            }
            //Gets Fog
            profile[3].TryGet(out CustomFog);
            if (!profile[3].TryGet(out CustomFog))
            {
                CustomFog = profile[3].Add<Fog>();
            }
            CustomFog.SetAllOverridesTo(true);
            fogDensityVol = AssetHandler.Instance.ActiveDensityVol.GetComponent<DensityVolume>();

            // Get HDRI sky
            profile[0].TryGet(out activeHDRI[0]);
            if (!profile[0].TryGet(out activeHDRI[0]))
            {
                activeHDRI[0] = profile[0].Add<HDRISky>();
            }
            activeHDRI[0].SetAllOverridesTo(true);

            profile[1].TryGet(out activeHDRI[1]);
            if (!profile[1].TryGet(out activeHDRI[1]))
            {
                activeHDRI[1] = profile[1].Add<HDRISky>();
            }
            activeHDRI[1].SetAllOverridesTo(true);

            profile[2].TryGet(out activeHDRI[2]);
            if (!profile[2].TryGet(out activeHDRI[2]))
            {
                activeHDRI[2] = profile[2].Add<HDRISky>();
            }
            activeHDRI[2].SetAllOverridesTo(true);
        }

        private Volume[] volList;
        public Volume currentMapVolume;
        public IEnumerator GetMapVolComponents()
        {
            yield return new WaitUntil(() => AssetHandler.Instance.IsPrefabsSpawned());
            yield return new WaitForFixedUpdate();

            volList = null;
            currentMapVolume = null;

            volList = FindObjectsOfType(typeof(Volume)) as Volume[];

            if (volList == null)
                yield break;

            PrioCheck.CheckPrio(volList);

            foreach (Volume vol in volList)
            {
                if (vol.enabled && vol.profile.Has<VisualEnvironment>() || vol.gameObject.name == "Sky and Fog Volume")
                {
                    currentMapVolume = vol;

                    // Set Default volume Weight Values
                    Main.settings.DefaultVolWeight = currentMapVolume.weight;
                    Main.settings.VolWeightfloat = Main.settings.DefaultVolWeight;

                    yield break;
                }
            }
        }
        public void SetSkyVolumePrio(float prio)
        {
            volume[0].priority = prio + 1;  // Night Sky
            volume[1].priority = prio + 1;  // SunSet Sky
            volume[2].priority = prio + 1;  // Blue Sky
            volume[3].priority = prio + 1;  // Fog Remove
        }
        // ---------------- End Get Components -----------------------

        // -------------- Update Settings --------------------

        private WeatherData.FogSettings FogSettings = new WeatherData.FogSettings(Main.settings.fogMeanFreePath, Main.settings.fogBaseHeight, Main.settings.fogMaxDistance, Main.settings.fogMaxHeight);
        private SkyData.NightSkySettings NightSkySettings = new SkyData.NightSkySettings(Main.settings.NightFixedExposureFloat, Main.settings.NightSkyboxExposureFloat, Main.settings.NightRotateFloat, Main.settings.NightIndirectDiffuse, Main.settings.NightIndirectSpecular);
        private SkyData.SunSetSkySettings SunSetSkySettings = new SkyData.SunSetSkySettings(Main.settings.SunSetSkyExposureFloat, Main.settings.SunSetSkyboxExposureFloat, Main.settings.SunSetRotateFloat, Main.settings.SunSetIndirectDiffuse, Main.settings.SunSetIndirectSpecular);
        private SkyData.BlueSkySettings BlueSkySettings = new SkyData.BlueSkySettings(Main.settings.BlueSkyExposureFloat, Main.settings.BlueSkyboxExposureFloat, Main.settings.BlueRotateFloat, Main.settings.BlueSkyIndirectDiffuse, Main.settings.BlueSkyIndirectSepcular);

        private Vector3 UpdatedSpeed;
        private float oldDensityDist;

        public void ResetDataSettings()
        {
            NightSkySettings = new SkyData.NightSkySettings(0, 0, 0, 0, 0);
            SunSetSkySettings = new SkyData.SunSetSkySettings(0, 0, 0, 0, 0);
            BlueSkySettings = new SkyData.BlueSkySettings(0, 0, 0, 0, 0);

            FogSettings = new WeatherData.FogSettings(0, 0, 0, 0);
            leafSettings = new WeatherData.LeafSettings(0, 0, 0, 0, 0);
            snowSettings = new WeatherData.SnowSettings(0, 0, 0, 0, 0);
            rainSettings = new WeatherData.RainSettings(0, 0, 0, 0, 0);
            cloudSettings = new WeatherData.CloudSettings(0, 0, 0, 0, new Vector3(0, 0, 0), 0, 0);
        }
        private void UpdateNightSkySettings()
        {
            if (volume[0] == null)
                return;

            // Night Sky
            if (NightSkySettings.Exposure != Main.settings.NightFixedExposureFloat)
            {
                exposure[0].fixedExposure.Override(Main.settings.NightFixedExposureFloat);
                NightSkySettings.Exposure = Main.settings.NightFixedExposureFloat;
            }
            if (NightSkySettings.Skyexposure != Main.settings.NightSkyboxExposureFloat)
            {
                activeHDRI[0].exposure.Override(Main.settings.NightSkyboxExposureFloat);
                NightSkySettings.Skyexposure = Main.settings.NightSkyboxExposureFloat;
            }
            if (NightSkySettings.Rotation != Main.settings.NightRotateFloat)
            {
                activeHDRI[0].rotation.Override(Main.settings.NightRotateFloat);
                NightSkySettings.Rotation = Main.settings.NightRotateFloat;
            }
            if (NightSkySettings.IndirectDiffuse != Main.settings.NightIndirectDiffuse)
            {
                indirectLight[0].indirectDiffuseIntensity.Override(Main.settings.NightIndirectDiffuse);
                NightSkySettings.IndirectDiffuse = Main.settings.NightIndirectDiffuse;
            }
            if (NightSkySettings.IndirectSpecular != Main.settings.NightIndirectSpecular)
            {
                indirectLight[0].indirectSpecularIntensity.Override(Main.settings.NightIndirectSpecular);
                NightSkySettings.IndirectSpecular = Main.settings.NightIndirectSpecular;
            }
        }
        private void UpdateSunSetSkySettings()
        {
            if (volume[1] == null)
                return;

            // Sun Set Sky
            if (SunSetSkySettings.Exposure != Main.settings.SunSetSkyExposureFloat)
            {
                exposure[1].fixedExposure.Override(Main.settings.SunSetSkyExposureFloat);
                SunSetSkySettings.Exposure = Main.settings.SunSetSkyExposureFloat;
            }
            if (SunSetSkySettings.Skyexposure != Main.settings.SunSetSkyboxExposureFloat)
            {
                activeHDRI[1].exposure.Override(Main.settings.SunSetSkyboxExposureFloat);
                SunSetSkySettings.Skyexposure = Main.settings.SunSetSkyboxExposureFloat;
            }
            if (SunSetSkySettings.Rotation != Main.settings.SunSetRotateFloat)
            {
                activeHDRI[1].rotation.Override(Main.settings.SunSetRotateFloat);
                SunSetSkySettings.Rotation = Main.settings.SunSetRotateFloat;
            }
            if (SunSetSkySettings.IndirectDiffuse != Main.settings.SunSetIndirectDiffuse)
            {
                indirectLight[1].indirectDiffuseIntensity.Override(Main.settings.SunSetIndirectDiffuse);
                SunSetSkySettings.IndirectDiffuse = Main.settings.SunSetIndirectDiffuse;
            }
            if (SunSetSkySettings.IndirectSpecular != Main.settings.SunSetIndirectSpecular)
            {
                indirectLight[1].indirectSpecularIntensity.Override(Main.settings.SunSetIndirectSpecular);
                SunSetSkySettings.IndirectSpecular = Main.settings.SunSetIndirectSpecular;
            }
        }
        private void UpdateBlueSkySettings()
        {
            if (volume[2] == null)
                return;

            // Blue Sky
            if (BlueSkySettings.Exposure != Main.settings.BlueSkyExposureFloat)
            {
                exposure[2].fixedExposure.Override(Main.settings.BlueSkyExposureFloat);
                BlueSkySettings.Exposure = Main.settings.BlueSkyExposureFloat;
            }
            if (BlueSkySettings.Skyexposure != Main.settings.BlueSkyboxExposureFloat)
            {
                activeHDRI[2].exposure.Override(Main.settings.BlueSkyboxExposureFloat);
                BlueSkySettings.Skyexposure = Main.settings.BlueSkyboxExposureFloat;
            }
            if (BlueSkySettings.Rotation != Main.settings.BlueRotateFloat)
            {
                activeHDRI[2].rotation.Override(Main.settings.BlueRotateFloat);
                BlueSkySettings.Rotation = Main.settings.BlueRotateFloat;
            }
            if (BlueSkySettings.IndirectDiffuse != Main.settings.BlueSkyIndirectDiffuse)
            {
                indirectLight[2].indirectDiffuseIntensity.Override(Main.settings.BlueSkyIndirectDiffuse);
                BlueSkySettings.IndirectDiffuse = Main.settings.BlueSkyIndirectDiffuse;
            }
            if (BlueSkySettings.IndirectSpecular != Main.settings.BlueSkyIndirectSepcular)
            {
                indirectLight[2].indirectSpecularIntensity.Override(Main.settings.BlueSkyIndirectSepcular);
                BlueSkySettings.IndirectSpecular = Main.settings.BlueSkyIndirectSepcular;
            }
        }
        private void UpdateFogSettings()
        {
            if (CustomFog == null)
                return;

            if (ToggleStateData.removeFogToggle == false & ToggleStateData.addFogToggle == false)
            {
                volume[3].weight = 0;
            }
            else
            {
                volume[3].weight = 1;
            }

            if (FogSettings.FogMFP != Main.settings.fogMeanFreePath)
            {
                //fogSettings.meanFreePath.Override(Main.settings.fogMeanFreePath);
                CustomFog.meanFreePath.Override(Main.settings.fogMeanFreePath);
                FogSettings.FogMFP = Main.settings.fogMeanFreePath;
            }
            if (FogSettings.FogBH != Main.settings.fogBaseHeight)
            {
                //fogSettings.baseHeight.Override(Main.settings.fogBaseHeight);
                CustomFog.baseHeight.Override(Main.settings.fogBaseHeight);
                FogSettings.FogBH = Main.settings.fogBaseHeight;
            }
            if (FogSettings.FogMD != Main.settings.fogMaxDistance)
            {
                //fogSettings.maxFogDistance.Override(Main.settings.fogMaxDistance);
                CustomFog.maxFogDistance.Override(Main.settings.fogMaxDistance);
                FogSettings.FogMD = Main.settings.fogMaxDistance;
            }
            if (FogSettings.FogMH != Main.settings.fogMaxHeight)
            {
                CustomFog.maximumHeight.Override(Main.settings.fogMaxHeight);
                FogSettings.FogMH = Main.settings.fogMaxHeight;
            }


            if (AssetHandler.Instance.ActiveDensityVol != null)
            {
                UpdatedSpeed = new Vector3(Main.settings.fogDensitySpeedX, Main.settings.fogDensitySpeedY, Main.settings.fogDensitySpeedZ);
                Vector3 currentspeed = new Vector3(fogDensityVol.parameters.textureScrollingSpeed.x, fogDensityVol.parameters.textureScrollingSpeed.y, fogDensityVol.parameters.textureScrollingSpeed.z);

                if (currentspeed != UpdatedSpeed)
                {
                    fogDensityVol.parameters.textureScrollingSpeed.x = Main.settings.fogDensitySpeedX;
                    fogDensityVol.parameters.textureScrollingSpeed.y = Main.settings.fogDensitySpeedY;
                    fogDensityVol.parameters.textureScrollingSpeed.z = Main.settings.fogDensitySpeedZ;
                    currentspeed = UpdatedSpeed;
                }

                if (oldDensityDist != Main.settings.fogDensityDistance)
                {
                    fogDensityVol.parameters.meanFreePath = Main.settings.fogDensityDistance;
                    oldDensityDist = Main.settings.fogDensityDistance;
                }
            }     
        }

        protected float oldVolWeight = 0f;
        void UpdateVolSettings()
        {
            if (currentMapVolume == null)
                return;

            if (oldVolWeight != Main.settings.VolWeightfloat)
            {
                currentMapVolume.weight = Main.settings.VolWeightfloat;
                oldVolWeight = Main.settings.VolWeightfloat;
            }
        }
        private void ResetVolWeight()
        {
            Main.settings.VolWeightfloat = Main.settings.DefaultVolWeight;
            //currentMapVolume.weight = Main.settings.DefaultVolWeight;
        }

        // ------------- End Update Settings -----------------

        // ------------ Update VFX settings -------------

        private VisualEffect rain, snow, leaves, lightning;
        private AudioSource RainAudio;
        private ExposedProperty LeafDensity = "LeafDensity", LeafAreaSize = "Box Size", LeafGravity = "Gravity Strength", LeafWind = "Wind Speed", LeafLifetime = "Lifetime";
        private ExposedProperty SnowDensity = "SnowDensity", SnowAreaSize = "Box Size", SnowGravity = "Gravity Strength", SnowWind = "Wind Speed", SnowLifetime = "Lifetime";
        private ExposedProperty RainDensity = "RainDensity", RainAreaSize = "Box Size", RainGravity = "Gravity Strength", RainWind = "Wind Speed", RainLifetime = "Lifetime";

        private WeatherData.LeafSettings leafSettings = new WeatherData.LeafSettings(Main.settings.LeafDensityFloat, Main.settings.LeafSizeFloat, Main.settings.LeafGravityFloat, Main.settings.LeafWindFloat, Main.settings.LeafLifeFloat);
        private WeatherData.SnowSettings snowSettings = new WeatherData.SnowSettings(Main.settings.SnowDensityFloat, Main.settings.SnowSizeFloat, Main.settings.SnowGravityFloat, Main.settings.SnowWindFloat, Main.settings.SnowLifeFloat);
        private WeatherData.RainSettings rainSettings = new WeatherData.RainSettings(Main.settings.RainDensityFloat, Main.settings.RainSizeFloat, Main.settings.RainGravityFloat, Main.settings.RainWindFloat, Main.settings.RainLifeFloat);
        private WeatherData.CloudSettings cloudSettings = new WeatherData.CloudSettings(Main.settings.Cloud_ParallexOffset, Main.settings.Cloud_Iterations, Main.settings.Cloud_NoiseScale, Main.settings.Cloud_NoiseDepth, new Vector3(1, 1, 1), Main.settings.Cloud_Speed, Main.settings.Cloud_Intensity);

        private IEnumerator GetVfxComponent()
        {
            yield return new WaitUntil(() => AssetHandler.Instance.IsPrefabsSpawned());

            leaves = AssetHandler.Instance.activeVFX[0].GetComponent<VisualEffect>();
            snow = AssetHandler.Instance.activeVFX[1].GetComponent<VisualEffect>();
            rain = AssetHandler.Instance.activeVFX[2].GetComponent<VisualEffect>();
            RainAudio = rain.GetComponent<AudioSource>();
            lightning = AssetHandler.Instance.activeVFX[4].GetComponent<VisualEffect>();
            cloudRenderer = AssetHandler.Instance.activeVFX[5].GetComponent<MeshRenderer>();
        }
        
        // creates state check instances for each vfx to stop and play effects if density should be 0, not a good fix. will come up with better solution.
        VFXUtils.VFXStateCheck LeavesStateCheck = new VFXUtils.VFXStateCheck();
        VFXUtils.VFXStateCheck RainStateCheck = new VFXUtils.VFXStateCheck();
        VFXUtils.VFXStateCheck SnowStateCheck = new VFXUtils.VFXStateCheck();
        private void UpdateLeafSettings()
        {
            if (AssetHandler.Instance.activeVFX[0]?.activeSelf == true)
            {
                if (leafSettings.Density != Main.settings.LeafDensityFloat)
                {
                    leaves.SetFloat(LeafDensity, Main.settings.LeafDensityFloat);
                    leafSettings.Density = Main.settings.LeafDensityFloat;
                }

                LeavesStateCheck.UpdateVFXState(Main.settings.LeafDensityFloat, leaves);

                if (leafSettings.AreaSize != Main.settings.LeafSizeFloat)
                {
                    leaves.SetFloat(LeafAreaSize, Main.settings.LeafSizeFloat);
                    leafSettings.AreaSize = Main.settings.LeafSizeFloat;
                }
                if (leafSettings.Gravity != Main.settings.LeafGravityFloat)
                {
                    leaves.SetFloat(LeafGravity, Main.settings.LeafGravityFloat);
                    leafSettings.Gravity = Main.settings.LeafGravityFloat;
                }
                if (leafSettings.Wind != Main.settings.LeafWindFloat)
                {
                    leaves.SetFloat(LeafWind, Main.settings.LeafWindFloat);
                    leafSettings.Wind = Main.settings.LeafWindFloat;
                }
                if (leafSettings.Lifetime != Main.settings.LeafLifeFloat)
                {
                    leaves.SetFloat(LeafLifetime, Main.settings.LeafLifeFloat);
                    leafSettings.Lifetime = Main.settings.LeafLifeFloat;
                }
            }
        }
        private void UpdateSnowSettings()
        {
            if (AssetHandler.Instance.activeVFX[1]?.activeSelf == true)
            {
                if (snowSettings.Density != Main.settings.SnowDensityFloat)
                {
                    snow.SetFloat(SnowDensity, Main.settings.SnowDensityFloat);
                    snowSettings.Density = Main.settings.SnowDensityFloat;
                }

                SnowStateCheck.UpdateVFXState(Main.settings.SnowDensityFloat, snow);

                if (snowSettings.AreaSize != Main.settings.SnowSizeFloat)
                {
                    snow.SetFloat(SnowAreaSize, Main.settings.SnowSizeFloat);
                    snowSettings.AreaSize = Main.settings.SnowSizeFloat;
                }
                if (snowSettings.Gravity != Main.settings.SnowGravityFloat)
                {
                    snow.SetFloat(SnowGravity, Main.settings.SnowGravityFloat);
                    snowSettings.Gravity = Main.settings.SnowGravityFloat;
                }
                if (snowSettings.Wind != Main.settings.SnowWindFloat)
                {
                    snow.SetFloat(SnowWind, Main.settings.SnowWindFloat);
                    snowSettings.Wind = Main.settings.SnowWindFloat;
                }
                if (snowSettings.Lifetime != Main.settings.SnowLifeFloat)
                {
                    snow.SetFloat(SnowLifetime, Main.settings.SnowLifeFloat);
                    snowSettings.Lifetime = Main.settings.SnowLifeFloat;
                }
            }       
        }

        private void UpdateRainSettings()
        {
            if (AssetHandler.Instance.activeVFX[2]?.activeSelf == true)
            {
                if (rainSettings.Density != Main.settings.RainDensityFloat)
                {
                    rain.SetFloat(RainDensity, Main.settings.RainDensityFloat);
                    rainSettings.Density = Main.settings.RainDensityFloat;
                }

                RainStateCheck.UpdateVFXState(Main.settings.RainDensityFloat, rain);

                if (rainSettings.AreaSize != Main.settings.RainSizeFloat)
                {
                    rain.SetFloat(RainAreaSize, Main.settings.RainSizeFloat);
                    rainSettings.AreaSize = Main.settings.RainSizeFloat;
                }
                if (rainSettings.Gravity != Main.settings.RainGravityFloat)
                {
                    rain.SetFloat(RainGravity, Main.settings.RainGravityFloat);
                    rainSettings.Gravity = Main.settings.RainGravityFloat;
                }
                if (rainSettings.Wind != Main.settings.RainWindFloat)
                {
                    rain.SetFloat(RainWind, Main.settings.RainWindFloat);
                    rainSettings.Wind = Main.settings.RainWindFloat;
                }
                if (rainSettings.Lifetime != Main.settings.RainLifeFloat)
                {
                    rain.SetFloat(RainLifetime, Main.settings.RainLifeFloat);
                    rainSettings.Lifetime = Main.settings.RainLifeFloat;
                }

                if (RainAudio.volume != Main.settings.RainVolumeFloat)
                {
                    RainAudio.volume = Main.settings.RainVolumeFloat;
                }
            }
        }

        private void UpdateCloudSettings()
        {
            if (AssetHandler.Instance.activeVFX[5]?.activeSelf == true)
            {
                if (cloudSettings.ParallexOffset != Main.settings.Cloud_ParallexOffset)
                {
                    cloudRenderer.material.SetFloat("_ParallexOffset", Main.settings.Cloud_ParallexOffset);
                    cloudSettings.ParallexOffset = Main.settings.Cloud_ParallexOffset;
                }
                if (cloudSettings.Iterations != Main.settings.Cloud_Iterations)
                {
                    cloudRenderer.material.SetFloat("_Iterations", Main.settings.Cloud_Iterations);
                    cloudSettings.Iterations = Main.settings.Cloud_Iterations;
                }
                if (cloudSettings.NoiseScale != Main.settings.Cloud_NoiseScale)
                {
                    cloudRenderer.material.SetFloat("_NoiseScale", Main.settings.Cloud_NoiseScale);
                    cloudSettings.NoiseScale = Main.settings.Cloud_NoiseScale;
                }
                if (cloudSettings.NoiseDepth != Main.settings.Cloud_NoiseDepth)
                {
                    cloudRenderer.material.SetFloat("_NoiseDepth", Main.settings.Cloud_NoiseDepth);
                    cloudSettings.NoiseDepth = Main.settings.Cloud_NoiseDepth;
                }
                var tiling = new Vector3(Main.settings.Cloud_CrackTiling_x, Main.settings.Cloud_CrackTiling_y, 1);
                if (cloudSettings.CrackTiling != tiling)
                {
                    cloudRenderer.material.SetVector("_CrackTiling", tiling);
                    cloudSettings.CrackTiling = tiling;
                }
                if (cloudSettings.Speed != Main.settings.Cloud_Speed)
                {
                    cloudRenderer.material.SetFloat("_Speed", Main.settings.Cloud_Speed);
                    cloudSettings.Speed = Main.settings.Cloud_Speed;
                }
                if (cloudSettings.Intensity != Main.settings.Cloud_Intensity)
                {
                    cloudRenderer.material.SetFloat("_Intensity", Main.settings.Cloud_Intensity);
                    cloudSettings.Intensity = Main.settings.Cloud_Intensity;
                }
            } 
        }

        // ------------- End Update VFX settings --------------
    }
}



