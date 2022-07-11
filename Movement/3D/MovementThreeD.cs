using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MovementThreeD : MonoBehaviour
{
    //TODO abstract movement into 2d/3d
    [SerializeField] private float _speed;
    [SerializeField] private float _jumpForce;
    [SerializeField] private string _movementActionName;
    [SerializeField] private string _attackActionName;

    private PlayerInput input;
    private bool isMovementLocked;

    // Start is called before the first frame update
    void Start()
    {
        input = GetComponent<PlayerInput>();
        ModuleManager.GetModule<PlayerEventmanager>().lockMove += (locked) => { isMovementLocked = locked; };
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(!isMovementLocked) move(input.actions[_movementActionName].ReadValue<Vector2>());
        ModuleManager.GetModule<PlayerEventmanager>().Move(GetComponent<Rigidbody>().velocity);
        if (input.actions[_attackActionName].triggered)
        {
            ModuleManager.GetModule<PlayerEventmanager>().Attack();
        }
    }

    public void move(Vector3 direction)
    {
        GetComponent<Rigidbody>().AddForce(Vector3.Scale(new Vector3(direction.y * _speed, 0, direction.y * _speed), new Vector3(Camera.main.transform.forward.x, 0, Camera.main.transform.forward.z).normalized));
        GetComponent<Rigidbody>().AddForce(Vector3.Scale(new Vector3(direction.x * _speed, 0, direction.x * _speed), new Vector3(Camera.main.transform.right.x, 0, Camera.main.transform.right.z).normalized));

        if(direction.x != 0 || direction.y != 0)
        {
            Quaternion rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(
                (Vector3.Scale(new Vector3(direction.y, 0, direction.y), Camera.main.transform.forward) +
                Vector3.Scale(new Vector3(direction.x, 0, direction.x), Camera.main.transform.right))/2f
                , Vector3.up), 0.5f);
            transform.rotation = rotation;
        }
    }
}
