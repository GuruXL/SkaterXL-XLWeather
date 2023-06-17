using UnityEngine;
using System.Collections;
using System.IO;
using System.Reflection;
using System;
using System.Linq;
using ModIO.UI;
using Object = UnityEngine.Object;

namespace XLWeather.Utils
{
    public class AssetHandler
    {
        public static AssetHandler __instance { get; private set; }
        public static AssetHandler Instance => __instance ?? (__instance = new AssetHandler());

        public GameObject[] SkyVolume = new GameObject[4];
        public GameObject[] VFXprefab = new GameObject[6];
        public GameObject DayNightprefab;
        public Cubemap[] activeCubeMap = new Cubemap[7];
        public Cubemap[] cycleSkys = new Cubemap[6];
        public GameObject DensityVolObj;  
        public GameObject dronePrefab;
        public Texture2D MoonCookie;

        public GameObject[] activeSky = new GameObject[4];
        public GameObject[] activeVFX = new GameObject[6];
        public GameObject activeDayNight;  
        public GameObject ActiveDensityVol;
        public GameObject activeDrone;

        public AssetBundle fxBundle;
        public AssetBundle dayNightBundle;
        protected bool prefabsLoaded = false;
        protected bool prefabsSpawned = false;

        public string fxBundlePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "weatherfx");
        public string dayNightBundlePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "daynightcycle");
        public bool IsPrefabsLoaded()
        {
            return prefabsLoaded;
        }
        public bool IsPrefabsSpawned()
        {
            return prefabsSpawned;
        }

        public void LoadBundles()
        {
            // Check if a type from the Unity assembly has been loaded
            Type unityObjectType = Type.GetType("UnityEngine.Object, UnityEngine");

            if (unityObjectType != null)
            {
                // load assets here
                PlayerController.Instance.StartCoroutine(LoadAssetBundleAsync()); // 1.2.2.8
                //PlayerController.Instance.StartCoroutine(LoadAssetBundleAsync()); // 1.2.6.0             
            }
        }

        private IEnumerator LoadAssetBundleAsync()
        {
            var requestFXbundle = AssetBundle.LoadFromFileAsync(fxBundlePath);
            var requestDayNightbundle = AssetBundle.LoadFromFileAsync(dayNightBundlePath);

            while (!requestFXbundle.isDone && !requestDayNightbundle.isDone)
            {
                yield return null;
            }

            fxBundle = requestFXbundle.assetBundle;
            dayNightBundle = requestDayNightbundle.assetBundle;

            yield return PlayerController.Instance.StartCoroutine(LoadPrefabs());
            yield return new WaitUntil(() => prefabsLoaded == true);
            yield return PlayerController.Instance.StartCoroutine(InstantiatePrefabs());
        }
        private IEnumerator LoadPrefabs()
        {
            if (fxBundle == null || dayNightBundle == null)
            {
                MessageSystem.QueueMessage(MessageDisplayData.Type.Error, $"XLWeather Asset bundles are not loaded!", 2.5f);
                yield break;
            }

            yield return PlayerController.Instance.StartCoroutine(LoadDayNightPrefab());
            yield return PlayerController.Instance.StartCoroutine(LoadCycleSkys());
            yield return PlayerController.Instance.StartCoroutine(LoadVolumes());
            yield return PlayerController.Instance.StartCoroutine(LoadActiveCubeMaps());
            yield return PlayerController.Instance.StartCoroutine(LoadVFXPrefabs());
            yield return PlayerController.Instance.StartCoroutine(LoadDronePrefab());

            // Wait for all prefabs to finish loading before setting prefabsLoaded to true
            while (!DayNightprefab || cycleSkys.Any(c => c == null) || SkyVolume.Any(v => v == null) ||
                   activeCubeMap.Any(a => a == null) || VFXprefab.Any(v => v == null) || dronePrefab == null)
            {
                yield return null;
            }
            prefabsLoaded = true;
        }
        private IEnumerator LoadDayNightPrefab()
        {
            DayNightprefab = dayNightBundle.LoadAsset<GameObject>("CycleV3");
            yield return null;

            //MoonCookie = dayNightBundle.LoadAsset<Texture2D>("mooncookie2");
            //yield return null;
        }
        private IEnumerator LoadCycleSkys()
        {
            cycleSkys[0] = dayNightBundle.LoadAsset<Cubemap>("ReflectionProbeClouds2k");
            yield return null;
            cycleSkys[1] = dayNightBundle.LoadAsset<Cubemap>("ReflectionProbeScatteredClouds2k");
            yield return null;
            cycleSkys[2] = dayNightBundle.LoadAsset<Cubemap>("ReflectionProbeCloudSky3");
            yield return null;
            cycleSkys[3] = dayNightBundle.LoadAsset<Cubemap>("ReflectionProbeCloudSky4");
            yield return null;
            cycleSkys[4] = dayNightBundle.LoadAsset<Cubemap>("ReflectionProbeCloudy5");
            yield return null;
            cycleSkys[5] = dayNightBundle.LoadAsset<Cubemap>("ReflectionProbeWhispyClouds");
            yield return null;
        }
        private IEnumerator LoadVolumes()
        {
            SkyVolume[0] = fxBundle.LoadAsset<GameObject>("NightSkyVolume");
            yield return null;
            SkyVolume[1] = fxBundle.LoadAsset<GameObject>("SunSetSkyVolume");
            yield return null;
            SkyVolume[2] = fxBundle.LoadAsset<GameObject>("BlueSkyVolume");
            yield return null;
            SkyVolume[3] = fxBundle.LoadAsset<GameObject>("FogRemoveVolume");
            yield return null;
            DensityVolObj = fxBundle.LoadAsset<GameObject>("FogDensityVolume");
        }
        private IEnumerator LoadActiveCubeMaps()
        {
            activeCubeMap[0] = fxBundle.LoadAsset<Cubemap>("Night4kV3");
            yield return null;
            activeCubeMap[1] = fxBundle.LoadAsset<Cubemap>("MilkyWayPanorama8K");
            yield return null;
            activeCubeMap[2] = fxBundle.LoadAsset<Cubemap>("StarryNight");
            yield return null;
            activeCubeMap[3] = fxBundle.LoadAsset<Cubemap>("SunSet");
            yield return null;
            activeCubeMap[4] = fxBundle.LoadAsset<Cubemap>("SunSet2");
            yield return null;
            activeCubeMap[5] = fxBundle.LoadAsset<Cubemap>("BlueSky3");
            yield return null;
            activeCubeMap[6] = fxBundle.LoadAsset<Cubemap>("OverCast");
        }
        private IEnumerator LoadVFXPrefabs()
        {
            VFXprefab[0] = fxBundle.LoadAsset<GameObject>("LeavesFalling");
            yield return null;
            VFXprefab[1] = fxBundle.LoadAsset<GameObject>("SnowV3");
            yield return null;
            VFXprefab[2] = fxBundle.LoadAsset<GameObject>("RainV2");
            yield return null;
            VFXprefab[3] = fxBundle.LoadAsset<GameObject>("NorthernLights");
            yield return null;
            VFXprefab[4] = fxBundle.LoadAsset<GameObject>("LightningV3");
            yield return null;
            VFXprefab[5] = fxBundle.LoadAsset<GameObject>("CloudPlane");
            yield return null;
        }
        private IEnumerator LoadDronePrefab()
        {
            dronePrefab = fxBundle.LoadAsset<GameObject>("DroneParent");
            yield return null;
        }
   
        public static GameObject InstantiatePrefab(GameObject prefab)
        {
            GameObject instance = Object.Instantiate(prefab);
            return instance;
        }
        private IEnumerator InstantiatePrefabs()
        {
            activeDayNight = InstantiatePrefab(DayNightprefab);
            activeDayNight.transform.SetParent(Main.scriptManager.transform);
            activeDayNight.SetActive(false);

            activeSky[0] = InstantiatePrefab(SkyVolume[0]);
            activeSky[1] = InstantiatePrefab(SkyVolume[1]);
            activeSky[2] = InstantiatePrefab(SkyVolume[2]);
            activeSky[3] = InstantiatePrefab(SkyVolume[3]);
            foreach (var sky in activeSky)
            {
                sky.transform.SetParent(Main.scriptManager.transform);
                sky.SetActive(false);
            }

            ActiveDensityVol = InstantiatePrefab(DensityVolObj);
            ActiveDensityVol.transform.SetParent(Main.scriptManager.transform);
            ActiveDensityVol.SetActive(false);

            activeVFX[0] = InstantiatePrefab(VFXprefab[0]);
            activeVFX[1] = InstantiatePrefab(VFXprefab[1]);
            activeVFX[2] = InstantiatePrefab(VFXprefab[2]);
            activeVFX[3] = InstantiatePrefab(VFXprefab[3]);
            activeVFX[4] = InstantiatePrefab(VFXprefab[4]);
            activeVFX[5] = InstantiatePrefab(VFXprefab[5]);
            foreach (var vfx in activeVFX)
            {
                vfx.transform.SetParent(Main.scriptManager.transform);
                vfx.gameObject.SetActive(false);
            }

            activeDrone = InstantiatePrefab(dronePrefab);
            activeDrone.transform.SetParent(Main.scriptManager.transform);
            activeDrone.SetActive(false);

            // Wait until all prefabs are instantiated
            yield return new WaitUntil(() =>
                activeSky[0] != null &&
                activeSky[1] != null &&
                activeSky[2] != null &&
                activeSky[3] != null &&
                ActiveDensityVol != null &&
                activeDayNight != null &&
                activeVFX[0] != null &&
                activeVFX[1] != null &&
                activeVFX[2] != null &&
                activeVFX[3] != null &&
                activeVFX[4] != null &&
                activeVFX[5] != null &&
                activeDrone != null);

            prefabsSpawned = true;
        }

        public void UnloadAssetBundle()
        {
            if (fxBundle != null)
            {
                fxBundle.Unload(true);
                fxBundle = null;
            }
            if (dayNightBundle != null)
            {
                dayNightBundle.Unload(true);
                dayNightBundle = null;
            }
            prefabsLoaded = false;
            prefabsSpawned = false;
        }

        private void OnDestroy()
        {
            UnloadAssetBundle();
        }

    }
}
