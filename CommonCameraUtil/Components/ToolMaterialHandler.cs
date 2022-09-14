using System.Linq;
using UnityEngine;

namespace CommonCameraUtil.Handlers;

public class ToolMaterialHandler : MonoBehaviour
{
    private PlayerTool _heldTool;

    // Some tools must be set one tick after equip
    private readonly string[] laterTickTools = { "NomaiTranslatorProp", "ProbeLauncher", "ItemCarryTool", "TutorialProbeLauncher_Base" };
    private bool _thirdPersonMaterialNextTick = false;

    // We make some tools larger in 3rd person view
    private readonly string[] resizingExemptTools = { "NomaiTranslatorProp", "ProbeLauncher", "TutorialCamera_Base", "TutorialProbeLauncher_Base" };
    private bool _hasFixedProbeLauncher = false;

    private bool _checkToolEquipNextTick = false;

    public void Awake()
    {
        CommonCameraUtil.Instance.EquipTool.AddListener(OnToolEquiped);
        CommonCameraUtil.Instance.UnequipTool.AddListener(OnToolUnequiped);

        GlobalMessenger.AddListener("OnRetrieveProbe", OnRetrieveProbe);
        GlobalMessenger.AddListener("EnterDreamWorld", OnEnterDreamWorld);
        GlobalMessenger<OWCamera>.AddListener("SwitchActiveCamera", OnSwitchActiveCamera);
	}

    public void OnDestroy()
    {
        CommonCameraUtil.Instance.EquipTool.RemoveListener(OnToolEquiped);
        CommonCameraUtil.Instance.UnequipTool.RemoveListener(OnToolUnequiped);

        GlobalMessenger.RemoveListener("OnRetrieveProbe", OnRetrieveProbe);
        GlobalMessenger.RemoveListener("EnterDreamWorld", OnEnterDreamWorld);
        GlobalMessenger<OWCamera>.RemoveListener("SwitchActiveCamera", OnSwitchActiveCamera);
    }

    public void OnRetrieveProbe()
    {
        SetToolMaterials(CommonCameraUtil.UsingCustomCamera());
    }

    public void OnEnterDreamWorld()
    {
        // When entering dream world I think it gives a new artifact
        _checkToolEquipNextTick = true;
    }

    public void OnToolEquiped(PlayerTool tool)
    {
        _heldTool = tool;

        Util.Write("Picked up " + _heldTool.name + " " + _heldTool.gameObject.name + " " + _heldTool.GetType());

        if (!CommonCameraUtil.UsingCustomCamera()) return;

        // Some tool materials must be set next tick
        if (laterTickTools.Contains(_heldTool.name)) _thirdPersonMaterialNextTick = true;
        else SetToolMaterials(true);

        // We make some of the models look larger for the 3rd person view
        if (!resizingExemptTools.Contains(tool.name)) tool.transform.localScale = new Vector3(2, 2, 2);
    }

    public void OnToolUnequiped(PlayerTool tool)
    {
        // Put them back to normal when unequiping
        //SetToolMaterials(false);
        _heldTool = null;
    }

    public void OnSwitchActiveCamera(OWCamera camera)
    {
        if (CommonCameraUtil.UsingCustomCamera())
        {
            SetToolMaterials(true);
        }
        else
        {
            SetToolMaterials(false);
            // Double check we're still holding it
            if (_heldTool != null && !_heldTool.IsEquipped()) _heldTool = null;
        }
    }

    private void SetToolMaterials(bool thirdPerson)
    {
        if (!_hasFixedProbeLauncher)
        {
            GameObject probeLauncher = Locator.GetPlayerBody().GetComponentInChildren<ProbeLauncher>().gameObject;
            if (probeLauncher != null)
            {
                probeLauncher.layer = 0;
                _hasFixedProbeLauncher = true;
            }
        }

        if (_heldTool == null) return;
        MeshRenderer[] meshRenderers = _heldTool.GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer meshRenderer in meshRenderers)
        {
            // Have to keep their relative order
            foreach (Material m in meshRenderer.materials)
            {
                if (m.name.Contains("NOM") || m.name.Contains("Terrain_QM_CenterArch_mat")) continue; // Don't want to mess up scroll /solanum materials
                if (thirdPerson && m.renderQueue >= 2000) m.renderQueue -= 2000;
                else if (!thirdPerson && m.renderQueue < 2000) m.renderQueue += 2000;
            }
        }
    }

    public void Update()
    {
        if (_heldTool is ItemTool && Locator.GetToolModeSwapper().GetItemCarryTool() == null)
        {
            // We dropped it?
            SetToolMaterials(false);
            _heldTool = null;
        }

        if (_thirdPersonMaterialNextTick)
        {
            SetToolMaterials(true);
            _thirdPersonMaterialNextTick = false;
        }

        if (_checkToolEquipNextTick)
        {
            OnToolEquiped(Locator.GetToolModeSwapper().GetItemCarryTool());
            _checkToolEquipNextTick = false;
        }
    }
}
