using CommonCameraUtil.API;
using CommonCameraUtil.Components;
using CommonCameraUtil.Handlers;
using HarmonyLib;
using OWML.ModHelper;
using OWML.Utils;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.PostProcessing;
using UnityEngine.SceneManagement;

namespace CommonCameraUtil
{
    public class CommonCameraUtil : ModBehaviour
    {
        public static CommonCameraUtil Instance;

        private List<OWCamera> _customCameras = new List<OWCamera>();
        private bool _usingCustomCamera;

        public class CameraEvent<T> : UnityEvent<T> { }
        public UnityEvent<PlayerTool> EquipTool = new CameraEvent<PlayerTool>();
        public UnityEvent<PlayerTool> UnequipTool = new CameraEvent<PlayerTool>();

        private List<OWCamera> _uninitializedCameras = new List<OWCamera>();

        public override object GetApi()
        {
            return new CommonCameraAPI();
        }

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            // Patches
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());

            GlobalMessenger<OWCamera>.AddListener("SwitchActiveCamera", OnSwitchActiveCamera);

            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        public void OnDestroy()
        {
            GlobalMessenger<OWCamera>.RemoveListener("SwitchActiveCamera", OnSwitchActiveCamera);

            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void Update()
        {

        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name != "SolarSystem" && scene.name != "EyeOfTheUniverse") return;

            _uninitializedCameras.Clear();

            ModHelper.Events.Unity.FireOnNextUpdate(Init);
        }

        private void Init()
        {
            Locator.GetPlayerBody().gameObject.AddComponent<PlayerMeshHandler>();
            Locator.GetPlayerBody().gameObject.AddComponent<HeadRotation>();
        }

        private void OnSwitchActiveCamera(OWCamera camera)
        {
            if (_uninitializedCameras.Contains(camera))
            {
                InitCustomCamera(camera.name, camera, camera.mainCamera, camera.gameObject);
                _uninitializedCameras.Remove(camera);
            }

            _usingCustomCamera = _customCameras.Contains(camera);
        }

        public void RegisterCustomCamera(OWCamera camera)
        {
            // We add our camera to the list of cameras in daydream so it shows the sky correctly too
            try
            {
                ModHelper.Interaction.GetMod("xen.DayDream").GetValue<List<OWCamera>>("Cameras").Add(camera);
            }
            catch { }

            _customCameras.Add(camera);
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

                // CopyFrom will overwrite the transform stuff which is annoying
                var rot = camera.transform.rotation;
                var pos = camera.transform.position;
                camera.CopyFrom(Locator.GetPlayerCamera().mainCamera);
                camera.transform.rotation = rot;
                camera.transform.position = pos;

                cameraObject.name = name;

                cameraObject.SetActive(true);
            }
            catch (Exception ex)
            {
                Util.WriteError($"Could not initialize camera [{OWCamera?.name}] : {ex.Message}, {ex.StackTrace}");
            }
        }

        public static bool UsingCustomCamera()
        {
            return Instance._usingCustomCamera;
        }

        public static bool IsCustomCamera(OWCamera camera)
        {
            return Instance._customCameras.Contains(camera);
        }
    }
}
