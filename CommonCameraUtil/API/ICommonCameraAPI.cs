using System;
using UnityEngine;

namespace CommonCameraUtil.API
{
    public interface ICommonCameraAPI
    {
        void RegisterCustomCamera(OWCamera OWCamera);
        (OWCamera, Camera) CreateCustomCamera(string name);
    }
}
