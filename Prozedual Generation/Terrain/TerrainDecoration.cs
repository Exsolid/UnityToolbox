using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TerrainDecoration : MonoBehaviour
{
    [SerializeField] [Range(0.1f, 1)] private float _pctForNone;
    [SerializeField] private TerrainDecorationPositionType _positionType;
    [SerializeField] private bool _raycastPlacement;
    [SerializeField] private LayerMask _groundMask;

    [SerializeField] private List<TerrainDecorationInformation> _objectsToPlace;
    [SerializeField] private TerrainGenerator _terrainGenerator;

    [SerializeField] [HideInInspector] private List<GameObject> _spawnedObjects;

    private List<TerrainDecorationInformation> _weightedObjects;

    public void DeleteObjects()
    {
        foreach (GameObject obj in _spawnedObjects)
        {
            DestroyImmediate(obj);
        }
        _spawnedObjects = new List<GameObject>();
    }

    public void PlaceObjects()
    {
        _weightedObjects = new List<TerrainDecorationInformation>();

        float currentWeight = 0;
        foreach(TerrainDecorationInformation info in _objectsToPlace)
        {
            currentWeight += info.Weight;
            TerrainDecorationInformation newInfo = (TerrainDecorationInformation) info.Clone();
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
            foreach(GameObject obj in _spawnedObjects)
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
                if (CheckPositionType(x,y))
                {
                    float rnd = Random.Range(0f, 1f);

                    if(rnd < _pctForNone)
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
                        RotationPlacementForChildren(_spawnedObjects.Last(), selected.HeightOffset, selected.WidthPlacement);
                    }
                    else
                    {
                        _spawnedObjects.Add(Instantiate(selected.Object, position + Vector3.up * selected.HeightOffset, Quaternion.Euler(0, Random.Range(0, 350), 0)));
                        _spawnedObjects.Last().transform.parent = transform;
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
            case TerrainDecorationPositionType.Floor:
            case TerrainDecorationPositionType.CornerFloor:
                if(grid[x,y] != (int) TerrainValues.Floor)
                {
                    return false;
                }
                break;
            case TerrainDecorationPositionType.Wall:
            case TerrainDecorationPositionType.CornerWall:
                if (grid[x, y] != (int)TerrainValues.Wall)
                {
                    return false;
                }
                break;
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
            case TerrainDecorationPositionType.Floor:
                if (wallCount == 0)
                {
                    return true;
                }
                break;
            case TerrainDecorationPositionType.CornerFloor:
            case TerrainDecorationPositionType.CornerWall:
                if (wallCount < 7 && wallCount > 0)
                {
                    return true;
                }
                break;
            case TerrainDecorationPositionType.Wall:
                if (wallCount >= 7)
                {
                    return true;
                }
                break;
        }

        return false;
    }

    private void RotationPlacementForChildren(GameObject parent, float heightOffset, bool widthPlacement)
    {
        RotationPlacementForChildrenRecursiv(parent, heightOffset, widthPlacement);

        RaycastHit hit;
        Physics.Raycast(parent.transform.position, Vector3.down, out hit, 10, _groundMask);

        if (parent.transform.GetComponent<Collider>() != null && widthPlacement && hit.collider != null)
        {
            Vector3 bounds = parent.transform.GetComponent<Collider>().bounds.extents;
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

            if (parent.transform.GetComponent<Collider>() != null)
            {
                heightDiff += parent.transform.position.y - parent.transform.GetComponent<Collider>().bounds.min.y;
            }

            parent.transform.position = hit.point + Vector3.up * heightDiff;

            if (parent.transform.GetComponent<Collider>() != null && parent.transform.GetComponent<Collider>().bounds.size.y < 0.8f)
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
            DestroyImmediate(parent.transform.gameObject);
        }
    }

    private void RotationPlacementForChildrenRecursiv(GameObject parent, float heightOffset, bool widthPlacement)
    {
        List<Transform> unchangedChildren = parent.transform.Cast<Transform>().ToList();
        foreach (Transform transform in unchangedChildren)
        {
            transform.parent = this.transform;
            _spawnedObjects.Add(transform.gameObject);
            RotationPlacementForChildrenRecursiv(transform.gameObject, heightOffset, widthPlacement);

            RaycastHit hit;
            Physics.Raycast(transform.position, Vector3.down, out hit, 10, _groundMask);

            if (transform.GetComponent<Collider>() != null && widthPlacement && hit.collider != null)
            {
                Vector3 bounds = transform.GetComponent<Collider>().bounds.extents;
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
                    if(rHit.collider != null && rHit.point.y < hit.point.y)
                    {
                        hit.point = new Vector3(hit.point.x, rHit.point.y, hit.point.z);
                    }
                }
            }

            if (hit.collider != null)
            {
                float heightDiff = heightOffset;

                if (transform.GetComponent<Collider>() != null)
                {
                    heightDiff += transform.position.y - transform.GetComponent<Collider>().bounds.min.y;
                }

                transform.position = hit.point + Vector3.up * heightDiff;

                if (transform.GetComponent<Collider>() != null && transform.GetComponent<Collider>().bounds.size.y < 0.8f)
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
            }
        }
    }
}
