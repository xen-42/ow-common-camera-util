using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonCameraUtil.Patches
{
    [HarmonyPatch]
    public static class PlayerToolPatches
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(PlayerTool), nameof(PlayerTool.EquipTool))]
        public static void PlayerTool_EquipTool(PlayerTool __instance)
        {
            CommonCameraUtil.Instance.EquipTool.Invoke(__instance);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(PlayerTool), nameof(PlayerTool.UnequipTool))]
        public static void PlayerTool_UnequipTool(PlayerTool __instance)
        {
            CommonCameraUtil.Instance.UnequipTool.Invoke(__instance);
        }
    }
}
