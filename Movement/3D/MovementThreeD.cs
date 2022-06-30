using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MovementThreeD : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float jumpForce;
    [SerializeField] private string movementActionName;
    [SerializeField] private string attackActionName;

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
        if(!isMovementLocked) move(input.actions[movementActionName].ReadValue<Vector2>());
        ModuleManager.GetModule<PlayerEventmanager>().Move(GetComponent<Rigidbody>().velocity);
        if (input.actions[attackActionName].triggered)
        {
            ModuleManager.GetModule<PlayerEventmanager>().Attack();
        }
    }

    public void move(Vector3 direction)
    {
        GetComponent<Rigidbody>().AddForce(Vector3.Scale(new Vector3(direction.y * speed, 0, direction.y * speed), new Vector3(Camera.main.transform.forward.x, 0, Camera.main.transform.forward.z).normalized));
        GetComponent<Rigidbody>().AddForce(Vector3.Scale(new Vector3(direction.x * speed, 0, direction.x * speed), new Vector3(Camera.main.transform.right.x, 0, Camera.main.transform.right.z).normalized));

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
