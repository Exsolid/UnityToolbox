using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// A helping script which saves all current colliders, colliding with the current object.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class ColliderInfo : MonoBehaviour
{
    private HashSet<Collider> _colliders;
    [SerializeField] private bool _lookForTrigger = true;
    [SerializeField] private bool _lookForCollision = true;

    private void OnCollisionEnter(Collision collision)
    {
        if (_colliders == null)
        {
            _colliders = new HashSet<Collider>();
        }

        if (_lookForCollision)
        {
            _colliders.Add(collision.collider);
        } 
    }

    private void OnCollisionExit(Collision collision)
    {
        if (_colliders == null)
        {
            _colliders = new HashSet<Collider>();
        }

        if (_lookForCollision)
        {
            _colliders.Remove(collision.collider);
        }
    }

     private void OnTriggerEnter(Collider collider)
    {
        if (_colliders == null)
        {
            _colliders = new HashSet<Collider>();
        }

        if (_lookForTrigger)
        {
            _colliders.Add(collider);
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        if (_colliders == null)
        {
            _colliders = new HashSet<Collider>();
        }

        if (_lookForTrigger)
        {
            _colliders.Remove(collider);
        }
    }

    /// <summary>
    /// Filters all current collisions with a layerMask.
    /// </summary>
    /// <param name="layerMask">The layer mask of the wanted collisions.</param>
    /// <returns>A list of current colliders.</returns>
    public List<Collider> GetAllCollisions(LayerMask layerMask)
    {
        List<Collider> result = new List<Collider>();
        result.AddRange(_colliders.Where(col => layerMask == (layerMask | (1 << col.gameObject.layer))));
        return result;
    }

    /// <summary>
    /// Returns all current colliders.
    /// </summary>
    /// <returns>A list of current colliders.</returns>
    public List<Collider> GetAllCollisions()
    {
        return _colliders == null ? new List<Collider>() : _colliders.ToList();
    }
}
