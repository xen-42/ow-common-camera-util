using HarmonyLib;

namespace CommonCameraUtil.Patches;

[HarmonyPatch(typeof(NomaiRemoteCameraPlatform))]
internal class NomaiRemoteCameraPlatformPatches
{
	[HarmonyPrefix]
	[HarmonyPatch(nameof(NomaiRemoteCameraPlatform.SwitchToPlayerCamera))]
	public static void NomaiRemoteCameraPlatform_SwitchToPlayerCamera(NomaiRemoteCameraPlatform __instance) =>
		CommonCameraUtil.Instance.RemoveCamera(__instance._slavePlatform._ownedCamera._camera);
}
