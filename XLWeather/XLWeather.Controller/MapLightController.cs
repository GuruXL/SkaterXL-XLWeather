using GameManagement;
using UnityEngine;
using System.Collections.Generic;
using XLWeather.Data;
using ModIO.UI;
using System;

namespace XLWeather.Controller
{
    public class MapLightController : MonoBehaviour
    {
        public Light[] lights;
        List<Light> TaggedLightsList;
        List<Light> CurrentMainLights;
        List<GameObject> TaggedGO;
        public LightType lightType = LightType.Directional;
        public Light MainLight = null;
        public List<Material> hdrpMaterials;
        public List<ReflectionProbe> probes;
        public Dictionary<Material, float> originalEmissionweights;
        //private int delay = 0;

        private bool IsDefaultValueSet = false;

        private float lastIntensity;

        public bool ListsPopulated()
        {
            if (TaggedLightsList.Count > 0 && TaggedGO.Count > 0 && hdrpMaterials.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void Update()
        {
            if (MainLight == null)
                return;

            if (lastIntensity != Main.settings.MapLightIntensity)
            {
                MainLight.intensity = Main.settings.MapLightIntensity;
                lastIntensity = Main.settings.MapLightIntensity;
            }

        }
        private void FixedUpdate()
        {
            if (!Main.settings.MapLayersToggle)
                return;

            if (ToggleStateData.DayNightToggle && ListsPopulated())
            {
                UpdateLayerMapLights();
            }
        }

        // ----- Enable/Disable Lights for DayNightCycle -------
        public void GetLights()
        {
            if (GameStateMachine.Instance.CurrentState.GetType() == typeof(GearSelectionState))
                return;

            if (MainLight == null)
            {
                IsDefaultValueSet = false;
            }

            lights = null;

            CurrentMainLights = new List<Light>();
            CurrentMainLights.Clear();

            lights = FindObjectsOfType(typeof(Light)) as Light[];

            if (lights == null)
                return;

            foreach (Light LightObj in lights)
            {
                if (LightObj.type == lightType && LightObj.isActiveAndEnabled &&
                    LightObj.name != "AmbientLight" && LightObj.name != "CycleSun" && LightObj.name != "CycleMoon")
                {
                    CurrentMainLights.Add(LightObj);
                }
            }
        }
        public void ToggleMainLights(bool state)
        {
            if (CurrentMainLights == null)
                return;

            foreach (Light light in CurrentMainLights)
            {
                light.enabled = state;
            }
        }
        // ------ End Enable/Disable Lights for DayNightCycle -------

        // ------ Get Lights and Values for Main map Light -----------
        public void GetMainSun()
        {
            if (CurrentMainLights == null)
                return;

            foreach (Light LightObj in CurrentMainLights)
            {
                if (LightObj.type == lightType)
                {
                    MainLight = LightObj;
                }
            }

            if (IsDefaultValueSet && MainLight == null)
                return;

            GetDefaultIntensity();
        }

        private void GetDefaultIntensity()
        {
            Main.settings.DefaultMapLightIntensity = MainLight.intensity;
            Main.settings.MapLightIntensity = Main.settings.DefaultMapLightIntensity;
            IsDefaultValueSet = true;
        }
        public void ResetMainLight()
        {
            if (MainLight == null)
                return;

            MainLight.intensity = Main.settings.DefaultMapLightIntensity;
            IsDefaultValueSet = false;
            MainLight = null;

        }
        // ------ End Get Lights and Values for Main map Light -----------

        // ------- Toggles Objects on and off during Day Night if they are on Layer 28 ---------------
        public void GetLayerObjects()
        {
            if (GameStateMachine.Instance.CurrentState.GetType() == typeof(GearSelectionState))
                return;

            GetLayerMapLights();
            GetLayerGO();
            GetMapMaterials();
            GetReflectionProbes();
        }
        private void GetLayerMapLights()
        {
            TaggedLightsList = new List<Light>();
            TaggedLightsList.Clear();
            
            if (lights == null)
                return;

            int layerMask = 1 << 30; // shift 1 by the layer number to create a layer mask
            for (int i = 0; i < lights.Length; i++)
            {
                if ((1 << lights[i].gameObject.layer & layerMask) != 0)
                {
                    TaggedLightsList.Add(lights[i]);
                }
            }

            Main.Logger.Log($"{TaggedLightsList.Count} Map Lights on Layer 30");
        }

        private void GetLayerGO()
        {
            TaggedGO = new List<GameObject>();
            TaggedGO.Clear();
    
            GameObject[] allObjects = FindObjectsOfType<GameObject>();

            if (allObjects == null)
                return;

            int layerMask = 1 << 29;
            for (int i = 0; i < allObjects.Length; i++)
            {
                if ((1 << allObjects[i].layer & layerMask) != 0)
                {
                    TaggedGO.Add(allObjects[i]);
                }
            }

            Main.Logger.Log($"{TaggedGO.Count} Game Objects on Layer 29");
        }

        private void ToggleMapLights(bool state)
        {
            if (TaggedLightsList == null)
                return;

            int i = 0;
            foreach (Light light in TaggedLightsList)
            {
                light.enabled = state;
                i++;
            }
            Main.Logger.Log($"{i} : lights toggled");

        }
        private void ToggleLayerGO(bool state)
        {
            if (TaggedGO == null)
                return;

            int i = 0;
            foreach (GameObject go in TaggedGO)
            {
                go.SetActive(state);
                i++;
            }
            Main.Logger.Log($"{i} : Game Objects toggled");
        }

        public bool previousSunActive;
        public void UpdateLayerMapLights()
        {
            bool sunActive = Main.Cyclectrl.GetIsDay();

            if (sunActive != previousSunActive)
            {
                LayerSwitch();
                previousSunActive = sunActive;
            }
        }

        public void LayerSwitch()
        {
            bool sunActive = Main.Cyclectrl.GetIsDay();

            switch (sunActive)
            {
                case true:
                    ToggleLayerGO(false);
                    ToggleMapLights(false);
                    ToggleEmission(false);
                    toggleProbes(true);
                    Main.Logger.Log("sunActive: " + sunActive);
                    break;
                case false:
                    ToggleLayerGO(true);
                    ToggleMapLights(true);
                    ToggleEmission(true);
                    toggleProbes(false);
                    Main.Logger.Log("sunActive: " + sunActive);
                    break;
            }
        }

        public void ResetLayerToggles()
        {
            previousSunActive = false;
            ToggleLayerGO(false);
            ToggleMapLights(true);
            ToggleEmission(true);
            toggleProbes(true);
        }

        // ------- End Toggle for Tagged Lights ---------------

        // ----------- get map materials -----------------

        private void GetMapMaterials()
        {
            hdrpMaterials = new List<Material>();
            originalEmissionweights = new Dictionary<Material, float>();

            Renderer[] renderers = FindObjectsOfType<Renderer>();
 
            if (renderers == null)
                return;

            List<Renderer> layeredRenderers = new List<Renderer>();

            int layerMask = 1 << 30; // shift 1 by the layer number to create a layer mask
            foreach (Renderer renderer in renderers)
            {
                if ((1 << renderer.gameObject.layer & layerMask) != 0)
                {
                    layeredRenderers.Add(renderer);
                }
            }

            if (layeredRenderers == null || layeredRenderers.Count <= 0)
                return;
            
            foreach (Renderer renderer in layeredRenderers)
            {
                Material[] materials = renderer.materials;

                foreach (Material material in materials)
                {
                    if (material != null && (material.shader.name.Contains("HDRP") || material.shader.name.Contains("HD")))
                    {
                        // Only add materials with non-black emissive color
                        if (material.GetColor("_EmissiveColor") != Color.black)
                        {
                            hdrpMaterials.Add(material);
                            originalEmissionweights[material] = material.GetFloat("_EmissiveExposureWeight");
                        }
                    }
                }
            }

            Main.Logger.Log($"{hdrpMaterials.Count} emissive materials found");
        }

        public void ToggleEmission(bool enable)
        {
            if (hdrpMaterials == null)
                return;

            foreach (Material material in hdrpMaterials)
            {
                if (enable)
                {
                    material.SetFloat("_EmissiveExposureWeight", originalEmissionweights[material]);
                    //Main.Logger.Log($" {originalEmissionweights[material]}: Exposure weight");
                }
                else
                {
                    originalEmissionweights[material] = material.GetFloat("_EmissiveExposureWeight");
                    material.SetFloat("_EmissiveExposureWeight", 1f);
                    //Main.Logger.Log($" 1: Exposure weight");
                }
            }

            Main.Logger.Log($"Emissive materials {(enable ? "enabled" : "disabled")}");
        }

        // ---------- end of Material changes  --------------

        // ---------- reflection probe functions -------------

        void GetReflectionProbes()
        {
            probes = new List<ReflectionProbe>();
            probes.Clear();

            foreach (GameObject obj in TaggedGO)
            {
                ReflectionProbe[] reflectionProbes = obj.GetComponents<ReflectionProbe>();

                if (reflectionProbes.Length > 0)
                {
                    probes.AddRange(reflectionProbes);
                }
            }
        }

        void toggleProbes(bool value)
        {
            foreach (ReflectionProbe probe in probes)
            {
                string probeName = probe.gameObject.name.ToLower(); // Convert the name to lowercase

                if (probeName.IndexOf("day", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    probe.gameObject.SetActive(value);
                }
                else if (probeName.IndexOf("night", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    probe.gameObject.SetActive(!value);
                }
            }
        }
    }   
}
