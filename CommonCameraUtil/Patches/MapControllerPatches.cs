using HarmonyLib;

namespace CommonCameraUtil.Patches;

[HarmonyPatch(typeof(MapController))]
internal class MapControllerPatches
{
	// Has to happen at the end hence why we don't use the event
	[HarmonyPostfix]
	[HarmonyPatch(nameof(MapController.ExitMapView))]
	public static void MapController_ExitMapView(MapController __instance)
	{
		CommonCameraUtil.Instance.RemoveCamera(__instance._mapCamera);

		var nextCamera = CommonCameraUtil.Instance.CameraStack.Pop();

		if (nextCamera != null)
		{
			CommonCameraUtil.Instance.ModHelper.Events.Unity.FireOnNextUpdate(() => CommonCameraUtil.Instance.EnterCamera(nextCamera));
		}
	}

	// It tracks what the last open camera was, we want to stop it from tracking any of our cameras because we do that ourselves
	[HarmonyPrefix]
	[HarmonyPatch(nameof(MapController.OnSwitchActiveCamera))]
	public static bool MapController_OnSwitchActiveCamera(OWCamera activeCam) => !CommonCameraUtil.IsCustomCamera(activeCam);
}
