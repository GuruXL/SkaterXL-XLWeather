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
        //public static float HighestPrio;
        public static float GetHighestPrio(Volume[] volumes)
        {
            float HighestPrio = 0f;

            if (volumes != null && volumes.Length > 0)
            {
                HighestPrio = Math.Max(volumes.Select(v => v.priority).Max(), 0);
                return HighestPrio;
            }
            return HighestPrio;
        }
        public static void CheckPrio(Volume[] volumes)
        {
            float prio = GetHighestPrio(volumes);

            if (Main.Cyclectrl.GetSunVolumePrio() < prio)
            {
                Main.Cyclectrl.SetCycleVolPrio(prio);
                Main.Weatherctrl.SetSkyVolumePrio(prio);
            }
        }

    }
}
