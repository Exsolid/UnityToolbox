using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A simple script which lets a given <see cref="Camera"/> follow the object this script sits on.
/// </summary>
public class DelayedCameraFollow : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private Collider2D _range;
    [SerializeField] private float _speed;

    private Vector3 initiaOffset;
    // Start is called before the first frame update
    void Start()
    {
        initiaOffset = Camera.main.transform.position - transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 currentPos = _range.ClosestPoint(_camera.transform.position);
        if (currentPos.x != _camera.transform.position.x || currentPos.y != _camera.transform.position.y)
        {
            _camera.transform.position = Vector3.Lerp(_camera.transform.position, new Vector3(currentPos.x, currentPos.y, initiaOffset.z), Time.deltaTime * _speed);
        }
    }
}
