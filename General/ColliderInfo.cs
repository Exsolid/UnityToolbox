using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnityToolbox.General
{
    /// <summary>
    /// A helping script which saves all current colliders, colliding with the current object.
    /// </summary>
    public class ColliderInfo : MonoBehaviour
    {
        private HashSet<Collider> _colliders;
        private HashSet<Collider2D> _colliders2D;
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

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (_colliders2D == null)
            {
                _colliders2D = new HashSet<Collider2D>();
            }

            if (_lookForCollision)
            {
                _colliders2D.Add(collision.collider);
            }
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            if (_colliders2D == null)
            {
                _colliders2D = new HashSet<Collider2D>();
            }

            if (_lookForCollision)
            {
                _colliders2D.Remove(collision.collider);
            }
        }

        private void OnTriggerEnter2D(Collider2D collider)
        {
            if (_colliders2D == null)
            {
                _colliders2D = new HashSet<Collider2D>();
            }

            if (_lookForTrigger)
            {
                _colliders2D.Add(collider);
            }
        }

        private void OnTriggerExit2D(Collider2D collider)
        {
            if (_colliders2D == null)
            {
                _colliders2D = new HashSet<Collider2D>();
            }

            if (_lookForTrigger)
            {
                _colliders2D.Remove(collider);
            }
        }

        /// <summary>
        /// Filters all current collisions with a layerMask.
        /// </summary>
        /// <param name="layerMask">The layer mask of the wanted collisions.</param>
        /// <returns>A list of current colliders.</returns>
        public List<Collider> GetAllCollisions(LayerMask layerMask)
        {
            if (_colliders == null)
            {
                _colliders = new HashSet<Collider>();
            }

            List<Collider> result = new List<Collider>();
            result.AddRange(_colliders.Where(col => col != null && layerMask == (layerMask | (1 << col.gameObject.layer))));
            return result;
        }

        /// <summary>
        /// Filters all current 2D collisions with a layerMask.
        /// </summary>
        /// <param name="layerMask">The layer mask of the wanted collisions.</param>
        /// <returns>A list of current colliders.</returns>
        public List<Collider2D> GetAllCollisions2D(LayerMask layerMask)
        {
            if (_colliders2D == null)
            {
                _colliders2D = new HashSet<Collider2D>();
            }

            List<Collider2D> result = new List<Collider2D>();
            result.AddRange(_colliders2D.Where(col => col != null && layerMask == (layerMask | (1 << col.gameObject.layer))));
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

        /// <summary>
        /// Returns all current 2D colliders.
        /// </summary>
        /// <returns>A list of current colliders.</returns>
        public List<Collider2D> GetAllCollisions2D()
        {
            return _colliders2D == null ? new List<Collider2D>() : _colliders2D.ToList();
        }
    }
}
