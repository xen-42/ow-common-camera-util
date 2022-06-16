using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CommonCameraUtil.Components
{
    public class HeadRotation : MonoBehaviour
    {
        private Animator _playerAnimator;
        private Transform _lookBase;

        public void Start()
        {
            _playerAnimator = Locator.GetPlayerBody().GetComponent<Animator>();

            var head = _playerAnimator.GetBoneTransform(HumanBodyBones.Head);
            _lookBase = new GameObject("LookBase").transform;
            _lookBase.transform.parent = head.parent;
            _lookBase.transform.rotation = head.rotation;
            _lookBase.transform.position = head.position;
        }

        public void Update()
        {
            if (CommonCameraUtil.UsingCustomCamera())
            {
                // From QSB, thanks _nebula

                var bone = _playerAnimator.GetBoneTransform(HumanBodyBones.Head);
                // Get the camera's local rotation with respect to the player body
                var lookLocalRotation = Quaternion.Inverse(_playerAnimator.transform.rotation) * _lookBase.rotation;
                bone.localRotation = Quaternion.Euler(-lookLocalRotation.eulerAngles.y, -lookLocalRotation.eulerAngles.z, lookLocalRotation.eulerAngles.x);
            }
        }
    }
}
