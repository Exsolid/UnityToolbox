using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityToolbox.General.Management;
using UnityToolbox.General.Preferences;
using UnityToolbox.UI.Settings;

namespace UnityToolbox.PlayerControls.ThreeD
{
    /// <summary>
    /// A camera controller which lets the camera follow the player, move it and react on collision.
    /// To understand how it is set up, the the summeries of the position and anchor transform as well as the camera.
    /// Requires the <see cref="PlayerPrefKeys"/> and <see cref="SettingsManager"/>.
    /// </summary>
    public class ThirdPersonCameraController : MonoBehaviour
    {
        /// <summary>
        /// The target the camera should follow.
        /// </summary>
        [SerializeField] private Transform _targetToFollow;
        /// <summary>
        /// Should be set up as the parent transform of the <see cref="_positionTransform"/> with no parent of itself.
        /// This handels the following of the target.
        /// </summary>
        [SerializeField] private Transform _anchorTransform;
        /// <summary>
        /// Should have the <see cref="_anchorTransform"/> as its parent and its position and rotation set to all zero.
        /// The <see cref="_camera"/> should be its child.
        /// This handels the updated position in case of a collision.
        /// </summary>
        [SerializeField] private Transform _positionTransform;
        /// <summary>
        /// This needs to be a child of the <see cref="_positionTransform"/> and can be placed anywhere. The perspective set up will be the defined maximum distance to the <see cref="_targetToFollow"/>.
        /// </summary>
        [SerializeField] private Camera _camera;
        [SerializeField] private Collider _camCollider;
        [SerializeField] private float _camMoveSpeed;
        [SerializeField] private List<string> _collidingLayers;
        private int _layerMask;
        private float _currentDistanceFromTarget;

        [SerializeField] private string _mouseMoveActionName;
        [SerializeField] private string _rotateTriggerActionName;
        [SerializeField] [Range(0,90)] private float _maxAngle;
        private PlayerInput _input;

        private float _mouseSense;


        // Start is called before the first frame update
        void Start()
        {
            _anchorTransform.transform.position = _targetToFollow.transform.position;
            _positionTransform.position = _targetToFollow.transform.position;
            _input = GetComponent<PlayerInput>();
            _mouseSense = PlayerPrefs.GetFloat(ModuleManager.GetModule<PlayerPrefKeys>().GetPrefereceKey(PlayerPrefKeys.MOUSE_SENSITIVITY));
            ModuleManager.GetModule<SettingsManager>().OnSenseValueChanged += (newValue) => { _mouseSense = newValue; };
            foreach(string layer in _collidingLayers)
            {
                _layerMask = _layerMask | LayerMask.GetMask(layer);
            }
        }

        // Update is called once per frame
        void Update()
        {
            FollowTarget();
            RotateAroundTarget();
            HandleCollisions();
        }

        private void FollowTarget()
        {
            _anchorTransform.transform.position = _targetToFollow.transform.position;
        }

        private void RotateAroundTarget()
        {
            if (_input.actions[_rotateTriggerActionName].IsPressed())
            {
                Vector3 rotationInput = new Vector3(-1 * _input.actions[_mouseMoveActionName].ReadValue<Vector2>().y, _input.actions[_mouseMoveActionName].ReadValue<Vector2>().x);
                Vector3 rotation = Vector3.Lerp(_anchorTransform.rotation.eulerAngles, 
                    _anchorTransform.rotation.eulerAngles + rotationInput,
                    Time.deltaTime * _mouseSense * _camMoveSpeed);
                rotation.x = Mathf.Abs(rotation.x - 180) < Mathf.Abs(_maxAngle - 180) ? _anchorTransform.rotation.eulerAngles.x : rotation.x;


                _anchorTransform.rotation = Quaternion.Euler(rotation);
            }
        }

        private void HandleCollisions()
        {
            float disctancePosition = Vector3.Distance(_positionTransform.position, _anchorTransform.position);
            float disctanceCamera = Vector3.Distance(_camera.transform.position, _anchorTransform.position);
            if (Physics.OverlapBox(_camera.transform.position, _camCollider.bounds.size / 2, Quaternion.identity, _layerMask).Any() && disctanceCamera > 1)
            {
                _currentDistanceFromTarget = Vector3.Distance(_positionTransform.position + Vector3.Scale(_camCollider.bounds.size / 2, _camera.transform.forward), _targetToFollow.transform.position);
                _positionTransform.position = Vector3.Lerp(_positionTransform.position + _currentDistanceFromTarget * _camera.transform.forward, _positionTransform.position, 0.9f);
            }
            else if(!Physics.OverlapBox(_camera.transform.position, _camCollider.bounds.size / 1.5f, Quaternion.identity, _layerMask).Any() && disctancePosition > 0.1f)
            {
                _currentDistanceFromTarget = Vector3.Distance(_positionTransform.position + Vector3.Scale(_camCollider.bounds.size / 2, _camera.transform.forward), _targetToFollow.transform.position);
                _positionTransform.position = Vector3.Lerp(_positionTransform.position - _currentDistanceFromTarget * _camera.transform.forward, _positionTransform.position, 0.9f);
            }
        }
    }
}
