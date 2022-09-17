using System;
using UnityEngine;
using UnityEngine.Events;

namespace CommonCameraUtil.API;

public class CommonCameraAPI : ICommonCameraAPI
{
    public (OWCamera, Camera) CreateCustomCamera(string name) => 
        CommonCameraUtil.Instance.CreateCustomCamera(name);

	public (OWCamera, Camera) CreateCustomCamera(string name, Action<OWCamera> postInitMethod) =>
	    CommonCameraUtil.Instance.CreateCustomCamera(name, postInitMethod);

	public void RegisterCustomCamera(OWCamera OWCamera) => 
        CommonCameraUtil.Instance.RegisterCustomCamera(OWCamera);

    public void ExitCamera(OWCamera OWCamera) =>
        CommonCameraUtil.Instance.ExitCamera(OWCamera);

	public void EnterCamera(OWCamera OWCamera) =>
	    CommonCameraUtil.Instance.EnterCamera(OWCamera);

	public UnityEvent<PlayerTool> EquipTool() => 
        CommonCameraUtil.Instance.EquipTool;

    public UnityEvent<PlayerTool> UnequipTool() => 
        CommonCameraUtil.Instance.UnequipTool;
}
