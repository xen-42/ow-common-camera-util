using UnityEngine;

namespace CommonCameraUtil.Components;

public class RoastingStickAnim : MonoBehaviour
{
	public static bool IsRoasting;
	private Transform _roastingStick;
	private Animator _playerAnimator;

	private Transform _shoulder, _elbow, _hand;

	private Quaternion _elbowBaseRot, _shoulderBaseRot;
	private Vector3 _handOffset, _elbowOffset;

	private RoastingStickController _controller;

	private float _armLength;

	public Vector3 offset;

	private void Awake()
	{
		_playerAnimator = gameObject.transform.Find("Traveller_HEA_Player_v2").GetComponent<Animator>();

		_roastingStick = Util.Find("Player_Body/RoastingSystem/Stick_Root/Stick_Pivot/Stick_Tip").transform;

		_hand = _playerAnimator.GetBoneTransform(HumanBodyBones.RightHand);
		_elbow = _playerAnimator.GetBoneTransform(HumanBodyBones.RightLowerArm);
		_shoulder = _playerAnimator.GetBoneTransform(HumanBodyBones.RightUpperArm);

		_elbowBaseRot = _elbow.localRotation;
		_shoulderBaseRot = _shoulder.localRotation;

		_handOffset = _elbow.InverseTransformPoint(_hand.position);
		_elbowOffset = _shoulder.InverseTransformPoint(_elbow.position);

		_controller = gameObject.GetComponentInChildren<RoastingStickController>();

		GlobalMessenger<OWCamera>.AddListener("SwitchActiveCamera", OnSwitchActiveCamera);
	}

	private void Start()
	{
		_armLength = (_hand.position - _elbow.position).magnitude;
	}

	private void OnDestroy()
	{
		GlobalMessenger<OWCamera>.RemoveListener("SwitchActiveCamera", OnSwitchActiveCamera);
	}

	public void OnSwitchActiveCamera(OWCamera camera)
	{
		if (CommonCameraUtil.UsingCustomCamera())
		{
			_controller._stickMinZ = 1.5f;
			_controller._stickMaxZ = 2.1f;

			_controller.transform.localPosition = new Vector3(0.3f, 0.15f, 0.04f);
		}
		else
		{
			_controller._stickMinZ = 0.75f;
			_controller._stickMaxZ = 2f;

			_controller.transform.localPosition = new Vector3(0, 0.4f, 0);
		}
	}

	private void LateUpdate()
	{
		if (!IsRoasting || !CommonCameraUtil.UsingCustomCamera()) return;

		_elbow.transform.localRotation = Quaternion.FromToRotation(_handOffset.normalized, _elbow.InverseTransformPoint(_roastingStick.position).normalized) * _elbowBaseRot;

		// Closest distance from elbow to stick
		var closest = Util.ProjectPointLine(_elbow.position, _roastingStick.position, _roastingStick.position - _roastingStick.forward * 4f);

		var handle = closest + _roastingStick.forward * _armLength + _roastingStick.TransformVector(offset);

		var elbowPos = handle + (_elbow.position - handle).normalized * _armLength;

		_shoulder.transform.localRotation = Quaternion.FromToRotation(_elbowOffset.normalized, _shoulder.InverseTransformPoint(elbowPos).normalized) * _shoulderBaseRot;
		_elbow.transform.position = elbowPos;
		_hand.transform.position = handle;

		var head = _playerAnimator.GetBoneTransform(HumanBodyBones.Head);
		head.LookAt(_roastingStick, _playerAnimator.transform.up);
		head.localRotation *= Quaternion.Euler(0, 90, 270);
	}
}
