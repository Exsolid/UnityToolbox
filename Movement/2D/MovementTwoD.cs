using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MovementTwoD : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float jumpForce;
    [SerializeField] private Transform bottomTransform;
    [SerializeField] private string actionName;
    private RaycastHit2D grounded;
    private PlayerInput input;
    // Start is called before the first frame update
    void Start()
    {
        input = GetComponent<PlayerInput>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        int everyMaskExcept =~ LayerMask.GetMask("Player");
        grounded = Physics2D.Raycast(bottomTransform.position, transform.up * -1, 0.08f, everyMaskExcept);
        move(input.actions[actionName].ReadValue<Vector2>());
    }

    public void move(Vector3 direction)
    {
        ModuleManager.get<PlayerEventmanager>().Move(GetComponent<Rigidbody2D>().velocity);
        GetComponent<Rigidbody2D>().AddForce(new Vector3(direction.x * speed, 0, direction.z * speed));
    }

    public void moveWithStrength(Vector3 direction, float strength)
    {
        ModuleManager.get<PlayerEventmanager>().Move(GetComponent<Rigidbody2D>().velocity);
        GetComponent<Rigidbody2D>().AddForce(new Vector3(direction.x * strength, direction.y * strength, direction.z));
    }
}
