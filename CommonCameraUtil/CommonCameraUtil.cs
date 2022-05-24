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
        public static PlayerMeshHandler PlayerMeshHandler { get; private set; }
        public static ToolMaterialHandler ToolMaterialHandler { get; private set; }

        private List<OWCamera> _customCameras = new List<OWCamera>();
        private bool _usingCustomCamera;

        public UnityEvent PreInitialize = new UnityEvent();
        public UnityEvent Initialize = new UnityEvent();

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

            PlayerMeshHandler = new PlayerMeshHandler();
            ToolMaterialHandler = new ToolMaterialHandler();
            GlobalMessenger<OWCamera>.AddListener("SwitchActiveCamera", OnSwitchActiveCamera);

            Initialize.AddListener(Init);

            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        public void OnDestroy()
        {
            PlayerMeshHandler?.OnDestroy();
            ToolMaterialHandler?.OnDestroy();
            GlobalMessenger<OWCamera>.RemoveListener("SwitchActiveCamera", OnSwitchActiveCamera);

            Initialize.RemoveListener(Init);

            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void Update()
        {
            PlayerMeshHandler?.Update();
            ToolMaterialHandler?.Update();
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            PreInitialize.Invoke();
            ModHelper.Events.Unity.FireOnNextUpdate(Initialize.Invoke);
        }

        private void Init()
        {
            PlayerMeshHandler?.Init();
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
    }
}
