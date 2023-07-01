﻿using UnityEngine;
using RapidGUI;
using System.Collections;
using XLWeather.Controller;
using UnityEngine.VFX;
using UnityEngine.VFX.Utility;
using XLWeather.Data;
using System;

namespace XLWeather.Utils
{
    public class VFXUtils
    {
        public class VFXStateCheck
        {
            private bool vfxStopped = false;
            public void UpdateVFXState(float settingsValue, VisualEffect vfx)
            {
                if (settingsValue == 0.0f && !vfxStopped)
                {
                    vfx.Stop();
                    vfxStopped = true;
                }
                else if (settingsValue > 0.0f && vfxStopped)
                {
                    vfx.Play();
                    vfxStopped = false;
                }
            }
        }
    }
}
