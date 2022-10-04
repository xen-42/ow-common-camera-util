﻿using CommonCameraUtil.API;
using CommonCameraUtil.Components;
using CommonCameraUtil.Handlers;
using HarmonyLib;
using OWML.ModHelper;
using OWML.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.PostProcessing;
using UnityEngine.SceneManagement;

namespace CommonCameraUtil;

public class CommonCameraUtil : ModBehaviour
{
	public static CommonCameraUtil Instance;

	private List<OWCamera> _customCameras = new();

	private bool _usingCustomCamera;

	public class CameraEvent<T> : UnityEvent<T> { }
	public UnityEvent<PlayerTool> EquipTool = new CameraEvent<PlayerTool>();
	public UnityEvent<PlayerTool> UnequipTool = new CameraEvent<PlayerTool>();

	private Dictionary<OWCamera, Action<OWCamera>> _uninitializedCameras = new();

	public StackList<OWCamera> CameraStack { get; private set; } = new();

	private bool _fireEventNextTick = false;
	private OWCamera _nextCamera = null;
	private OWCamera _currentCamera = null;

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

	private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		if (scene.name != "SolarSystem" && scene.name != "EyeOfTheUniverse") return;

		_nextCamera = null;
		_fireEventNextTick = false;

		CameraStack.Clear();
		_uninitializedCameras.Clear();

		// Can't use locator yet because it isnt set up yet
		var player = GameObject.Find("Player_Body");
		player.AddComponent<PlayerMeshHandler>();
		player.AddComponent<HeadRotation>();
		player.AddComponent<RoastingStickAnim>();
		player.AddComponent<ToolMaterialHandler>();
	}

	private void OnSwitchActiveCamera(OWCamera camera)
	{
		if (_uninitializedCameras.ContainsKey(camera))
		{
			InitCustomCamera(camera.name, camera, camera.mainCamera, camera.gameObject);
			_uninitializedCameras[camera]?.Invoke(camera);
			_uninitializedCameras.Remove(camera);
		}

		if (camera.name == "MapCamera")
		{
			CameraStack.Add(camera);
			_usingCustomCamera = false;
		}
		else if (camera.name == "RemoteViewerCamera" || _customCameras.Contains(camera))
		{
			_usingCustomCamera = true;
			CameraStack.Add(camera);
		}
		else
		{
			_usingCustomCamera = false;
		}

		_currentCamera = camera;
	}

	public void RemoveCamera(OWCamera camera) => CameraStack.Remove(camera);

	public void OnExitMapView() => _customCameras.Remove(Locator.GetMapController()._mapCamera);

	public void RegisterCustomCamera(OWCamera camera)
	{
		// We add our camera to the list of cameras in daydream so it shows the sky correctly too
		try
		{
			ModHelper.Interaction.TryGetMod("xen.DayDream").GetValue<List<OWCamera>>("Cameras").Add(camera);
		}
		catch { }

		_customCameras.Add(camera);
	}

	public (OWCamera, Camera) CreateCustomCamera(string name, Action<OWCamera> postInit = null)
	{
		Util.Write($"Creating custom camera [{name}]");

		var cameraObject = new GameObject(name);
		cameraObject.SetActive(false);

		var camera = cameraObject.AddComponent<Camera>();
		camera.enabled = false;

		var OWCamera = cameraObject.AddComponent<OWCamera>();
		OWCamera.renderSkybox = true;

		RegisterCustomCamera(OWCamera);

		_uninitializedCameras.Add(OWCamera, postInit);

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

	public void ExitCamera(OWCamera OWCamera)
	{
		CameraStack.Remove(OWCamera);

		if (Locator.GetActiveCamera() == OWCamera)
		{
			var nextCamera = CameraStack.Peek();
			if (nextCamera == null) nextCamera = Locator.GetPlayerCamera();

			GlobalMessenger<OWCamera>.FireEvent("SwitchActiveCamera", nextCamera);
			OWCamera.mainCamera.enabled = false;
			nextCamera.mainCamera.enabled = true;
		}
	}

	public void EnterCamera(OWCamera OWCamera)
	{
		var currentCamera = Locator.GetActiveCamera();

		if (currentCamera != OWCamera)
		{
			try
			{
				GlobalMessenger<OWCamera>.FireEvent("SwitchActiveCamera", OWCamera);
			}
			catch (Exception)
			{
				_fireEventNextTick = true;
				_nextCamera = OWCamera;
			}

			currentCamera.mainCamera.enabled = false;
			OWCamera.mainCamera.enabled = true;
		}
	}

	public void Update()
	{
		if (_fireEventNextTick)
		{
			GlobalMessenger<OWCamera>.FireEvent("SwitchActiveCamera", _nextCamera);
			_nextCamera = null;
			_fireEventNextTick = false;
		}
	}


	public static bool UsingCustomCamera() => Instance._usingCustomCamera;

	public static bool IsCustomCamera(OWCamera camera) => Instance._customCameras.Contains(camera);
}
