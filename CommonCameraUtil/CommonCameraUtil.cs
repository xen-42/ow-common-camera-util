using CommonCameraUtil.API;
using HarmonyLib;
using OWML.Common;
using OWML.ModHelper;
using OWML.Utils;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace CommonCameraUtil
{
    public class CommonCameraUtil : ModBehaviour
    {
        public static CommonCameraUtil Instance;

        private PlayerMeshHandler _playerMeshHandler;
        private ToolMaterialHandler _toolMaterialHandler;
        private CameraCreationHandler _cameraCreationHandler;

        private List<OWCamera> _customCameras = new List<OWCamera>();
        private bool _usingCustomCamera;

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

            _playerMeshHandler = new PlayerMeshHandler();
            _toolMaterialHandler = new ToolMaterialHandler();
            _cameraCreationHandler = new CameraCreationHandler();

            GlobalMessenger<OWCamera>.AddListener("SwitchActiveCamera", OnSwitchActiveCamera);

            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        public void OnDestroy()
        {
            _playerMeshHandler?.OnDestroy();
            _toolMaterialHandler?.OnDestroy();
            _cameraCreationHandler?.OnDestroy();
            GlobalMessenger<OWCamera>.RemoveListener("SwitchActiveCamera", OnSwitchActiveCamera);

            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void Update()
        {
            _playerMeshHandler?.Update();
            _toolMaterialHandler?.Update();
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            ModHelper.Events.Unity.FireOnNextUpdate(Init);
        }

        private void Init()
        {
            _playerMeshHandler?.Init();
        }

        private void OnSwitchActiveCamera(OWCamera camera)
        {
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
