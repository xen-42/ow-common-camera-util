using System;
using UnityEngine;
using UnityEngine.Events;
using static CommonCameraUtil.API.ICommonCameraAPI;

namespace CommonCameraUtil.API
{
    public class CommonCameraAPI : ICommonCameraAPI
    {
        public (OWCamera, Camera) CreateCustomCamera(string name)
        {
            return CameraCreationHandler.Instance.CreateCustomCamera(name);
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
}
