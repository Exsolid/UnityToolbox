using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityToolbox.GameplayFeatures.ProceduralGeneration.Enums;

namespace UnityToolbox.GameplayFeatures.ProceduralGeneration.Terrain
{
    /// <summary>
    /// A script which places decoration on a generated terrain.
    /// </summary>
    public class TerrainDecoration : MonoBehaviour
    {
        [SerializeField] [Range(0.1f, 1)] private float _pctForNone;
        [SerializeField] private TerrainGenerationAssetPosition _positionType;
        [SerializeField] private bool _raycastPlacement;
        [SerializeField] private LayerMask _groundMask;

        [SerializeField] private List<TerrainDecorationInformation> _objectsToPlace;
        [SerializeField] private TerrainGenerator _terrainGenerator;

        [SerializeField] [HideInInspector] private List<GameObject> _spawnedObjects;

        private List<TerrainDecorationInformation> _weightedObjects;

        /// <summary>
        /// Deletes all spawned objects.
        /// </summary>
        public void DeleteObjects()
        {
            foreach (GameObject obj in _spawnedObjects)
            {
                DestroyImmediate(obj);
            }
            _spawnedObjects = new List<GameObject>();
        }

        /// <summary>
        /// Places all defined objects.
        /// </summary>
        public void PlaceObjects()
        {
            _weightedObjects = new List<TerrainDecorationInformation>();

            float currentWeight = 0;
            foreach (TerrainDecorationInformation info in _objectsToPlace)
            {
                currentWeight += info.Weight;
                TerrainDecorationInformation newInfo = (TerrainDecorationInformation)info.Clone();
                newInfo.Weight = currentWeight;
                _weightedObjects.Add(newInfo);
            }
            _weightedObjects.OrderBy(obj => obj.Weight);

            if (_spawnedObjects == null)
            {
                _spawnedObjects = new List<GameObject>();
            }
            else
            {
                foreach (GameObject obj in _spawnedObjects)
                {
                    DestroyImmediate(obj);
                }
                _spawnedObjects = new List<GameObject>();
            }

            int[,] grid = _terrainGenerator.GeneratedGrid;
            Mesh mesh = _terrainGenerator.GeneratedMesh;

            for (int y = 0; y < grid.GetLength(1); y++)
            {
                for (int x = 0; x < grid.GetLength(0); x++)
                {
                    if (CheckPositionType(x, y))
                    {
                        float rnd = Random.Range(0f, 1f);

                        if (rnd < _pctForNone)
                        {
                            continue;
                        }

                        rnd = Random.Range(0, _weightedObjects.Last().Weight);
                        TerrainDecorationInformation selected = _weightedObjects.Where(obj => rnd <= obj.Weight).OrderBy(obj => obj.Weight).FirstOrDefault();
                        Vector3 position = mesh.vertices[grid.GetLength(1) * y + x];

                        if (_raycastPlacement)
                        {
                            _spawnedObjects.Add(Instantiate(selected.Object, position + Vector3.up * 2, Quaternion.Euler(0, Random.Range(0, 350), 0)));

                            _spawnedObjects.Last().transform.parent = transform;


                            if (_spawnedObjects.Last().GetComponent<TerrainDecorationChildInfo>() == null || !_spawnedObjects.Last().GetComponent<TerrainDecorationChildInfo>().RelativeToParent)
                            {
                                RotationPlacementForChildren(_spawnedObjects.Last(), selected.HeightOffset, selected.WidthPlacement);
                            }

                            if (_spawnedObjects.Count != 0 && _spawnedObjects.Last().GetComponent<Renderer>() != null)
                            {
                                Collider[] collisions = Physics.OverlapBox(_spawnedObjects.Last().transform.position, _spawnedObjects.Last().GetComponent<Renderer>().bounds.extents, _spawnedObjects.Last().transform.rotation);

                                foreach (Collider col in collisions)
                                {
                                    if (col.gameObject.GetComponent<TerrainDecorationAnchorObject>() != null)
                                    {
                                        GameObject obj = _spawnedObjects.Last();
                                        _spawnedObjects.Remove(obj);
                                        DestroyImmediate(obj);
                                        break;
                                    }
                                }
                            }

                        }
                        else
                        {
                            _spawnedObjects.Add(Instantiate(selected.Object, position + Vector3.up * selected.HeightOffset, Quaternion.Euler(0, Random.Range(0, 350), 0)));
                            _spawnedObjects.Last().transform.parent = transform;
                        
                            CheckForAnchorsRecursiv(_spawnedObjects.Last().transform);
                            if (_spawnedObjects.Last().GetComponent<Renderer>() != null)
                            {
                                Collider[] collisions = Physics.OverlapBox(_spawnedObjects.Last().transform.position, _spawnedObjects.Last().GetComponent<Renderer>().bounds.extents, _spawnedObjects.Last().transform.rotation);

                                foreach (Collider col in collisions)
                                {
                                    if (col.gameObject.GetComponent<TerrainDecorationAnchorObject>() != null)
                                    {
                                        GameObject obj = _spawnedObjects.Last();
                                        _spawnedObjects.Remove(obj);
                                        DestroyImmediate(obj);
                                        break;
                                    }
                                }
                            }

                        }
                    }
                }
            }
        }

        private bool CheckPositionType(int x, int y)
        {
            int[,] grid = _terrainGenerator.GeneratedGrid;
            switch (_positionType)
            {
                //case TerrainGenerationAssetPosition.Ground:
                //case TerrainGenerationAssetPosition.CliffCornerBottom:
                //    if (grid[x, y] != (int)TerrainValues.Floor)
                //    {
                //        return false;
                //    }
                //    break;
                //case TerrainGenerationAssetPosition.CliffGround:
                //case TerrainGenerationAssetPosition.CliffCornerTop:
                //    if (grid[x, y] != (int)TerrainValues.Wall)
                //    {
                //        return false;
                //    }
                //    break;
            }

            int wallCount = 0;
            for (int yN = -2; yN <= 2; yN++)
            {
                for (int xN = -2; xN <= 2; xN++)
                {
                    if (yN + y >= 0 && yN + y < grid.GetLength(1) && xN + x >= 0 && xN + x < grid.GetLength(0) && grid[x + xN, y + yN] == (int)TerrainValues.Wall && !(x == 0 && y == 0))
                    {
                        wallCount++;
                    }
                }
            }

            switch (_positionType)
            {
                //case TerrainGenerationAssetPosition.Ground:
                //    if (wallCount == 0)
                //    {
                //        return true;
                //    }
                //    break;
                //case TerrainGenerationAssetPosition.CliffCornerBottom:
                //case TerrainGenerationAssetPosition.CliffCornerTop:
                //    if (wallCount < 7 && wallCount > 0)
                //    {
                //        return true;
                //    }
                //    break;
                //case TerrainGenerationAssetPosition.CliffGround:
                //    if (wallCount >= 7)
                //    {
                //        return true;
                //    }
                //    break;
            }

            return false;
        }

        private void RotationPlacementForChildren(GameObject parent, float heightOffset, bool widthPlacement)
        {
            RotationPlacementForChildrenRecursiv(parent, heightOffset, widthPlacement);

            RaycastHit hit;
            Physics.Raycast(parent.transform.position, Vector3.down, out hit, 10, _groundMask);

            if (parent.transform.GetComponent<Renderer>() != null && widthPlacement && hit.collider != null)
            {
                Vector3 bounds = parent.transform.GetComponent<Renderer>().bounds.extents;
                List<RaycastHit> hits = new List<RaycastHit>();

                RaycastHit hit1;
                RaycastHit hit2;
                RaycastHit hit3;
                RaycastHit hit4;
                RaycastHit hit5;
                RaycastHit hit6;
                RaycastHit hit7;
                RaycastHit hit8;

                Physics.Raycast(parent.transform.position + new Vector3(bounds.x, 0, bounds.z), Vector3.down, out hit1, 10, _groundMask);
                Physics.Raycast(parent.transform.position + new Vector3(-bounds.x, 0, bounds.z), Vector3.down, out hit2, 10, _groundMask);
                Physics.Raycast(parent.transform.position + new Vector3(bounds.x, 0, -bounds.z), Vector3.down, out hit3, 10, _groundMask);
                Physics.Raycast(parent.transform.position + new Vector3(-bounds.x, 0, -bounds.z), Vector3.down, out hit4, 10, _groundMask);

                Physics.Raycast(parent.transform.position + new Vector3(bounds.x, 0, 0), Vector3.down, out hit5, 10, _groundMask);
                Physics.Raycast(parent.transform.position + new Vector3(0, 0, bounds.z), Vector3.down, out hit6, 10, _groundMask);
                Physics.Raycast(parent.transform.position + new Vector3(0, 0, -bounds.z), Vector3.down, out hit7, 10, _groundMask);
                Physics.Raycast(parent.transform.position + new Vector3(-bounds.x, 0, 0), Vector3.down, out hit8, 10, _groundMask);

                hits.Add(hit1);
                hits.Add(hit2);
                hits.Add(hit3);
                hits.Add(hit4);
                hits.Add(hit5);
                hits.Add(hit6);
                hits.Add(hit7);
                hits.Add(hit8);

                foreach (RaycastHit rHit in hits)
                {
                    if (rHit.collider != null && rHit.point.y < hit.point.y)
                    {
                        hit.point = new Vector3(hit.point.x, rHit.point.y, hit.point.z);
                    }
                }
            }

            if (hit.collider != null)
            {
                float heightDiff = heightOffset;

                if (parent.transform.GetComponent<Renderer>() != null)
                {
                    heightDiff += parent.transform.position.y - parent.transform.GetComponent<Renderer>().bounds.min.y;
                }

                parent.transform.position = hit.point + Vector3.up * heightDiff;

                if (parent.transform.GetComponent<Renderer>() != null && parent.transform.GetComponent<Renderer>().bounds.size.y < 0.8f)
                {
                    GameObject pivot = new GameObject();
                    pivot.transform.position = hit.point;
                    parent.transform.parent = pivot.transform;
                    pivot.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
                    parent.transform.parent = this.transform;
                    DestroyImmediate(pivot);
                }
            }
            else
            {
                _spawnedObjects.Remove(parent.transform.gameObject);
                DestroyImmediate(parent.transform.gameObject);
            }
        }

        private void RotationPlacementForChildrenRecursiv(GameObject parent, float heightOffset, bool widthPlacement)
        {
            List<Transform> unchangedChildren = parent.transform.Cast<Transform>().ToList();
            foreach (Transform transform in unchangedChildren)
            {
                if (transform.GetComponent<ParticleSystem>() != null || transform.GetComponent<Light>() != null)
                {
                    continue;
                }

                if (transform.GetComponent<TerrainDecorationChildInfo>() == null || !transform.GetComponent<TerrainDecorationChildInfo>().RelativeToParent)
                {
                    RotationPlacementForChildrenRecursiv(transform.gameObject, heightOffset, widthPlacement);
                    transform.parent = this.transform;
                    _spawnedObjects.Add(transform.gameObject);
                }
                else
                {
                    continue;
                }

                RaycastHit hit;
                Physics.Raycast(transform.position, Vector3.down, out hit, 10, _groundMask);

                if (transform.GetComponent<Renderer>() != null && widthPlacement && hit.collider != null)
                {
                    Vector3 bounds = transform.GetComponent<Renderer>().bounds.extents;
                    List<RaycastHit> hits = new List<RaycastHit>();

                    RaycastHit hit1;
                    RaycastHit hit2;
                    RaycastHit hit3;
                    RaycastHit hit4;
                    RaycastHit hit5;
                    RaycastHit hit6;
                    RaycastHit hit7;
                    RaycastHit hit8;

                    Physics.Raycast(transform.position + new Vector3(bounds.x, 0, bounds.z), Vector3.down, out hit1, 10, _groundMask);
                    Physics.Raycast(transform.position + new Vector3(-bounds.x, 0, bounds.z), Vector3.down, out hit2, 10, _groundMask);
                    Physics.Raycast(transform.position + new Vector3(bounds.x, 0, -bounds.z), Vector3.down, out hit3, 10, _groundMask);
                    Physics.Raycast(transform.position + new Vector3(-bounds.x, 0, -bounds.z), Vector3.down, out hit4, 10, _groundMask);

                    Physics.Raycast(transform.position + new Vector3(bounds.x, 0, 0), Vector3.down, out hit5, 10, _groundMask);
                    Physics.Raycast(transform.position + new Vector3(0, 0, bounds.z), Vector3.down, out hit6, 10, _groundMask);
                    Physics.Raycast(transform.position + new Vector3(0, 0, -bounds.z), Vector3.down, out hit7, 10, _groundMask);
                    Physics.Raycast(transform.position + new Vector3(-bounds.x, 0, 0), Vector3.down, out hit8, 10, _groundMask);

                    hits.Add(hit1);
                    hits.Add(hit2);
                    hits.Add(hit3);
                    hits.Add(hit4);
                    hits.Add(hit5);
                    hits.Add(hit6);
                    hits.Add(hit7);
                    hits.Add(hit8);

                    foreach (RaycastHit rHit in hits)
                    {
                        if (rHit.collider != null && rHit.point.y < hit.point.y)
                        {
                            hit.point = new Vector3(hit.point.x, rHit.point.y, hit.point.z);
                        }
                    }
                }

                if (hit.collider != null)
                {
                    float heightDiff = heightOffset;

                    if (transform.GetComponent<Renderer>() != null)
                    {
                        heightDiff += transform.position.y - transform.GetComponent<Renderer>().bounds.min.y;
                    }

                    transform.position = hit.point + Vector3.up * heightDiff;

                    if (transform.GetComponent<Renderer>() != null && transform.GetComponent<Renderer>().bounds.size.y < 0.8f)
                    {
                        GameObject pivot = new GameObject();
                        pivot.transform.position = hit.point;
                        transform.parent = pivot.transform;
                        pivot.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
                        transform.parent = this.transform;
                        DestroyImmediate(pivot);
                    }
                }
                else
                {
                    DestroyImmediate(transform.gameObject);
                    continue;
                }

                if (transform.GetComponent<Renderer>() != null)
                {
                    Collider[] collisions = Physics.OverlapBox(transform.transform.position, transform.GetComponent<Renderer>().bounds.extents, transform.transform.rotation);
                    foreach (Collider col in collisions)
                    {
                        if (col.gameObject.GetComponent<TerrainDecorationAnchorObject>() != null)
                        {
                            DestroyImmediate(transform.gameObject);
                            break;
                        }
                    }
                }
            }
        }

        private void CheckForAnchorsRecursiv(Transform parent)
        {
            List<Transform> unchangedChildren = parent.transform.Cast<Transform>().ToList();
            foreach (Transform transform in unchangedChildren)
            {
                CheckForAnchorsRecursiv(transform);
                if (transform.GetComponent<Renderer>() != null)
                {
                    Collider[] collisions = Physics.OverlapBox(transform.transform.position, transform.GetComponent<Renderer>().bounds.extents, transform.transform.rotation);
                    foreach (Collider col in collisions)
                    {
                        if (col.gameObject.GetComponent<TerrainDecorationAnchorObject>() != null)
                        {
                            DestroyImmediate(transform.gameObject);
                            break;
                        }
                    }
                }
            }
        }
    }
}
