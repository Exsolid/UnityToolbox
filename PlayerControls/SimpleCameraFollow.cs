using UnityEngine;

namespace UnityToolbox.PlayerControls
{
    /// <summary>
    /// A simple script which lets a given <see cref="Camera"/> follow the object this script sits on.
    /// </summary>
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
}
