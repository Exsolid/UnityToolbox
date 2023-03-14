using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class FirstPersonCameraController : MonoBehaviour
{
    [SerializeField] private PlayerInput _input;
    [SerializeField] private float _mouseSensitivity;
    private float _mouseSensitivityUpdated;
    [SerializeField] private string _viewActionName;
    [SerializeField] private Camera _camera;
    [SerializeField] private GameObject _playerToRotateInstead;

    private Vector3 _rotateToPosition;

    [SerializeField] private bool _lockRotation;

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
        Vector2 mouse = _input.actions[_viewActionName].ReadValue<Vector2>();
        if(_playerToRotateInstead == null)
        {
            Quaternion rotation = Quaternion.Lerp(_camera.transform.rotation, Quaternion.Euler(_camera.transform.rotation.eulerAngles.x + mouse.y * -1 / 2, _camera.transform.rotation.eulerAngles.y + mouse.x / 2, 0), _mouseSensitivityUpdated * Time.deltaTime * 10);
            rotation = Quaternion.Euler((Mathf.Abs(rotation.eulerAngles.x - 180) < 115) ? _camera.transform.rotation.eulerAngles.x : rotation.eulerAngles.x, rotation.eulerAngles.y, 0);
            _camera.transform.rotation = rotation;
        }
        else
        {
            Quaternion rotation = Quaternion.Lerp(_playerToRotateInstead.transform.rotation, Quaternion.Euler(_playerToRotateInstead.transform.rotation.eulerAngles.x + mouse.y * -1 / 2, _playerToRotateInstead.transform.rotation.eulerAngles.y + mouse.x / 2, 0), _mouseSensitivityUpdated * Time.deltaTime * 10);
            rotation = Quaternion.Euler((Mathf.Abs(rotation.eulerAngles.x - 180) < 115) ? _playerToRotateInstead.transform.rotation.eulerAngles.x : rotation.eulerAngles.x, rotation.eulerAngles.y, 0);

            Quaternion rotationCam = Quaternion.Lerp(_camera.transform.rotation, Quaternion.Euler(_camera.transform.rotation.eulerAngles.x + mouse.y * -1 / 2, _camera.transform.rotation.eulerAngles.y + mouse.x / 2, 0), _mouseSensitivityUpdated * Time.deltaTime * 10);
            rotationCam = Quaternion.Euler((Mathf.Abs(rotationCam.eulerAngles.x - 180) < 115) ? _camera.transform.rotation.eulerAngles.x : rotationCam.eulerAngles.x, rotationCam.eulerAngles.y, 0);

            _camera.transform.rotation = Quaternion.Euler(rotationCam.eulerAngles.x, _camera.transform.rotation.eulerAngles.y, rotationCam.eulerAngles.z);
            _playerToRotateInstead.transform.rotation = Quaternion.Euler(_playerToRotateInstead.transform.rotation.eulerAngles.x, rotation.eulerAngles.y, _playerToRotateInstead.transform.rotation.eulerAngles.z);
        }
    }

    public void RotateTo(Vector3 position)
    {
        _rotateToPosition = position;
    }

    private void UpdateMovementLock(bool locked)
    {
        _lockRotation = locked;
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
            ModuleManager.GetModule<SettingsManager>().OnSenseValueChanged += UpdateMouseSense;
        }
    }
}
