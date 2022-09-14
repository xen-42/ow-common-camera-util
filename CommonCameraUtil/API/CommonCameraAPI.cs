using UnityEngine;
using UnityEngine.Events;

namespace CommonCameraUtil.API;

public class CommonCameraAPI : ICommonCameraAPI
{
    public (OWCamera, Camera) CreateCustomCamera(string name)
    {
        return CommonCameraUtil.Instance.CreateCustomCamera(name);
    }

    public void RegisterCustomCamera(OWCamera OWCamera)
    {
        CommonCameraUtil.Instance.RegisterCustomCamera(OWCamera);
    }

    public UnityEvent<PlayerTool> EquipTool()
    {
        return CommonCameraUtil.Instance.EquipTool;
    }

    public UnityEvent<PlayerTool> UnequipTool()
    {
        return CommonCameraUtil.Instance.UnequipTool;
    }
}
