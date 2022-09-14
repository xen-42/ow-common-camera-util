using UnityEngine;
using UnityEngine.Events;

namespace CommonCameraUtil.API;

public class CommonCameraAPI : ICommonCameraAPI
{
    public (OWCamera, Camera) CreateCustomCamera(string name) => 
        CommonCameraUtil.Instance.CreateCustomCamera(name);

    public void RegisterCustomCamera(OWCamera OWCamera) => 
        CommonCameraUtil.Instance.RegisterCustomCamera(OWCamera);

    public UnityEvent<PlayerTool> EquipTool() => 
        CommonCameraUtil.Instance.EquipTool;

    public UnityEvent<PlayerTool> UnequipTool() => 
        CommonCameraUtil.Instance.UnequipTool;
}
