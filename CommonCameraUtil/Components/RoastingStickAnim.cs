using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CommonCameraUtil.Components
{
    public class RoastingStickAnim : MonoBehaviour
    {
        public static bool IsRoasting;
        private Transform _roastingStick;
        private Animator _playerAnimator;

        private Transform _shoulder, _elbow, _hand;

        private Quaternion _elbowBaseRot;
        private Vector3 _handOffset;

        private RoastingStickController _controller;

        private void Awake()
        {
            _playerAnimator = Locator.GetPlayerBody().transform.Find("Traveller_HEA_Player_v2").GetComponent<Animator>();

            _roastingStick = Util.Find("Player_Body/RoastingSystem/Stick_Root/Stick_Pivot/Stick_Tip").transform;

            _hand = _playerAnimator.GetBoneTransform(HumanBodyBones.RightHand);
            _elbow = _playerAnimator.GetBoneTransform(HumanBodyBones.RightLowerArm);
            _shoulder = _playerAnimator.GetBoneTransform(HumanBodyBones.RightUpperArm);

            _elbowBaseRot = _elbow.localRotation;

            _handOffset = _elbow.InverseTransformPoint(_hand.position);

            _controller = Locator.GetPlayerBody().GetComponentInChildren<RoastingStickController>();

            GlobalMessenger<OWCamera>.AddListener("SwitchActiveCamera", OnSwitchActiveCamera);
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
                _controller._stickMaxZ = 2.5f;

                _controller.transform.position = _elbow.position;
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

            var head = _playerAnimator.GetBoneTransform(HumanBodyBones.Head);
            head.LookAt(_roastingStick, _playerAnimator.transform.up);
            head.localRotation *= Quaternion.Euler(0, 90, 270);
        }
    }
}
