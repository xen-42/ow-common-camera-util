using CommonCameraUtil.Components;
using HarmonyLib;

namespace CommonCameraUtil.Patches;

[HarmonyPatch]
public static class RoastingStickPatches
{
	[HarmonyPrefix]
	[HarmonyPatch(typeof(RoastingStickController), nameof(RoastingStickController.OnEnterRoastingMode))]
	public static void RoastingStickController_OnEnterRoastingMode()
	{
		RoastingStickAnim.IsRoasting = true;
	}

	[HarmonyPrefix]
	[HarmonyPatch(typeof(RoastingStickController), nameof(RoastingStickController.OnExitRoastingMode))]
	public static void RoastingStickController_OnExitRoastingMode()
	{
		RoastingStickAnim.IsRoasting = false;
	}
}
