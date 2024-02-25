using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using ModIO.UI;
using Newtonsoft.Json;

namespace XLWeather.Presets
{
    public class PresetManager : MonoBehaviour
    {
        public PresetSettings savePreset;
        public PresetSettings loadedPreset;

        private string mainPath;
        public string PresetName = "";
        public string PresetToLoad { get; set; }
        private string LastPresetLoaded;
        private readonly string defaulState = "Select Preset to Load";

        public void Awake()
        {
            mainPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "SkaterXL\\XLWeather\\");

            if (!Directory.Exists(mainPath + "CyclePresets"))
            {
                Directory.CreateDirectory(mainPath + "CyclePresets");
                MessageSystem.QueueMessage(MessageDisplayData.Type.Info, $"CyclePresets folder created", 2.5f);
                Main.Logger.Log("No CyclePresets Folder Found: New folder has been created");
            }

            ResetPresetList();
            CreatePresets();
        }

        public void Update()
        {
            if (!loadingConditions())
                return;

            LoadPreset();
        }

        void CreatePresets()
        {
            savePreset = new PresetSettings();
            loadedPreset = new PresetSettings();
        }
     
        public void ResetPresetList()
        {
            PresetToLoad = defaulState;
            LastPresetLoaded = defaulState;
        }

        private bool loadingConditions()
        {
            if (PresetToLoad != LastPresetLoaded && PresetToLoad != defaulState)
            {
                return true;
            }
            return false;
        }
        public void LoadPreset()
        {
            string filePath = Path.Combine(mainPath, "CyclePresets", $"{PresetToLoad}.json");
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                loadedPreset = JsonConvert.DeserializeObject<PresetSettings>(json); // Deserialize the JSON string
                MessageSystem.QueueMessage(MessageDisplayData.Type.Info, $"{PresetToLoad} Preset Ready to Apply", 2.5f);
                LastPresetLoaded = PresetToLoad;
            }
            else
            {
                MessageSystem.QueueMessage(MessageDisplayData.Type.Error, $"{PresetToLoad} Preset file not found.", 2.5f);
            }
        }
        public void SavePreset()
        {
            savePreset.timeMulipiler = Main.settings.timeMulipiler;
            savePreset.startHour = Main.settings.startHour;
            savePreset.sunriseHour = Main.settings.sunriseHour;
            savePreset.sunsetHour = Main.settings.sunsetHour;
            savePreset.sunMaxIntensity = Main.settings.sunMaxIntensity;
            savePreset.sunMinIntensity = Main.settings.sunMinIntensity;
            savePreset.moonIntensity = Main.settings.moonIntensity;
            savePreset.sunShadowFloat = Main.settings.sunShadowFloat;
            savePreset.moonShadowFloat = Main.settings.moonShadowFloat;
            savePreset.ShadowDistFloat = Main.settings.ShadowDistFloat;
            savePreset.ShadowHighlights = Main.settings.ShadowHighlights;
            savePreset.SunMinExFloat = Main.settings.SunMinExFloat;
            savePreset.SunMaxExFloat = Main.settings.SunMaxExFloat;
            savePreset.SunExCompFlt = Main.settings.SunExCompFlt;
            savePreset.MoonMinExFloat = Main.settings.MoonMinExFloat;
            savePreset.MoonMaxExFloat = Main.settings.MoonMaxExFloat;
            savePreset.MoonExCompFlt = Main.settings.MoonExCompFlt;
            savePreset.sunSkyExFloat = Main.settings.sunSkyExFloat;
            savePreset.moonSkyExFloat = Main.settings.moonSkyExFloat;
            savePreset.CycleXrotFloat = Main.settings.CycleXrotFloat;
            savePreset.sunDimmerFloat = Main.settings.sunDimmerFloat;
            savePreset.moonDimmerFloat = Main.settings.moonDimmerFloat;
            savePreset.SunColorFloat = Main.settings.SunColorFloat;
            savePreset.MoonColorFloat = Main.settings.MoonColorFloat;
            savePreset.AmbientLightFloat = Main.settings.AmbientLightFloat;
            savePreset.sunSpaceEmission = Main.settings.sunSpaceEmission;
            savePreset.moonSpaceEmission = Main.settings.moonSpaceEmission;
            savePreset.SunIndirectLight = Main.settings.SunIndirectLight;
            savePreset.SunIndirectSpecular = Main.settings.SunIndirectSpecular;
            savePreset.sunAngularDiameter = Main.settings.sunAngularDiameter;
            savePreset.MoonIndirectLight = Main.settings.MoonIndirectLight;
            savePreset.MoonIndirectSpecular = Main.settings.MoonIndirectSpecular;
            savePreset.moonAngularDiameter = Main.settings.moonAngularDiameter;
            savePreset.VolWeightfloat = Main.settings.VolWeightfloat;

            string json = JsonConvert.SerializeObject(savePreset, Formatting.Indented); // Using JsonConvert with Formatting.Indented for serialization

            File.WriteAllText(Path.Combine(mainPath, "CyclePresets", $"{PresetName}.json"), json);

            MessageSystem.QueueMessage(MessageDisplayData.Type.Success, $"{PresetName} Preset Created", 2.5f);

            PresetName = "";
        }

        public void ApplyPreset()
        {
            if (PresetToLoad != defaulState)
            {
                // Applying values from loadedPreset to Main.settings
                Main.settings.timeMulipiler = loadedPreset.timeMulipiler;
                Main.settings.startHour = loadedPreset.startHour;
                Main.settings.sunriseHour = loadedPreset.sunriseHour;
                Main.settings.sunsetHour = loadedPreset.sunsetHour;
                Main.settings.sunMaxIntensity = loadedPreset.sunMaxIntensity;
                Main.settings.sunMinIntensity = loadedPreset.sunMinIntensity;
                Main.settings.moonIntensity = loadedPreset.moonIntensity;
                Main.settings.sunShadowFloat = loadedPreset.sunShadowFloat;
                Main.settings.moonShadowFloat = loadedPreset.moonShadowFloat;
                Main.settings.ShadowDistFloat = loadedPreset.ShadowDistFloat;
                Main.settings.ShadowHighlights = loadedPreset.ShadowHighlights;
                Main.settings.SunMinExFloat = loadedPreset.SunMinExFloat;
                Main.settings.SunMaxExFloat = loadedPreset.SunMaxExFloat;
                Main.settings.SunExCompFlt = loadedPreset.SunExCompFlt;
                Main.settings.MoonMinExFloat = loadedPreset.MoonMinExFloat;
                Main.settings.MoonMaxExFloat = loadedPreset.MoonMaxExFloat;
                Main.settings.MoonExCompFlt = loadedPreset.MoonExCompFlt;
                Main.settings.sunSkyExFloat = loadedPreset.sunSkyExFloat;
                Main.settings.moonSkyExFloat = loadedPreset.moonSkyExFloat;
                Main.settings.CycleXrotFloat = loadedPreset.CycleXrotFloat;
                Main.settings.sunDimmerFloat = loadedPreset.sunDimmerFloat;
                Main.settings.moonDimmerFloat = loadedPreset.moonDimmerFloat;
                Main.settings.SunColorFloat = loadedPreset.SunColorFloat;
                Main.settings.MoonColorFloat = loadedPreset.MoonColorFloat;
                Main.settings.AmbientLightFloat = loadedPreset.AmbientLightFloat;
                Main.settings.sunSpaceEmission = loadedPreset.sunSpaceEmission;
                Main.settings.moonSpaceEmission = loadedPreset.moonSpaceEmission;
                Main.settings.SunIndirectLight = loadedPreset.SunIndirectLight;
                Main.settings.SunIndirectSpecular = loadedPreset.SunIndirectSpecular;
                Main.settings.sunAngularDiameter = loadedPreset.sunAngularDiameter;
                Main.settings.MoonIndirectLight = loadedPreset.MoonIndirectLight;
                Main.settings.MoonIndirectSpecular = loadedPreset.MoonIndirectSpecular;
                Main.settings.moonAngularDiameter = loadedPreset.moonAngularDiameter;
                Main.settings.VolWeightfloat = loadedPreset.VolWeightfloat;

                // Display success message
                MessageSystem.QueueMessage(MessageDisplayData.Type.Success, $"{PresetToLoad} Preset Applied", 2.5f);
            }
        }

        public string[] GetPresetNames()
        {
            string[] NullState = { defaulState };

            if (!string.IsNullOrEmpty(mainPath))
            {
                string[] jsons = Directory.GetFiles(Path.Combine(mainPath, "CyclePresets"), "*.json");
                string[] names = jsons.Select(Path.GetFileNameWithoutExtension).ToArray();

                return names.Length > 0 ? names : NullState;
            }

            return NullState;
        }
    }
}
