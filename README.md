# Outer Wilds Common Camera Utility

![common camera utility thumbnail](https://user-images.githubusercontent.com/22628069/190283897-380af8ee-2e51-47f2-bdb7-5c4dcfe282da.png)

A utility mod for setting up cameras in Outer Wilds. Also implements some QOL changes for third person perspectives:
- Adds ghost model in dream world
- Stops arm from disappearing when using tools
- Puts the player's head back onto a layer where it can be seen.
- Renders held tools properly
- Handles compatibility between your mod and other CCU mods, and with [Day Dream](https://outerwildsmods.com/mods/daydream/).

## For use in other mods:

Include this interface in your mod.
```cs
public interface ICommonCameraAPI
{
    void RegisterCustomCamera(OWCamera OWCamera);
    (OWCamera, Camera) CreateCustomCamera(string name);
    UnityEvent<PlayerTool> EquipTool();
    UnityEvent<PlayerTool> UnequipTool();
    void ExitCamera(OWCamera OWCamera);
	void EnterCamera(OWCamera OWCamera);
}
```

Then to use the API from another class:
```cs
var CommonCameraAPI = ModHelper.Interaction.TryGetModApi<ICommonCameraAPI>("xen.CommonCameraUtility");
```

If you want to manually set up the camera yourself, be sure to call `RegisterCustomCamera` on it. However it's easier to just use `CreateCustomCamera` and the utility will set it up for you. Also includes some helpful events to hook on to.

To switch to your camera, use the `EnterCamera` method. To exit your camera, use the `ExitCamera` method. CCU will handle bringing the player back to the appropriate camera, be it from the base game or from another CCU mod.
