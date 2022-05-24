using OWML.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.PostProcessing;

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
    }
}
