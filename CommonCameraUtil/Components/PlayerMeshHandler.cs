using System;
using UnityEngine;

namespace CommonCameraUtil.Components;

public class PlayerMeshHandler : MonoBehaviour
{
    private bool _isToolHeld = false;
    private bool _setArmVisibleNextTick = false;

    private MeshRenderer _fireRenderer, _dreamFireRenderer, _activeFireRenderer;
    private HazardDetector _hazardDetector;
    private float fade = 0f;
    private bool inFire = false;

    private int _propID_Fade;

    // Body parts
    private GameObject _suitArm, _fleshArm, _helmetMesh, _helmet2D, _head;

    // Hand holding marshmallow stick
    private MeshRenderer _roastingStickArm, _roastingStickNoSuit;

    private static AssetBundle assetBundle;
    private GameObject ghostPrefab;

    public static bool InGreenFire;

    public void Awake()
    {
        CommonCameraUtil.Instance.EquipTool.AddListener(OnToolEquiped);
        CommonCameraUtil.Instance.UnequipTool.AddListener(OnToolUnequiped);

        GlobalMessenger.AddListener("RemoveHelmet", OnRemoveHelmet);
        GlobalMessenger.AddListener("PutOnHelmet", OnPutOnHelmet);
        GlobalMessenger<OWCamera>.AddListener("SwitchActiveCamera", OnSwitchActiveCamera);
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

            var head = ghost.transform.Find("Head");
            var eyes = ghost.transform.Find("Eyes");
            eyes.transform.parent = head;
            head.transform.parent = headRoot;
            head.transform.localRotation = Quaternion.Euler(0, 0, 90);
            head.transform.localScale = 1.25f * Vector3.one;
            head.transform.localPosition = new Vector3(2, -1, 0);

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

            eyes.gameObject.layer = 28;
            head.gameObject.layer = 28;
            torso.gameObject.layer = 28;
            rightHand.gameObject.layer = 28;
            leftHand.gameObject.layer = 28;

            GameObject.Destroy(ghost);
        }
        catch (Exception)
        {
            Util.WriteWarning("Couldn't load custom assets.");
        }

        SetHeadVisible();
    }

    private void OnHazardsUpdated()
    {
        var _inFire = _hazardDetector.GetLatestHazardType() == HazardVolume.HazardType.FIRE;

        _activeFireRenderer = (InGreenFire && _dreamFireRenderer != null) ? _dreamFireRenderer : _fireRenderer;

        if (_inFire && !_activeFireRenderer.enabled)
        {
            inFire = true;
            _activeFireRenderer.material.SetFloat(_propID_Fade, fade);
            if (CommonCameraUtil.UsingCustomCamera()) _activeFireRenderer.enabled = true;
        }
        else
        {
            inFire = false;
        }
    }

    private void OnSwitchActiveCamera(OWCamera camera)
    {
        if (CommonCameraUtil.IsCustomCamera(camera))
        {
            SetHelmet(false);
            SetArmVisibility(true);
            ShowReticule(false);

            SetHeadVisible();

            _roastingStickArm.enabled = false;
            _roastingStickNoSuit.enabled = false;

            if (_activeFireRenderer != null) _activeFireRenderer.enabled = inFire || fade > 0f;
        }
        else
        {
            SetHelmet(true);
            SetArmVisibility(!_isToolHeld);
            ShowReticule(true);

            _roastingStickArm.enabled = true;
            _roastingStickNoSuit.enabled = true;

            if (_activeFireRenderer != null) _activeFireRenderer.enabled = false;
        }
    }

    private void OnToolEquiped(PlayerTool _)
    {
        _isToolHeld = true;
        if (CommonCameraUtil.UsingCustomCamera())
        {
            _setArmVisibleNextTick = true;
        }
    }

    private void OnToolUnequiped(PlayerTool _)
    {
        _isToolHeld = false;
        SetArmVisibility(true);
    }

    private void SetHelmet(bool visible)
    {
        if (_helmet2D != null)
        {
            _helmet2D.transform.Find("HelmetRoot/HelmetMesh/HUD_Helmet_v2/Helmet").transform.localScale = visible ? Vector3.one : Vector3.zero;
            _helmet2D.transform.Find("HelmetRoot/HelmetMesh/HUD_Helmet_v2/HelmetFrame").transform.localScale = visible ? Vector3.one : Vector3.zero;
            _helmet2D.transform.Find("HelmetRoot/HelmetMesh/HUD_Helmet_v2/Scarf").transform.localScale = visible ? Vector3.one : Vector3.zero;
        }
    }

    private void SetArmVisibility(bool visible)
    {
        try
        {
            if (_suitArm == null) _suitArm = Locator.GetPlayerTransform().Find("Traveller_HEA_Player_v2/Traveller_Mesh_v01:Traveller_Geo/Traveller_Mesh_v01:PlayerSuit_RightArm").gameObject;
            if (_fleshArm == null) _fleshArm = Locator.GetPlayerTransform().Find("Traveller_HEA_Player_v2/player_mesh_noSuit:Traveller_HEA_Player/player_mesh_noSuit:Player_RightArm").gameObject;
        }
        catch (Exception)
        {
            Util.WriteWarning("Couldn't find arm");
        }

        if (_suitArm != null) _suitArm.layer = visible ? OWLayer.Default : OWLayer.VisibleToProbe;
        if (_fleshArm != null) _fleshArm.layer = visible ? OWLayer.Default : OWLayer.VisibleToProbe;
		}

    private void OnRemoveHelmet()
    {
        SetHeadVisible();
    }

    private void OnPutOnHelmet()
    {
        SetHeadVisible();
    }

    private void SetHeadVisible()
    {
        if (Locator.GetPlayerSuit().IsWearingHelmet())
        {
            if (_helmetMesh == null)
            {
                _helmetMesh = Locator.GetPlayerTransform().Find("Traveller_HEA_Player_v2/Traveller_Mesh_v01:Traveller_Geo/Traveller_Mesh_v01:PlayerSuit_Helmet").gameObject;
                _helmetMesh.layer = OWLayer.Default;
            }
        }
        else
        {
            if (_head == null)
            {
                try
                {
                    _head = Locator.GetPlayerTransform().Find("Traveller_HEA_Player_v2/player_mesh_noSuit:Traveller_HEA_Player/player_mesh_noSuit:Player_Head").gameObject;
                    _head.layer = OWLayer.Default;
					}
                catch (Exception)
                {
                    Util.WriteWarning("Couldn't find players head.");
                }
            }
        }
    }

    private void ShowReticule(bool visible)
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
    }

    public void Update()
    {
        if (_setArmVisibleNextTick)
        {
            SetArmVisibility(true);
            _setArmVisibleNextTick = false;
        }

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
