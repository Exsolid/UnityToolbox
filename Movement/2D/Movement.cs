using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
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
        Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, Camera.main.transform.position.z);
    }

    public void move(Vector3 direction)
    {
        if (direction.x != 0 || direction.y != 0) ModuleManager.get<LevelEventmanager>().Moving(GetComponent<Rigidbody2D>().velocity);
        GetComponent<Rigidbody2D>().AddForce(new Vector3(direction.x * speed, 0, direction.z * speed));
    }

    public void moveWithStrength(Vector3 direction, float strength)
    {
        if (direction.x != 0 || direction.y != 0) ModuleManager.get<LevelEventmanager>().Moving(GetComponent<Rigidbody2D>().velocity);
        GetComponent<Rigidbody2D>().AddForce(new Vector3(direction.x * strength, direction.y * strength, direction.z));
    }
}
