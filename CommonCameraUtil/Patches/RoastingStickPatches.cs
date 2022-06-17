﻿using CommonCameraUtil.Components;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonCameraUtil.Patches
{
    [HarmonyPatch]
    public static class RoastingStickPatches
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(RoastingStickController), nameof(RoastingStickController.OnEnterRoastingMode))]
        public static void RoastingStickController_OnEnterRoastingMode()
        {
            RoastingStickIK.IsRoasting = true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(RoastingStickController), nameof(RoastingStickController.OnExitRoastingMode))]
        public static void RoastingStickController_OnExitRoastingMode()
        {
            RoastingStickIK.IsRoasting = false;
        }
    }
}
