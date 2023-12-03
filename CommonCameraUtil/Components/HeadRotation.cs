using UnityEngine;

namespace CommonCameraUtil.Components;

public class HeadRotation : MonoBehaviour
{
	private Animator _playerAnimator;
	private Transform _lookBase;

	public void Start()
	{
		_playerAnimator = Locator.GetPlayerBody().transform.Find("Traveller_HEA_Player_v2").GetComponent<Animator>();

		_lookBase = Locator.GetPlayerCamera().transform;
	}

	public void LateUpdate()
	{
		if (CommonCameraUtil.UsingCustomCamera())
		{
			// Modified from QSB PlayerHeadRotationSync, thanks _nebula
			var bone = _playerAnimator.GetBoneTransform(HumanBodyBones.Head);

			// Get the camera's local rotation with respect to the player body
			var lookLocalRotation = Quaternion.Inverse(_playerAnimator.transform.rotation) * _lookBase.rotation;
			var rot = lookLocalRotation.eulerAngles.x;

			// Instead of from -90 to 90 go from -30 to 30 so they dont break their neck
			if (rot > 180)
			{
				rot = ((rot - 360) / 3f) + 360f;
			}
			else
			{
				rot /= 3f;
			}

			bone.localRotation = Quaternion.Euler(0, 0, rot);
		}
	}
}
