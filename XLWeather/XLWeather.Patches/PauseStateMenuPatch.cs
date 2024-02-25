using HarmonyLib;
using UnityEngine;
using GameManagement;

namespace XLWeather.Patches
{
    [HarmonyPatch(typeof(PauseState))]
    [HarmonyPatch("OnEnter")]
    public class PauseStateMenuPatch
    {
        private static void Postfix(ref PauseState __instance)
        {
            RemoveDLCButton();

            __instance.StateMachine.PauseObject.SetActive(false);
            __instance.StateMachine.PauseObject.SetActive(true);

        }

        private static void RemoveDLCButton()
        {
            // remove DLC button :)
            if (PromotionController.Instance != null)
            {
                GameObject mainMenuBanner = Traverse.Create(PromotionController.Instance).Field("mainMenuBanner").GetValue<GameObject>();
                if (mainMenuBanner != null && mainMenuBanner.activeSelf)
                {
                    mainMenuBanner.SetActive(false);
                }
            }
        }

    }
}