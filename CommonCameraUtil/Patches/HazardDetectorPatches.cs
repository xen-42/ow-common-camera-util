using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonCameraUtil.Patches
{
    [HarmonyPatch]
    public static class HazardDetectorPatches
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(HazardDetector), nameof(HazardDetector.OnVolumeAdded))]
        public static bool HazardDetector_OnVolumeAdded(HazardDetector __instance, EffectVolume eVolume)
        {
            // Only doing this to the player
            if (__instance.Equals(Locator.GetPlayerDetector().GetComponent<HazardDetector>()))
            {
                // Only doing it when in a fire
                HazardVolume hazardVolume = eVolume as HazardVolume;
                if (hazardVolume.GetHazardType() == HazardVolume.HazardType.FIRE)
                {
                    PlayerMeshHandler.InGreenFire = !hazardVolume.transform.parent.name.Equals("Effects_HEA_Campfire");
                }
            }

            return true;
        }
    }
}
