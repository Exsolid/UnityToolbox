using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleCameraFollow : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    private Vector3 initiaOffset;
    // Start is called before the first frame update
    void Start()
    {
        initiaOffset = Camera.main.transform.position - transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        _camera.transform.position = initiaOffset + transform.position;
    }
}
