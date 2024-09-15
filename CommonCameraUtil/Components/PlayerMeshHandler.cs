using CommonCameraUtil.API;
using System;
using UnityEngine;

namespace CommonCameraUtil.Components;

public class PlayerMeshHandler : MonoBehaviour
{
	private bool _isToolHeld = false;

	private MeshRenderer _fireRenderer, _dreamFireRenderer, _activeFireRenderer;
	private HazardDetector _hazardDetector;
	private float fade = 0f;
	private bool inFire = false;

	private int _propID_Fade;

	// Body parts
	private GameObject _helmetMesh, _helmet2D, _head, _ghostHead;

	// Hand holding marshmallow stick
	private MeshRenderer _roastingStickArm, _roastingStickNoSuit;

	private static AssetBundle assetBundle;
	private GameObject ghostPrefab;

	public static bool InGreenFire;

	private PlayerAnimController _animController;

	public void Awake()
	{
		CommonCameraUtil.Instance.EquipTool.AddListener(OnToolEquiped);
		CommonCameraUtil.Instance.UnequipTool.AddListener(OnToolUnequiped);

		GlobalMessenger.AddListener("RemoveHelmet", OnRemoveHelmet);
		GlobalMessenger.AddListener("PutOnHelmet", OnPutOnHelmet);
		GlobalMessenger<OWCamera>.AddListener("SwitchActiveCamera", OnSwitchActiveCamera);

		// Have to be precise about the path because Ultimate Skin Changer adds another PlayerAnimController
		_animController = transform.Find("Traveller_HEA_Player_v2").GetComponent<PlayerAnimController>();
	}

	public void OnDestroy()
	{
		CommonCameraUtil.Instance.EquipTool.RemoveListener(OnToolEquiped);
		CommonCameraUtil.Instance.UnequipTool.RemoveListener(OnToolUnequiped);

		GlobalMessenger.RemoveListener("RemoveHelmet", OnRemoveHelmet);
		GlobalMessenger.RemoveListener("PutOnHelmet", OnPutOnHelmet);
		GlobalMessenger<OWCamera>.RemoveListener("SwitchActiveCamera", OnSwitchActiveCamera);
	}

	public void Start()
	{
		_roastingStickArm = Util.Find("Player_Body/RoastingSystem/Stick_Root/Stick_Pivot/Stick_Tip/Props_HEA_RoastingStick/RoastingStick_Arm").GetComponent<MeshRenderer>();
		_roastingStickNoSuit = Util.Find("Player_Body/RoastingSystem/Stick_Root/Stick_Pivot/Stick_Tip/Props_HEA_RoastingStick/RoastingStick_Arm_NoSuit").GetComponent<MeshRenderer>();

		_helmet2D = GameObject.FindObjectOfType<HUDHelmetAnimator>()?.gameObject;

		_helmetMesh = Util.Find("Player_Body/Traveller_HEA_Player_v2/Traveller_Mesh_v01:Traveller_Geo/Traveller_Mesh_v01:PlayerSuit_Helmet").gameObject;
		_head = Util.Find("Player_Body/Traveller_HEA_Player_v2/player_mesh_noSuit:Traveller_HEA_Player/player_mesh_noSuit:Player_Head").gameObject;

		try
		{
			var campfire = GameObject.Find("/Moon_Body/Sector_THM/Interactables_THM/Effects_HEA_Campfire/Props_HEA_Campfire/Campfire_Flames");

			_fireRenderer = GameObject.Instantiate(campfire.GetComponent<MeshRenderer>(), Locator.GetPlayerBody().transform);
			_fireRenderer.transform.localScale = new Vector3(0.8f, 2f, 0.8f);
			_fireRenderer.transform.localPosition = new Vector3(0, -1.2f, -0.25f);
			_fireRenderer.enabled = false;
		}
		catch (Exception) { }

		try
		{
			var dreamCampfire = GameObject.Find("/RingWorld_Body/Sector_RingInterior/Sector_Zone1/Sector_DreamFireHouse_Zone1/Interactables_DreamFireHouse_Zone1/DreamFireChamber/Prefab_IP_DreamCampfire/Props_IP_DreamFire/DreamFire_Flames");
			_dreamFireRenderer = GameObject.Instantiate(dreamCampfire.GetComponent<MeshRenderer>(), Locator.GetPlayerBody().transform);
			_dreamFireRenderer.transform.localScale = new Vector3(0.8f, 2f, 0.8f);
			_dreamFireRenderer.transform.localPosition = new Vector3(0, -1.6f, -0.25f);
			_dreamFireRenderer.enabled = false;
		}
		catch (Exception) { }

		_hazardDetector = Locator.GetPlayerController().GetComponentInChildren<HazardDetector>();
		_hazardDetector.OnHazardsUpdated += OnHazardsUpdated;

		_propID_Fade = Shader.PropertyToID("_Fade");

		LoadPlayerGhost();
	}

	private void LoadPlayerGhost()
	{
		try
		{
			if (assetBundle == null)
			{
				assetBundle = CommonCameraUtil.Instance.ModHelper.Assets.LoadBundle("assets/ghost-hatchling");
			}

			if (ghostPrefab == null)
			{
				ghostPrefab = assetBundle.LoadAsset<GameObject>("Assets/Player Ghost/Ghost.prefab");
				ghostPrefab.SetActive(false);
			}

			var ghost = GameObject.Instantiate(ghostPrefab, Locator.GetPlayerTransform());
			ghost.transform.localScale = 0.25f * Vector3.one;
			ghost.transform.localPosition = Vector3.zero;
			ghost.SetActive(true);

			var backRoot = Locator.GetPlayerTransform().Find("Traveller_HEA_Player_v2/Traveller_Rig_v01:Traveller_Trajectory_Jnt/Traveller_Rig_v01:Traveller_ROOT_Jnt/Traveller_Rig_v01:Traveller_Spine_01_Jnt/Traveller_Rig_v01:Traveller_Spine_02_Jnt");
			var headRoot = Locator.GetPlayerTransform().Find("Traveller_HEA_Player_v2/Traveller_Rig_v01:Traveller_Trajectory_Jnt/Traveller_Rig_v01:Traveller_ROOT_Jnt/Traveller_Rig_v01:Traveller_Spine_01_Jnt/Traveller_Rig_v01:Traveller_Spine_02_Jnt/Traveller_Rig_v01:Traveller_Spine_Top_Jnt/Traveller_Rig_v01:Traveller_Neck_01_Jnt/Traveller_Rig_v01:Traveller_Neck_Top_Jnt/Traveller_Rig_v01:Traveller_Head_Top_Jnt");
			var leftHandRoot = Locator.GetPlayerTransform().Find("Traveller_HEA_Player_v2/Traveller_Rig_v01:Traveller_Trajectory_Jnt/Traveller_Rig_v01:Traveller_ROOT_Jnt/Traveller_Rig_v01:Traveller_Spine_01_Jnt/Traveller_Rig_v01:Traveller_Spine_02_Jnt/Traveller_Rig_v01:Traveller_Spine_Top_Jnt/Traveller_Rig_v01:Traveller_LF_Arm_Clavicle_Jnt/Traveller_Rig_v01:Traveller_LF_Arm_Shoulder_Jnt/Traveller_Rig_v01:Traveller_LF_Arm_Elbow_Jnt/Traveller_Rig_v01:Traveller_LF_Arm_Wrist_Jnt");
			var rightHandRoot = Locator.GetPlayerTransform().Find("Traveller_HEA_Player_v2/Traveller_Rig_v01:Traveller_Trajectory_Jnt/Traveller_Rig_v01:Traveller_ROOT_Jnt/Traveller_Rig_v01:Traveller_Spine_01_Jnt/Traveller_Rig_v01:Traveller_Spine_02_Jnt/Traveller_Rig_v01:Traveller_Spine_Top_Jnt/Traveller_Rig_v01:Traveller_RT_Arm_Clavicle_Jnt/Traveller_Rig_v01:Traveller_RT_Arm_Shoulder_Jnt/Traveller_Rig_v01:Traveller_RT_Arm_Elbow_Jnt/Traveller_Rig_v01:Traveller_RT_Arm_Wrist_Jnt");

			_ghostHead = ghost.transform.Find("Head").gameObject;
			var eyes = ghost.transform.Find("Eyes");
			eyes.transform.parent = _ghostHead.transform;
			_ghostHead.transform.parent = headRoot;
			_ghostHead.transform.localRotation = Quaternion.Euler(0, 0, 90);
			_ghostHead.transform.localScale = 1.25f * Vector3.one;
			_ghostHead.transform.localPosition = new Vector3(2, -1, 0);

			var torso = ghost.transform.Find("Torso");
			torso.transform.parent = backRoot;
			torso.transform.localRotation = Quaternion.Euler(0, 0, 90);
			torso.transform.localScale = new Vector3(1.5f, 2.5f, 2f);
			torso.transform.localPosition = new Vector3(-0.5f, 0, 0);

			var rightHand = ghost.transform.Find("RightHand");
			rightHand.transform.parent = rightHandRoot;
			rightHand.transform.localRotation = Quaternion.Euler(0, 0, 90);
			rightHand.transform.localScale = 1.5f * Vector3.one;
			rightHand.transform.localPosition = Vector3.zero;

			var leftHand = ghost.transform.Find("LeftHand");
			leftHand.transform.parent = leftHandRoot;
			leftHand.transform.localRotation = Quaternion.Euler(0, 0, 90);
			leftHand.transform.localScale = 1.5f * Vector3.one;
			leftHand.transform.localPosition = Vector3.zero;

			eyes.gameObject.layer = OWLayer.DreamSimulation;
			_ghostHead.gameObject.layer = OWLayer.DreamSimulation;
			torso.gameObject.layer = OWLayer.DreamSimulation;
			rightHand.gameObject.layer = OWLayer.DreamSimulation;
			leftHand.gameObject.layer = OWLayer.DreamSimulation;

			GameObject.Destroy(ghost);
		}
		catch (Exception)
		{
			Util.WriteWarning("Couldn't load custom assets.");
		}
	}

	private void OnHazardsUpdated()
	{
		var _inFire = _hazardDetector.GetLatestHazardType() == HazardVolume.HazardType.FIRE;

		_activeFireRenderer = (InGreenFire && _dreamFireRenderer != null) ? _dreamFireRenderer : _fireRenderer;

		if (_inFire && !_activeFireRenderer.enabled)
		{
			inFire = true;
			_activeFireRenderer.material.SetFloat(_propID_Fade, fade);
			if (CommonCameraUtil.UsingCustomCamera())
			{
				_activeFireRenderer.enabled = true;
			}
		}
		else
		{
			inFire = false;
		}
	}

	private void OnSwitchActiveCamera(OWCamera camera)
	{
		Util.Write($"Switched to {camera} : {(CommonCameraUtil.UsingCustomCamera() ? "custom camera" : "stock camera")}");

		if (camera.name == "LandingCamera")
		{
			// Fake camera, ignore it
			return;
		}

		if (CommonCameraUtil.UsingCustomCamera())
		{
			SetHeadVisibility(true);
			SetArmVisibility(true);
			ShowUI(false);

			_roastingStickArm.enabled = false;
			_roastingStickNoSuit.enabled = false;

			if (_activeFireRenderer != null) _activeFireRenderer.enabled = inFire || fade > 0f;
		}
		else
		{
			SetHeadVisibility(false);
			SetArmVisibility(!_isToolHeld);
			ShowUI(true);

			_roastingStickArm.enabled = true;
			_roastingStickNoSuit.enabled = true;

			if (_activeFireRenderer != null) _activeFireRenderer.enabled = false;
		}
	}

	private void OnToolEquiped(PlayerTool _)
	{
		_isToolHeld = true;

		SetArmVisibility(CommonCameraUtil.UsingCustomCamera());
	}

	private void OnToolUnequiped(PlayerTool _)
	{
		_isToolHeld = false;

		SetArmVisibility(true);
	}

	private void SetArmVisibility(bool visible)
	{
		try
		{
			_animController._probeOnlyLayer = visible ? OWLayer.Default : OWLayer.VisibleToProbe;

			foreach (var arm in _animController._rightArmObjects)
			{
				arm.layer = visible ? OWLayer.Default : OWLayer.VisibleToProbe;
			}
		}
		catch (Exception e)
		{
			Util.WriteError($"{e}");
		}
	}

	private void OnRemoveHelmet() => SetHeadVisibility(CommonCameraUtil.UsingCustomCamera());

	private void OnPutOnHelmet() => SetHeadVisibility(CommonCameraUtil.UsingCustomCamera());

	private void SetHeadVisibility(bool visible)
	{
		IHatchlingOutfit hatchlingOutfit = CommonCameraUtil.Instance.HatchlingOutfit;
		try
		{
			bool wearingHelmet;
			if (hatchlingOutfit != null) wearingHelmet = hatchlingOutfit.GetPlayerHelmeted();
			// Check wearing suit not wearing helmet for future pikpik mod compat eyes of the past or wtv
			else wearingHelmet = Locator.GetPlayerSuit().IsWearingSuit();

			if (wearingHelmet)
			{
				_helmetMesh.layer = visible ? OWLayer.Default : OWLayer.VisibleToProbe;
			}
			else
			{
				_head.layer = visible ? OWLayer.Default : OWLayer.VisibleToProbe;
			}

			_ghostHead?.SetActive(visible);
		}
		catch (Exception)
		{
			Util.WriteWarning("Couldn't find players head.");
		}

		CommonCameraUtil.Instance.HeadVisibilityChanged.Invoke(visible);
	}

	private void ShowUI(bool visible)
	{
		GameObject reticule = GameObject.Find("Reticule");
		if (visible)
		{
			reticule.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
		}
		else
		{
			reticule.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceCamera;
			reticule.GetComponent<Canvas>().worldCamera = Locator.GetPlayerCamera().mainCamera;
		}

		if (_helmet2D != null)
		{
			_helmet2D.transform.Find("HelmetRoot/HelmetMesh/HUD_Helmet_v2/Helmet").transform.localScale = visible ? Vector3.one : Vector3.zero;
			_helmet2D.transform.Find("HelmetRoot/HelmetMesh/HUD_Helmet_v2/HelmetFrame").transform.localScale = visible ? Vector3.one : Vector3.zero;
			_helmet2D.transform.Find("HelmetRoot/HelmetMesh/HUD_Helmet_v2/Scarf").transform.localScale = visible ? Vector3.one : Vector3.zero;
		}
	}

	public void LateUpdate()
	{
		if (_activeFireRenderer != null && _activeFireRenderer.enabled)
		{
			fade = Mathf.MoveTowards(fade, inFire ? 1f : 0f, Time.deltaTime / (inFire ? 1f : 0.5f));
			_activeFireRenderer.material.SetAlpha(fade);
			float fireWidth = PlayerState.IsWearingSuit() ? 1.2f : 0.8f;
			if (InGreenFire && _dreamFireRenderer != null) fireWidth = 1.6f;
			_activeFireRenderer.transform.localScale = new Vector3(fireWidth, 2f, fireWidth) * (0.75f + 0.25f * Mathf.Sqrt(fade));

			if (!inFire && fade <= 0f)
			{
				fade = 0f;
				_activeFireRenderer.enabled = false;
			}
		}
	}
}
