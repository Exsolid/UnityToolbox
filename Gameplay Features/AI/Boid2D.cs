using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A boid implementation which listens to the three rules cohesion, separation and alignment. Additionally, object attraction and avoidance can be set.
/// Requires <see cref="ColliderInfo"/> to work.
/// </summary>
public class Boid2D : MonoBehaviour
{
    [SerializeField] private ColliderInfo _detectionRange;
    [SerializeField] private LayerMask _boidMask;
    [SerializeField] private LayerMask _avoidMask;
    [SerializeField] private LayerMask _attractMask;
    [SerializeField] private float _minRangeBoids;
    [SerializeField] private float _minRangeObstacles;
    [SerializeField] private float _minRangeAttraction;
    [SerializeField] private float _rotationSpeed;
    [SerializeField] private float _speed;
    [SerializeField] private bool _boundsTeleporation;

    private Camera _camera;
    private Vector2 _bounds;

    private GameObject _nearestBoid;
    private GameObject _nearestAttraction;

    private GameObject _nearestObstacle;
    private GameObject _secondNearestObstacle;

    private Vector3 _avgAlignment;
    private Vector3 _avgPos;

    private void Start()
    {
        _camera = Camera.main;
        _bounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, _camera.transform.position.z));
    }

    void Update()
    {
        GetInformation();

        Cohesion();
        Alignment();
        Seperation();

        ObjectAttraction();
        ObjectAvoidance();

        MoveToRotation();

        if (_boundsTeleporation)
        {
            StayInBordersTeleport();
        }
    }

    private void MoveToRotation()
    {
        transform.position += transform.up * _speed * Time.deltaTime;
    }

    private void GetInformation()
    {
        List<Collider2D> nearby = _detectionRange.GetAllCollisions2D(_avoidMask);
        nearby.AddRange(_detectionRange.GetAllCollisions2D(_boidMask));
        nearby.AddRange(_detectionRange.GetAllCollisions2D(_attractMask));

        ResetInformation(nearby);

        foreach (Collider2D col in nearby)
        {
            if (gameObject.Equals(col.gameObject))
            {
                continue;
            }

            if ((1 << col.gameObject.layer).Equals(_boidMask))
            {
                _avgPos += col.transform.position;
                _avgAlignment += col.transform.up;
                if (Vector3.Distance(col.transform.position, transform.position) < _minRangeBoids && (_nearestBoid == null || Vector3.Distance(col.transform.position, transform.position) < Vector3.Distance(_nearestBoid.transform.position, transform.position)))
                {
                    _nearestBoid = col.gameObject;
                }
            }
            else if ((1 << col.gameObject.layer).Equals(_avoidMask))
            {
                float distance = Vector3.Distance(transform.position, col.GetComponent<Collider2D>().ClosestPoint(transform.position));
                if (distance < _minRangeObstacles && (_nearestObstacle == null || Vector3.Distance(col.GetComponent<Collider2D>().ClosestPoint(transform.position), transform.position) < Vector3.Distance(_nearestObstacle.GetComponent<Collider2D>().ClosestPoint(transform.position), transform.position)))
                {
                    _secondNearestObstacle = _nearestObstacle;
                    _nearestObstacle = col.gameObject;
                }
            }
            else if ((1 << col.gameObject.layer).Equals(_attractMask))
            {
                float distance = Vector3.Distance(transform.position, col.GetComponent<Collider2D>().ClosestPoint(transform.position));
                if (distance < _minRangeAttraction && (_nearestAttraction == null || Vector3.Distance(col.GetComponent<Collider2D>().ClosestPoint(transform.position), transform.position) < Vector3.Distance(_nearestAttraction.GetComponent<Collider2D>().ClosestPoint(transform.position), transform.position)))
                {
                    _nearestAttraction = col.gameObject;
                }
            }
        }

        _avgPos *= 1f/nearby.Count;
        _avgAlignment *= 1f/nearby.Count;
    }

    private void ResetInformation(List<Collider2D> nearby)
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
                float distance = Vector3.Distance(transform.position, _nearestObstacle.GetComponent<Collider2D>().ClosestPoint(transform.position));
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
                float distance = Vector3.Distance(transform.position, _secondNearestObstacle.GetComponent<Collider2D>().ClosestPoint(transform.position));
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
            MoveDirection(transform.position+_avgAlignment, false);
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

    private void MoveDirection(Vector3 position, bool away)
    {
        float sgn = GetSide(transform.position, transform.position + transform.up, position);
        transform.Rotate(new Vector3(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.x + _rotationSpeed * sgn * (away ? -1 : 1) * Time.deltaTime), Space.Self);
    }
}
