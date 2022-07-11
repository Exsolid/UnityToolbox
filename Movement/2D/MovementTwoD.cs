using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerInput))]
public class MovementTwoD : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private float _jumpForce;
    [SerializeField] private Transform _bottomTransform;
    [SerializeField] private string _actionName;
    private RaycastHit2D _grounded;
    private PlayerInput _input;
    private Rigidbody2D _rb;

    // Start is called before the first frame update
    void Start()
    {
        _input = GetComponent<PlayerInput>();
        _rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        int everyMaskExcept =~ LayerMask.GetMask("Player");
        _grounded = Physics2D.Raycast(_bottomTransform.position, transform.up * -1, 0.08f, everyMaskExcept);
        move(_input.actions[_actionName].ReadValue<Vector2>());
    }

    public void move(Vector3 direction)
    {
        ModuleManager.GetModule<PlayerEventmanager>().Move(_rb.velocity);
        _rb.AddForce(new Vector3(direction.x * _speed, 0, direction.z * _speed));
    }

    public void moveWithStrength(Vector3 direction, float strength)
    {
        ModuleManager.GetModule<PlayerEventmanager>().Move(_rb.velocity);
        _rb.AddForce(new Vector3(direction.x * strength, direction.y * strength, direction.z));
    }
}
