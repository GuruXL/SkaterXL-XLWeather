using UnityEngine;
using RapidGUI;
using System.Collections;
using XLWeather.Controller;
using UnityEngine.VFX;
using UnityEngine.VFX.Utility;
using XLWeather.Data;
using System;
using UnityEngine.Rendering;
using System.Linq;

namespace XLWeather.Utils
{
    public class PrioCheck
    {
        public static float HighestPrio;

        public static float GetHighestPrio(Volume[] volumes)
        {
            HighestPrio = 0f;

            if (volumes != null && volumes.Length > 0)
            {
                HighestPrio = Math.Max(volumes.Select(v => v.priority).Max(), 0);
                return HighestPrio;
            }
            return HighestPrio;
        }

        public static void CheckPrio()
        {
            if (Main.Cyclectrl.GetSunVolumePrio() < HighestPrio)
            {
                Main.Cyclectrl.SetCycleVolPrio(HighestPrio);
                Main.Weatherctrl.SetSkyVolumePrio(HighestPrio);
            }
        }

    }
}
