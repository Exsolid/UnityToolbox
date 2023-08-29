using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace UnityToolbox.AI.Boids
{
    /// <summary>
    /// An abstract boid implementation which listens to the three rules cohesion, separation and alignment. Additionally, object attraction and avoidance can be set.
    /// When implemented, additional checks can be added.
    /// Requires <see cref="ColliderInfo"/> to work.
    /// </summary>
    public abstract class Boid2DBase : MonoBehaviour
    {
        [SerializeField] private ColliderInfo _detectionRange;
        [SerializeField] private LayerMask _boidMask;
        /// <summary>
        /// The layer mask which indentifies what the boid should see as other boids. These objects require an implementation of <see cref="Boid2DBase"/>.
        /// </summary>
        public LayerMask BoidMask
        {
            get { return _boidMask; }
            set { _boidMask = value; }
        }

        [SerializeField] private LayerMask _avoidMask;
        /// <summary>
        /// The layer mask which indentifies what the boid should see as obstacles.
        /// </summary>
        public LayerMask AvoidMask
        {
            get { return _avoidMask; }
            set { _avoidMask = value; }
        }

        [SerializeField] private LayerMask _attractMask;
        /// <summary>
        /// The layer mask which indentifies what the boid should see as objects to be attracted to.
        /// </summary>
        public LayerMask AttractMask
        {
            get { return _attractMask; }
            set { _attractMask = value; }
        }

        [SerializeField] private float _minRangeBoids;
        [SerializeField] private float _minRangeObstacles;
        [SerializeField] private float _minRangeAttraction;
        [SerializeField] private float _rotationSpeed;
        [SerializeField] private float _speed;
        [SerializeField] private bool _boundsTeleporation;

        private Camera _camera;
        private Vector2 _bounds;

        /// <summary>
        /// The closest boid within the detection range.
        /// </summary>
        protected GameObject _nearestBoid;
        /// <summary>
        /// The second closest boid within the detection range.
        /// </summary>
        protected GameObject _nearestAttraction;


        /// <summary>
        /// The closest obstacle within the detection range.
        /// </summary>
        protected GameObject _nearestObstacle;
        /// <summary>
        /// The second closest obstacle within the detection range.
        /// </summary>
        protected GameObject _secondNearestObstacle;

        /// <summary>
        /// The average alignment of all boids within the detection range.
        /// </summary>
        protected Vector3 _avgAlignment;
        /// <summary>
        /// The average position of all boids within the detection range.
        /// </summary>
        protected Vector3 _avgPos;

        /// <summary>
        /// If set to true, disables all rotations calculated by the boid. The boid will now only move forward if not rotated by other means.
        /// </summary>
        protected bool _disableBasicRotation;

        /// <summary>
        /// Is set to true, all rotations, movment and calculations of values will be disabled.
        /// </summary>
        protected bool _paused;

        private void Start()
        {
            _camera = Camera.main;
            _bounds = _camera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, transform.position.z - _camera.transform.position.z + 1));

            if (ModuleManager.ModuleRegistered<BoidManager>())
            {
                ModuleManager.GetModule<BoidManager>().OnPauseBoids += TogglePause;
            }
        }

        public void Update()
        {
            if (_paused)
            {
                return;
            }

            GetInformation();

            if (!_disableBasicRotation)
            {
                Cohesion();
                Alignment();
                Seperation();

                ObjectAttraction();
                ObjectAvoidance();
            }

            MoveToRotation();

            if (_boundsTeleporation)
            {
                StayInBordersTeleport();
            }
        }

        private void OnDestroy()
        {
            if (ModuleManager.ModuleRegistered<BoidManager>())
            {
                ModuleManager.GetModule<BoidManager>().OnPauseBoids -= TogglePause;
            }
        }

        private void TogglePause(bool paused)
        {
            _paused = paused;
        }

        /// <summary>
        /// An additional implementation which can filter the nearest object even further.
        /// </summary>
        /// <param name="nearest">The nearest found object to avoid.</param>
        /// <returns>Whether the object should be taken into account.</returns>
        protected abstract bool AdditionalAvoidanceCheck(Collider2D nearest);

        /// <summary>
        /// An additional implementation which can filter the nearest object even further.
        /// </summary>
        /// <param name="nearest">The nearest found object which attracts.</param>
        /// <returns>Whether the object should be taken into account.</returns>
        protected abstract bool AdditionalAttractionCheck(Collider2D nearest);

        /// <summary>
        /// An additional implementation which can filter the nearest boid even further.
        /// </summary>
        /// <param name="nearest">The nearest found boid.</param>
        /// <returns>Whether the object should be taken into account.</returns>
        protected abstract bool AdditionalBoidCheck(Collider2D nearest);

        private void MoveToRotation()
        {
            transform.position += transform.up * _speed * Time.deltaTime;
        }

        private void GetInformation()
        {
            HashSet<Collider2D> nearby = new HashSet<Collider2D>();
            nearby.AddRange(_detectionRange.GetAllCollisions2D(_avoidMask));
            nearby.AddRange(_detectionRange.GetAllCollisions2D(_boidMask));
            nearby.AddRange(_detectionRange.GetAllCollisions2D(_attractMask));
            ResetInformation(nearby);

            int boidCounter = 0;
            foreach (Collider2D col in nearby)
            {
                if (gameObject.Equals(col.gameObject))
                {
                    continue;
                }

                if (((_boidMask & (1 << col.gameObject.layer)) != 0))
                {
                    if (AdditionalBoidCheck(col))
                    {
                        float distance = Vector3.Distance(transform.position, col.transform.position);
                        _avgPos += col.transform.position;
                        _avgAlignment += col.transform.up;
                        boidCounter++;

                        if (distance < _minRangeBoids && (_nearestBoid == null || Vector3.Distance(col.transform.position, transform.position) < Vector3.Distance(_nearestBoid.transform.position, transform.position)))
                        {
                            _nearestBoid = col.gameObject;
                        }
                    }
                }

                if (((_avoidMask & (1 << col.gameObject.layer)) != 0))
                {
                    float distance = Vector3.Distance(transform.position, col.GetComponent<Collider2D>().ClosestPoint(transform.position)) - transform.position.z;
                    if (distance < _minRangeObstacles && (_nearestObstacle == null || Vector3.Distance(col.GetComponent<Collider2D>().ClosestPoint(transform.position), transform.position) < Vector3.Distance(_nearestObstacle.GetComponent<Collider2D>().ClosestPoint(transform.position), transform.position)))
                    {
                        if (AdditionalAvoidanceCheck(col))
                        {
                            _secondNearestObstacle = _nearestObstacle;
                            _nearestObstacle = col.gameObject;
                        }
                    }
                }

                if (((_attractMask & (1 << col.gameObject.layer)) != 0))
                {
                    float distance = Vector3.Distance(transform.position, col.GetComponent<Collider2D>().ClosestPoint(transform.position)) - transform.position.z;
                    if (distance < _minRangeAttraction && (_nearestAttraction == null || Vector3.Distance(col.GetComponent<Collider2D>().ClosestPoint(transform.position), transform.position) < Vector3.Distance(_nearestAttraction.GetComponent<Collider2D>().ClosestPoint(transform.position), transform.position)))
                    {
                        if (AdditionalAttractionCheck(col))
                        {
                            _nearestAttraction = col.gameObject;
                        }
                    }
                }
            }

            if (boidCounter != 0)
            {
                _avgPos /= boidCounter;
                _avgAlignment /= boidCounter;
            }
        }

        private void ResetInformation(HashSet<Collider2D> nearby)
        {
            _avgPos = new Vector3(0, 0, 0);
            _avgAlignment = new Vector3(0, 0, 0);

            if (_nearestObstacle != null)
            {
                if (!nearby.Contains(_nearestObstacle.GetComponent<Collider2D>()))
                {
                    _nearestObstacle = null;
                }
                else
                {
                    float distance = Vector3.Distance(transform.position, _nearestObstacle.GetComponent<Collider2D>().ClosestPoint(transform.position)) - transform.position.z;
                    if (distance > _minRangeObstacles)
                    {
                        _nearestObstacle = null;
                    }
                }
            }

            if (_secondNearestObstacle != null)
            {
                if (!nearby.Contains(_secondNearestObstacle.GetComponent<Collider2D>()))
                {
                    _secondNearestObstacle = null;
                }
                else
                {
                    float distance = Vector3.Distance(transform.position, _secondNearestObstacle.GetComponent<Collider2D>().ClosestPoint(transform.position)) - transform.position.z;
                    if (distance > _minRangeObstacles)
                    {
                        _secondNearestObstacle = null;
                    }
                }
            }

            if (_nearestBoid != null && (!nearby.Contains(_nearestBoid.GetComponent<Collider2D>()) || Vector3.Distance(_nearestBoid.transform.position, transform.position) < _minRangeBoids))
            {
                _nearestBoid = null;
            }

            if (_nearestAttraction != null && (!nearby.Contains(_nearestAttraction.GetComponent<Collider2D>()) || Vector3.Distance(_nearestAttraction.transform.position, transform.position) < _minRangeAttraction))
            {
                _nearestAttraction = null;
            }
        }

        private void StayInBordersTeleport()
        {
            if (_camera.transform.position.x + _bounds.x < transform.position.x)
            {
                transform.position = new Vector3(_camera.transform.position.x - _bounds.x, transform.position.y, transform.position.z);
            }

            if (_camera.transform.position.x - _bounds.x > transform.position.x)
            {
                transform.position = new Vector3(_camera.transform.position.x + _bounds.x, transform.position.y, transform.position.z);
            }

            if (_camera.transform.position.y + _bounds.y < transform.position.y)
            {
                transform.position = new Vector3(transform.position.x, _camera.transform.position.y - _bounds.y, transform.position.z);
            }

            if (_camera.transform.position.y - _bounds.y > transform.position.y)
            {
                transform.position = new Vector3(transform.position.x, _camera.transform.position.y + _bounds.y, transform.position.z);
            }
        }

        private void ObjectAvoidance()
        {
            if (_nearestObstacle != null)
            {
                Vector2 avoidPoint = _nearestObstacle.GetComponent<Collider2D>().ClosestPoint(transform.position);
                if (_secondNearestObstacle != null && !_secondNearestObstacle.Equals(_nearestObstacle))
                {
                    avoidPoint += _secondNearestObstacle.GetComponent<Collider2D>().ClosestPoint(transform.position);
                    avoidPoint *= 0.5f;
                }

                MoveDirection(new Vector3(avoidPoint.x, avoidPoint.y, 0), true);
            }
        }

        private void ObjectAttraction()
        {
            if (_nearestAttraction != null && _nearestObstacle == null)
            {
                Vector2 attractionPoint = _nearestAttraction.GetComponent<Collider2D>().ClosestPoint(transform.position);
                MoveDirection(new Vector3(attractionPoint.x, attractionPoint.y, 0), false);
            }
        }

        private void Seperation()
        {
            if (_nearestBoid != null && _nearestObstacle == null)
            {
                Vector3 invertedNDir = (transform.position - _nearestBoid.transform.position).normalized;
                Vector3 nDir = _nearestBoid.transform.up;
                Vector3 sDir = transform.up;

                float sDegreeToAway = Mathf.Acos(Vector3.Dot(invertedNDir, sDir) / (invertedNDir.magnitude * sDir.magnitude));
                float nDegreeToAway = Mathf.Acos(Vector3.Dot(invertedNDir, nDir) / (invertedNDir.magnitude * nDir.magnitude));

                if (nDegreeToAway < sDegreeToAway + 0.1f)
                {
                    MoveDirection(_nearestBoid.transform.position, true);
                }
            }
        }

        private void Alignment()
        {
            if (!_avgPos.Equals(Vector3.zero) && _nearestBoid == null && _nearestObstacle == null)
            {
                MoveDirection(transform.position + _avgAlignment, false);
            }
        }

        private void Cohesion()
        {
            if (!_avgPos.Equals(Vector3.zero) && _nearestBoid == null && _nearestObstacle == null)
            {
                MoveDirection(_avgPos, false);
            }
        }

        private float GetSide(Vector3 A, Vector3 B, Vector3 C)
        {
            return Mathf.Sign((B.x - A.x) * (C.y - A.y) - (B.y - A.y) * (C.x - A.x));
        }

        public void MoveDirection(Vector3 position, bool away)
        {
            float sgn = GetSide(transform.position, transform.position + transform.up, position);
            transform.Rotate(new Vector3(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.x + _rotationSpeed * sgn * (away ? -1 : 1) * Time.deltaTime), Space.Self);
        }
    }
}
