using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonCameraController : MonoBehaviour
{
    [SerializeField] private Transform targetToFollow;
    [SerializeField] private Transform positionTransform;
    [SerializeField] private Transform anchorTransform;
    [SerializeField] private Camera cam;
    [SerializeField] private Collider camCollider;
    [SerializeField] private float camMoveSpeed;
    private float currentDistanceFromTarget;

    [SerializeField] private string mouseMoveActionName;
    [SerializeField] private string rotateTriggerActionName;
    [SerializeField] [Range(0,90)] private float maxAngle;
    private PlayerInput input;

    private float mouseSense;


    // Start is called before the first frame update
    void Start()
    {
        anchorTransform.transform.position = targetToFollow.transform.position;
        positionTransform.position = targetToFollow.transform.position;
        input = GetComponent<PlayerInput>();
        mouseSense = PlayerPrefs.GetFloat(ModuleManager.get<PlayerPrefKeys>().getPrefereceKey(PlayerPrefKeys.MOUSE_SENSITIVITY));
        ModuleManager.get<ControlManager>().valueChanged += (id, newValue) => { if (id.Equals(ModuleManager.get<PlayerPrefKeys>().getPrefereceKey(PlayerPrefKeys.MOUSE_SENSITIVITY))) mouseSense = newValue; };
    }

    // Update is called once per frame
    void Update()
    {
        followTarget();
        rotateAroundTarget();
        handleCollisions();
    }

    private void followTarget()
    {
        anchorTransform.transform.position = targetToFollow.transform.position;
    }

    private void rotateAroundTarget()
    {
        if (input.actions[rotateTriggerActionName].IsPressed())
        {
            Vector3 rotationInput = new Vector3(-1 * input.actions[mouseMoveActionName].ReadValue<Vector2>().y, input.actions[mouseMoveActionName].ReadValue<Vector2>().x);
            Vector3 rotation = Vector3.Lerp(anchorTransform.rotation.eulerAngles, 
                anchorTransform.rotation.eulerAngles + rotationInput,
                Time.deltaTime * mouseSense * camMoveSpeed);
            rotation.x = Mathf.Abs(rotation.x - 180) < Mathf.Abs(maxAngle - 180) ? anchorTransform.rotation.eulerAngles.x : rotation.x;


            anchorTransform.rotation = Quaternion.Euler(rotation);
        }
    }

    private void handleCollisions()
    {
        float disctancePosition = Vector3.Distance(positionTransform.position, anchorTransform.position);
        float disctanceCamera = Vector3.Distance(cam.transform.position, anchorTransform.position);
        if (Physics.OverlapBox(cam.transform.position, camCollider.bounds.size / 2, Quaternion.identity, LayerMask.GetMask("Ground")).Any() && disctanceCamera > 1)
        {
            currentDistanceFromTarget = Vector3.Distance(positionTransform.position + Vector3.Scale(camCollider.bounds.size / 2, cam.transform.forward), targetToFollow.transform.position);
            positionTransform.position = Vector3.Lerp(positionTransform.position + currentDistanceFromTarget * cam.transform.forward, positionTransform.position, 0.9f);
        }
        else if(!Physics.OverlapBox(cam.transform.position, camCollider.bounds.size / 1.5f, Quaternion.identity, LayerMask.GetMask("Ground")).Any() && disctancePosition > 1)
        {
            currentDistanceFromTarget = Vector3.Distance(positionTransform.position + Vector3.Scale(camCollider.bounds.size / 2, cam.transform.forward), targetToFollow.transform.position);
            positionTransform.position = Vector3.Lerp(positionTransform.position - currentDistanceFromTarget * cam.transform.forward, positionTransform.position, 0.9f);
        }
    }
}
