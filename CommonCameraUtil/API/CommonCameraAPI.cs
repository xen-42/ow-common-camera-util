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
        public OWCamera CreateCustomCamera(string name)
        {
            Util.Write($"Creating custom camera [{name}]");

            var cameraObject = new GameObject(name);
            cameraObject.SetActive(false);

            var camera = cameraObject.AddComponent<Camera>();
            camera.enabled = false;

            var OWCamera = cameraObject.AddComponent<OWCamera>();
            OWCamera.renderSkybox = true;

            CommonCameraUtil.Instance.Initialize.AddListener(() => InitCustomCamera(OWCamera, camera, cameraObject));

            CommonCameraUtil.Instance.RegisterCustomCamera(OWCamera);

            return OWCamera;
        }

        private void InitCustomCamera(OWCamera OWCamera, Camera camera, GameObject cameraObject)
        {
            Util.Write($"Initializing custom camera [{OWCamera?.name}]");

            try
            {
                var temp = cameraObject.AddComponent<FlashbackScreenGrabImageEffect>();
                temp._downsampleShader = Locator.GetPlayerCamera().gameObject.GetComponent<FlashbackScreenGrabImageEffect>()._downsampleShader;

                var _image = cameraObject.AddComponent<PlanetaryFogImageEffect>();
                _image.fogShader = Locator.GetPlayerCamera().gameObject.GetComponent<PlanetaryFogImageEffect>().fogShader;

                var _postProcessiong = cameraObject.AddComponent<PostProcessingBehaviour>();
                _postProcessiong.profile = Locator.GetPlayerCamera().gameObject.GetComponent<PostProcessingBehaviour>().profile;

                cameraObject.SetActive(true);
                camera.CopyFrom(Locator.GetPlayerCamera().mainCamera);
            }
            catch (Exception ex)
            {
                Util.WriteError($"Could not initialize camera [{OWCamera?.name}] : {ex.Message}, {ex.StackTrace}");
            }

            CommonCameraUtil.Instance.Initialize.RemoveListener(() => InitCustomCamera(OWCamera, camera, cameraObject));
        }

        public void RegisterCustomCamera(OWCamera OWCamera)
        {
            CommonCameraUtil.Instance.RegisterCustomCamera(OWCamera);
        }
    }
}
