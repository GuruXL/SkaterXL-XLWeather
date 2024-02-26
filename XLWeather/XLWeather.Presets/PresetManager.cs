using System;
using System.Linq;
using System.Collections;
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
        private readonly string defaultState = "Select Preset to Load";
        public string PresetToLoad { get; set; }
        public string LastPresetLoaded { get; private set; }
        public string LastPresetApplied { get; private set; } = "";

        public bool isPresetLoaded { get; private set; } = false;
        public bool presetFailedToLoad { get; private set; } = false;
        public bool isPresetApplied { get; private set; } = false;
        public bool saveFailed { get; private set; } = false;

        public string saveFailedMessage { get; private set; } = "";
        public bool saveSucess { get; private set; } = false;
        public string LastPresetSaved { get; private set; } = "";

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
            ResetBoolStates();
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
        private void ResetBoolStates()
        {
            isPresetLoaded = false;
            isPresetApplied = false;
            presetFailedToLoad = false;
            saveFailed = false;
        }
        public void ResetPresetList()
        {
            PresetToLoad = defaultState;
            LastPresetLoaded = defaultState;
        }

        public bool loadingConditions()
        {
            if (PresetToLoad != LastPresetLoaded && PresetToLoad != defaultState)
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
                isPresetLoaded = true;
                presetFailedToLoad = false;
            }
            else
            {
                MessageSystem.QueueMessage(MessageDisplayData.Type.Error, $"{PresetToLoad} Preset file not found.", 2.5f);
                isPresetLoaded = false;
                presetFailedToLoad = true;
            }
        }
        public void SavePreset()
        {
            saveFailedMessage = "";

            if (PresetName.Length <= 0)
            {
                saveFailed = true;
                saveFailedMessage = "Save Failed: preset needs a name";
                MessageSystem.QueueMessage(MessageDisplayData.Type.Error, $"{saveFailedMessage}", 2.5f);
                StartCoroutine(ResetSave());
                return;
            }
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

            string filePath = Path.Combine(mainPath, "CyclePresets", $"{PresetName}.json"); // Store the file path in a variable for reusability
            string json = JsonConvert.SerializeObject(savePreset, Formatting.Indented); // Serialize the object to JSON

            try
            {
                File.WriteAllText(filePath, json); // Write the JSON to a file

                // Check if the file has been created successfully
                if (File.Exists(filePath))
                {
                    saveSucess = true;
                    StartCoroutine(ResetSave());
                    MessageSystem.QueueMessage(MessageDisplayData.Type.Success, $"{PresetName} Preset Created", 2.5f);
                    LastPresetSaved = PresetName;
                }
                else
                {
                    saveFailed = true;
                    saveFailedMessage = "Preset save failed: File creation was not successful";
                    MessageSystem.QueueMessage(MessageDisplayData.Type.Error, $"{saveFailedMessage}", 2.5f);
                    StartCoroutine(ResetSave());
                }
            }
            catch (Exception ex)
            {
                // Catch any exceptions during file writing and log them
                saveFailed = true;
                saveFailedMessage = $"Preset save failed: {ex.Message}";
                MessageSystem.QueueMessage(MessageDisplayData.Type.Error, $"{saveFailedMessage}", 2.5f);
                StartCoroutine(ResetSave());
            }
            finally
            {
                PresetName = ""; // Reset the PresetName in all cases
            }
        }

        public void ApplyPreset()
        {
            if (PresetToLoad != defaultState)
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


                isPresetLoaded = false;
                isPresetApplied = true;
                LastPresetApplied = LastPresetLoaded;
                StartCoroutine(ResetApply());
            }
        }
        private IEnumerator ResetApply()
        {
            yield return new WaitForSeconds(5);
            isPresetApplied = false;
        }
        private IEnumerator ResetSave()
        {
            yield return new WaitForSeconds(3);
            saveFailed = false;
            saveSucess = false;
        }
        public string[] GetPresetNames()
        {
            string[] NullState = { defaultState };

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
