using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;
using UnityEngine.SceneManagement;

namespace CommonCameraUtil.Handlers
{
    public class CameraCreationHandler
    {
        private List<OWCamera> _uninitializedCameras = new List<OWCamera>();

        public static CameraCreationHandler Instance;

        public CameraCreationHandler()
        {
            Instance = this;

            SceneManager.sceneLoaded += OnSceneLoaded;
            GlobalMessenger<OWCamera>.AddListener("SwitchActiveCamera", OnSwitchActiveCamera);
        }

        public void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            GlobalMessenger<OWCamera>.RemoveListener("SwitchActiveCamera", OnSwitchActiveCamera);
        }

        public void OnSceneLoaded(Scene scene, LoadSceneMode _)
        {
            if (scene.name != "SolarSystem" && scene.name != "EyeOfTheUniverse") return;

            _uninitializedCameras.Clear();
        }

        public void OnSwitchActiveCamera(OWCamera camera)
        {
            if (_uninitializedCameras.Contains(camera))
            {
                InitCustomCamera(camera.name, camera, camera.mainCamera, camera.gameObject);
                _uninitializedCameras.Remove(camera);
            }
        }

        public (OWCamera, Camera) CreateCustomCamera(string name)
        {
            Util.Write($"Creating custom camera [{name}]");

            var cameraObject = new GameObject(name);
            cameraObject.SetActive(false);

            var camera = cameraObject.AddComponent<Camera>();
            camera.enabled = false;

            var OWCamera = cameraObject.AddComponent<OWCamera>();
            OWCamera.renderSkybox = true;

            CommonCameraUtil.Instance.RegisterCustomCamera(OWCamera);

            _uninitializedCameras.Add(OWCamera);

            return (OWCamera, camera);
        }

        private void InitCustomCamera(string name, OWCamera OWCamera, Camera camera, GameObject cameraObject)
        {
            Util.Write($"Initializing custom camera [{OWCamera?.name}]");

            try
            {
                cameraObject.SetActive(false);

                var downSampleShader = Locator.GetPlayerCamera()?.gameObject?.GetComponent<FlashbackScreenGrabImageEffect>()?._downsampleShader;
                if (downSampleShader != null)
                {
                    var temp = cameraObject.AddComponent<FlashbackScreenGrabImageEffect>();
                    temp._downsampleShader = downSampleShader;
                }

                var fogShader = Locator.GetPlayerCamera()?.gameObject?.GetComponent<PlanetaryFogImageEffect>()?.fogShader;
                if (fogShader != null)
                {
                    var _image = cameraObject.AddComponent<PlanetaryFogImageEffect>();
                    _image.fogShader = fogShader;
                }

                var profile = Locator.GetPlayerCamera()?.gameObject?.GetComponent<PostProcessingBehaviour>()?.profile;
                if (profile != null)
                {
                    var _postProcessiong = cameraObject.AddComponent<PostProcessingBehaviour>();
                    _postProcessiong.profile = profile;
                }

                camera.CopyFrom(Locator.GetPlayerCamera().mainCamera);

                cameraObject.name = name;

                cameraObject.SetActive(true);
            }
            catch (Exception ex)
            {
                Util.WriteError($"Could not initialize camera [{OWCamera?.name}] : {ex.Message}, {ex.StackTrace}");
            }
        }
    }
}
