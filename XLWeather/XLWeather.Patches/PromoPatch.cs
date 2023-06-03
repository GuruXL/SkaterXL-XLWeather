using HarmonyLib;
using UnityEngine;
using GameManagement;

namespace XLWeather.Patches
{
    [HarmonyPatch(typeof(PromotionController))]
    [HarmonyPatch("Update")]
    public class PromoPatch
    {
        private static void Postfix(ref PromotionController __instance)
        {
            if (GameStateMachine.Instance.CurrentState.GetType() == typeof(PauseState) && __instance.mainMenuBanner.activeSelf == true)
            {
                __instance.mainMenuBanner.gameObject.SetActive(false);
            }
        }
    }
}