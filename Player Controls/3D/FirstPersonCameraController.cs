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
    [SerializeField] [Range(-1, 180)] private int _maxVerticalAngle;
    [SerializeField] [Range(-1, 180)] private int _maxHorizontalAngle;
    [SerializeField] private Camera _camera;
    private Quaternion _initialRotation;

    private Vector3 _rotateToPosition;

    [SerializeField] private bool _lockRotation;

    private Quaternion _goalRotationCamera;
    private Quaternion _goalRotationPlayer;

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
        _initialRotation = _camera.transform.rotation;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        Vector2 mouse = _input.actions[_viewActionName].ReadValue<Vector2>();
        _goalRotationCamera = Quaternion.Euler(_camera.transform.rotation.eulerAngles.x + mouse.y * -1 / 2, _camera.transform.rotation.eulerAngles.y + mouse.x / 2, 0);
        if(_playerToRotateInstead != null)
        {
            _goalRotationPlayer = Quaternion.Euler(_playerToRotateInstead.transform.rotation.eulerAngles.x + mouse.y * -1 / 2, _playerToRotateInstead.transform.rotation.eulerAngles.y + mouse.x / 2, 0);
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
            Quaternion lerpedRotationCamera = Quaternion.Slerp(_camera.transform.rotation, _goalRotationCamera, _mouseSensitivityUpdated * Time.fixedDeltaTime * 10);
            Quaternion clampedRotationCamera = Quaternion.Euler(
                _maxVerticalAngle != -1 && (Mathf.Abs(lerpedRotationCamera.eulerAngles.x - 180) < _maxVerticalAngle) 
                    ? _camera.transform.rotation.eulerAngles.x 
                    : lerpedRotationCamera.eulerAngles.x,
                _maxHorizontalAngle != -1 && !MayRotateHorizontal(lerpedRotationCamera.eulerAngles) 
                    ? _camera.transform.rotation.eulerAngles.y 
                    : lerpedRotationCamera.eulerAngles.y, 
                0);

            _camera.transform.rotation = clampedRotationCamera;
        }
        else
        {
            Quaternion lerpedRotationPlayer = Quaternion.Slerp(_playerToRotateInstead.transform.rotation, _goalRotationPlayer, _mouseSensitivityUpdated * Time.fixedDeltaTime * 10);
            Quaternion clampedRotationPlayer = Quaternion.Euler(
                _maxVerticalAngle != -1 && (Mathf.Abs(lerpedRotationPlayer.eulerAngles.x - 180) < _maxVerticalAngle) 
                    ? _playerToRotateInstead.transform.rotation.eulerAngles.x 
                    : lerpedRotationPlayer.eulerAngles.x,
                lerpedRotationPlayer.eulerAngles.y, 
                0);

            Quaternion lerpedRotationCamera = Quaternion.Slerp(_camera.transform.rotation, _goalRotationCamera, _mouseSensitivityUpdated * Time.fixedDeltaTime * 10);
            Quaternion clampedRotationCamera = Quaternion.Euler(
                _maxVerticalAngle != -1 && (Mathf.Abs(lerpedRotationCamera.eulerAngles.x - 180) < _maxVerticalAngle) 
                    ? _camera.transform.rotation.eulerAngles.x 
                    : lerpedRotationCamera.eulerAngles.x,
                lerpedRotationCamera.eulerAngles.y, 
                0);

            _camera.transform.rotation = Quaternion.Euler(clampedRotationCamera.eulerAngles.x, _camera.transform.rotation.eulerAngles.y, clampedRotationCamera.eulerAngles.z);
            _playerToRotateInstead.transform.rotation = Quaternion.Euler(_playerToRotateInstead.transform.rotation.eulerAngles.x, clampedRotationPlayer.eulerAngles.y, _playerToRotateInstead.transform.rotation.eulerAngles.z);
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
        Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.Confined;
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

    private bool MayRotateHorizontal(Vector3 currentRotation)
    {
        float fromRight = 0;
        float fromLeft = 0;
        if (currentRotation.y > _initialRotation.eulerAngles.y)
        {
            fromRight = currentRotation.y - _initialRotation.eulerAngles.y;
            fromLeft = _initialRotation.eulerAngles.y + 360 - currentRotation.y;
        }
        else
        {
            fromRight = _initialRotation.eulerAngles.y - currentRotation.y;
            fromLeft = currentRotation.y + 360 - _initialRotation.eulerAngles.y;
        }

        return fromLeft < _maxHorizontalAngle || fromRight < _maxHorizontalAngle;
    }
}
