using HarmonyLib;

namespace CommonCameraUtil.Patches;

[HarmonyPatch(typeof(ShipCockpitController))]
internal class ShipCockpitControllerPatches
{
	// It tries putting us into the player camera but we dont want that
	[HarmonyPostfix]
	[HarmonyPatch(nameof(ShipCockpitController.ExitLandingView))]
	public static void ShipCockpitController_ExitLandingView(ShipCockpitController __instance)
	{
		CommonCameraUtil.Instance.RemoveCamera(__instance._playerCam);
		CommonCameraUtil.Instance.RemoveCamera(__instance._landingCam.owCamera);

		var nextCamera = CommonCameraUtil.Instance.CameraStack.Pop();

		if (nextCamera != null)
		{
			CommonCameraUtil.Instance.ModHelper.Events.Unity.FireOnNextUpdate(() => CommonCameraUtil.Instance.EnterCamera(nextCamera));
		}
	}
}
