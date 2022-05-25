# Outer Wilds Common Camera Utility

A utility mod for setting up cameras in Outer Wilds. Also implements some QOL changes for third person perspectives:
- Adds ghost model in dream world
- Stops arm from disappearing when using tools
- Puts the player's head back onto a layer where it can be seen.
- Renders held tools properly
- Adjusts custom cameras for [Day Dream](https://outerwildsmods.com/mods/daydream/) compatibility.

## For use in other mods:

Include this interface in your mod.
```cs
public interface ICommonCameraAPI
{
    void RegisterCustomCamera(OWCamera OWCamera);
    (OWCamera, Camera) CreateCustomCamera(string name);
    UnityEvent<PlayerTool> EquipTool();
    UnityEvent<PlayerTool> UnequipTool();
}
```

Then to use the API from another class:
```cs
var CommonCameraAPI = ModHelper.Interaction.GetModApi<ICommonCameraAPI>("xen.CommonCameraUtility");
```

If you want to manually set up the camera yourself, be sure to call `RegisterCustomCamera` on it. However it's easier to just use `CreateCustomCamera` and the utility will set it up for you. Also includes some helpful events to hook on to.

To switch to your camera, do something like this (where your camera is called `camera` and your `OWCamera` is called `OWCamera`:
```cs
previousCamera = Locator.GetActiveCamera();
previousCamera.mainCamera.enabled = false;
camera.enabled = true;
GlobalMessenger<OWCamera>.FireEvent("SwitchActiveCamera", OWCamera);
```

Keeping a reference to the previous camera is useful so that when you want to disable your camera you can set it back to use the previous one.

### Credits

Uses [free controller and keyboard prompts](https://thoseawesomeguys.com/prompts/) from Xelu, just like stock Outer Wilds.
