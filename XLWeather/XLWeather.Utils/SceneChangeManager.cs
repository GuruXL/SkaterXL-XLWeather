using UnityEngine;
using UnityEngine.SceneManagement;
using XLWeather.Data;

namespace XLWeather.Utils
{
    public class SceneChangeManager : MonoBehaviour
    {
       
        private void Awake()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
        }

        private void Start()
        {
            UpdateSceneObjects();
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name != "LoadingScene")
            {
                UpdateSceneObjects();
            }

            Main.Logger.Log("Scene Loaded: " + scene.name);
        }

        
        private void OnSceneUnloaded(Scene scene)
        {
            
            if (Main.settings.MapLayersToggle && scene.name != "LoadingScene")
            {
                Main.MapLightctrl.ResetLayerToggles();
            }
            
            Main.Logger.Log("Scene Unloaded: " + scene.name);
        }
        

        private void UpdateSceneObjects()
        {
            StartCoroutine(Main.Weatherctrl.GetMapVolComponents());
            Main.MapLightctrl.ResetMainLight();
            Main.MapLightctrl.GetLights();
            //Main.MapLightctrl.GetReflectionProbes();
            
            if (Main.settings.MapLayersToggle)
            {
                Main.MapLightctrl.GetLayerObjects();
            }
            
            if (ToggleStateData.DayNightToggle)
            {
                Main.MapLightctrl.ToggleMainLights(false);
            }
        }
    }
}