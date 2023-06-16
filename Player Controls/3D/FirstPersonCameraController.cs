using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

/// <summary>
/// A first person camera controller which moves with mouse movement. Can be set to only rotate to a certain degree.
/// For it to follow a target, place it as a child object.
/// Requires the <see cref="PlayerPrefKeys">, <see cref="PlayerEventManager"/> and <see cref="SettingsManager"/>.
/// </summary>
public class FirstPersonCameraController : MonoBehaviour
{
    [SerializeField] private PlayerInput _input;
    [SerializeField] private float _mouseSensitivity;
    private float _mouseSensitivityUpdated;
    /// <summary>
    /// The mouse input within the InputControls.
    /// </summary>
    [SerializeField] private string _viewActionName;
    [SerializeField] private GameObject _playerToRotateInstead;
    [SerializeField] [Range(0, 89)] private int _maxVerticalAngle;
    [SerializeField] [Range(-1, 180)] private int _maxHorizontalAngle = -1;
    [SerializeField] private float _slerpModifier = 6;
    [SerializeField] private Camera _camera;

    private Vector3 _rotateToPosition;

    [SerializeField] private bool _lockRotation;

    private Quaternion _initialRotation;

    private Vector2 _rotation = Vector2.zero;
    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.HasKey(ModuleManager.GetModule<PlayerPrefKeys>().GetPrefereceKey(PlayerPrefKeys.MOUSE_SENSITIVITY)))
        {
            _mouseSensitivityUpdated = _mouseSensitivity * PlayerPrefs.GetFloat(ModuleManager.GetModule<PlayerPrefKeys>().GetPrefereceKey(PlayerPrefKeys.MOUSE_SENSITIVITY));
        }
        else
        {
            _mouseSensitivityUpdated = _mouseSensitivity;
        }

        ModuleManager.GetModule<PlayerEventManager>().OnLockMove += UpdateMovementLock;
        ModuleManager.GetModule<SettingsManager>().OnSenseValueChanged += UpdateMouseSense;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        _initialRotation = _camera.transform.rotation;
    }

    private void Update()
    {
        if (!_lockRotation)
        {
            Vector2 mouse = _input.actions[_viewActionName].ReadValue<Vector2>();
            _rotation.x += mouse.x * _mouseSensitivityUpdated * Time.deltaTime * 2;
            _rotation.y += mouse.y * _mouseSensitivityUpdated * Time.deltaTime * 2;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!_lockRotation)
        {
            TurnView();
        }
        else if (!_rotateToPosition.Equals(Vector3.zero))
        {
            _camera.transform.LookAt(_rotateToPosition);
            _playerToRotateInstead.transform.rotation = Quaternion.Euler(_playerToRotateInstead.transform.rotation.eulerAngles.x, _camera.transform.rotation.eulerAngles.y, _playerToRotateInstead.transform.rotation.eulerAngles.z);
            _rotateToPosition = Vector3.zero;
        }
    }

    private void TurnView()
    {
        if (_playerToRotateInstead == null)
        {
            _rotation.y = Mathf.Clamp(_rotation.y, -_maxVerticalAngle - _initialRotation.eulerAngles.x, _maxVerticalAngle - _initialRotation.eulerAngles.x);

            if (_maxHorizontalAngle != -1)
            {
                _rotation.x = Mathf.Clamp(_rotation.x, -_maxHorizontalAngle + _initialRotation.eulerAngles.y, _maxHorizontalAngle + _initialRotation.eulerAngles.y);
            }

            var xQuat = Quaternion.AngleAxis(_rotation.x, Vector3.up);
            var yQuat = Quaternion.AngleAxis(_rotation.y, Vector3.left);
            
            _camera.transform.localRotation = Quaternion.Slerp(_camera.transform.localRotation, xQuat * yQuat, Time.deltaTime * _slerpModifier);
        }
        else
        {
            _rotation.y = Mathf.Clamp(_rotation.y, -_maxVerticalAngle - _initialRotation.eulerAngles.x, _maxVerticalAngle - _initialRotation.eulerAngles.x);

            if (_maxHorizontalAngle != -1)
            {
                _rotation.x = Mathf.Clamp(_rotation.x, -_maxHorizontalAngle + _initialRotation.eulerAngles.y, _maxHorizontalAngle + _initialRotation.eulerAngles.y);
            }

            Quaternion xQuat = Quaternion.AngleAxis(_rotation.x, Vector3.up);
            Quaternion yQuat = Quaternion.AngleAxis(_rotation.y, Vector3.left);

            _camera.transform.localRotation = Quaternion.Slerp(_camera.transform.localRotation, yQuat, Time.deltaTime * _slerpModifier);
            _playerToRotateInstead.transform.localRotation = Quaternion.Slerp(_playerToRotateInstead.transform.localRotation, xQuat, Time.deltaTime * _slerpModifier);
        }
    }

    /// <summary>
    /// Rotates the camera to look at a given position.
    /// </summary>
    /// <param name="position">The position to look at.</param>
    public void RotateTo(Vector3 position)
    {
        _rotateToPosition = position;
    }

    private void UpdateMovementLock(bool locked)
    {
        _lockRotation = locked;
        Cursor.lockState = locked ? CursorLockMode.Confined : CursorLockMode.Locked;
        Cursor.visible = !locked;
    }

    private void UpdateMouseSense(float newValue)
    {
        _mouseSensitivityUpdated = _mouseSensitivity * newValue;
    }

    private void OnDestroy()
    {
        if (ModuleManager.ModuleRegistered<PlayerEventManager>())
        {
            ModuleManager.GetModule<PlayerEventManager>().OnLockMove -= UpdateMovementLock;
        }
        if (ModuleManager.ModuleRegistered<SettingsManager>())
        {
            ModuleManager.GetModule<SettingsManager>().OnSenseValueChanged -= UpdateMouseSense;
        }
    }
}
