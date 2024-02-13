using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using UnityToolbox.GameplayFeatures.ProceduralGeneration.Data.Layered;
using Quaternion = UnityEngine.Quaternion;
using Random = UnityEngine.Random;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

namespace UnityToolbox.GameplayFeatures.ProceduralGeneration.Terrain.Layered
{
    /// <summary>
    /// The asset placement logic of the layered terrain mesh generator. Only useful within the <see cref="TerrainGeneration"/> class.
    /// </summary>
    public class TerrainGenerationLayeredAssetPlacement
    {
        private TerrainMeshTypeLayeredLayerBaseData[,] _dataGrid;
        private LayerMask _groundLayerMask;
        private MeshFilter[,] _meshes;
        private TerrainMeshTypeLayeredData _meshData;

        /// <summary>
        /// Sets the relevant data for the asset placement.
        /// </summary>
        /// <param name="dataGrid">The created data grid with all layers.</param>
        /// <param name="meshData">The mesh data.</param>
        /// <param name="meshes">The meshes created beforehand.</param>
        /// <param name="groundLayerMask">The ground layer mask for raycast placement.</param>
        public void SetData(TerrainMeshTypeLayeredLayerBaseData[,] dataGrid, TerrainMeshTypeLayeredData meshData, MeshFilter[,] meshes, LayerMask groundLayerMask)
        {
            _dataGrid = dataGrid;
            _groundLayerMask = groundLayerMask;
            _meshes = meshes;
            _meshData = meshData;
        }

        /// <summary>
        /// Places all assets.
        /// </summary>
        public void PlaceAssets()
        {

            int partSize = 50;

            for (int y = 0; y < _dataGrid.GetLength(1); y++)
            {
                for (int x = 0; x < _dataGrid.GetLength(0); x++)
                {

                    MeshFilter currentMesh = _meshes[x / partSize, y / partSize];

                    TerrainMeshTypeLayeredLayerGroundData groundData =
                        _dataGrid[x, y] as TerrainMeshTypeLayeredLayerGroundData;

                    InitAssets(groundData);
                    float rnd = Random.Range(0f, 1f);

                    if (rnd < groundData.PctForNoAsset)
                    {
                        continue;
                    }

                    List<TerrainGenerationLayeredAssetBaseData> toSpawn = groundData.AssetPlacements
                        .OrderBy(obj => obj.OddsForSpawn)
                        .Where(obj => obj.Position.Equals(groundData.AssetPositionType) && obj.OddsForSpawn != 0 && obj.PreIterate).ToList();

                    if (toSpawn.Count == 0)
                    {
                        continue;
                    }

                    rnd = Random.Range(0, toSpawn.Last().OddsForSpawn);
                    TerrainGenerationLayeredAssetBaseData selected = toSpawn.Where(obj => rnd <= obj.OddsForSpawn).OrderBy(obj => obj.OddsForSpawn).FirstOrDefault();

                    Vector3 position = currentMesh.sharedMesh.vertices[x % partSize + (y % partSize) * partSize + 1 + 2 * (y % partSize)];

                    if (selected.GetType() == typeof(TerrainGenerationLayeredAssetData))
                    {
                        TerrainGenerationLayeredAssetData nonClustered = (TerrainGenerationLayeredAssetData) selected;

                        SpawnAsset(nonClustered, position, currentMesh.gameObject);
                    }
                    else
                    {
                       ClusteredSpawnRecursive((TerrainGenerationLayeredAssetClusterData) selected, x, y, new HashSet<Vector2>(), 0);
                    }
                }
            }

            for (int y = 0; y < _dataGrid.GetLength(1); y++)
            {
                for (int x = 0; x < _dataGrid.GetLength(0); x++)
                {

                    MeshFilter currentMesh = _meshes[x / partSize, y / partSize];

                    TerrainMeshTypeLayeredLayerGroundData groundData =
                        _dataGrid[x, y] as TerrainMeshTypeLayeredLayerGroundData;

                    float rnd = Random.Range(0f, 1f);

                    if (rnd < groundData.PctForNoAsset)
                    {
                        continue;
                    }

                    List<TerrainGenerationLayeredAssetBaseData> toSpawn = groundData.AssetPlacements
                        .OrderBy(obj => obj.OddsForSpawn)
                        .Where(obj => obj.Position.Equals(groundData.AssetPositionType) && obj.OddsForSpawn != 0 && !obj.PreIterate).ToList();

                    if (toSpawn.Count == 0)
                    {
                        continue;
                    }

                    rnd = Random.Range(0, toSpawn.Last().OddsForSpawn);
                    TerrainGenerationLayeredAssetBaseData selected = toSpawn.Where(obj => rnd <= obj.OddsForSpawn).OrderBy(obj => obj.OddsForSpawn).FirstOrDefault();

                    Vector3 position = currentMesh.sharedMesh.vertices[x % partSize + (y % partSize) * partSize + 1 + 2 * (y % partSize)];

                    if (selected.GetType() == typeof(TerrainGenerationLayeredAssetData))
                    {
                        TerrainGenerationLayeredAssetData nonClustered = (TerrainGenerationLayeredAssetData)selected;

                        SpawnAsset(nonClustered, position, currentMesh.gameObject);
                    }
                    else
                    {
                        ClusteredSpawnRecursive((TerrainGenerationLayeredAssetClusterData)selected, x, y, new HashSet<Vector2>(), 0);
                    }
                }
            }
        }

        private HashSet<Vector2> ClusteredSpawnRecursive(TerrainGenerationLayeredAssetClusterData data, int currentX, int currentY,
            HashSet<Vector2> checkedPos, float pctForNone)
        {
            if (checkedPos.Contains(new Vector2(currentX, currentY)) || pctForNone >= 1 || currentX >= _dataGrid.GetLength(0) || currentX < 0 || currentY < 0 || currentY >= _dataGrid.GetLength(1))
            {
                return checkedPos;
            }

            int partSize = 50;
            MeshFilter currentMesh = _meshes[currentX / partSize, currentY / partSize];

            TerrainMeshTypeLayeredLayerGroundData groundData =
                _dataGrid[currentX, currentY] as TerrainMeshTypeLayeredLayerGroundData;

            float rnd = Random.Range(0f, 1f);

            if (rnd >= pctForNone)
            {
                List<TerrainGenerationLayeredAssetData> toSpawn = data.Assets.OrderBy(obj => obj.OddsForSpawn).Where(obj => obj.Position.Equals(groundData.AssetPositionType) && obj.OddsForSpawn != 0).ToList();

                if (toSpawn.Count != 0)
                {
                    rnd = Random.Range(0, toSpawn.Last().OddsForSpawn);

                    TerrainGenerationLayeredAssetData selected = toSpawn.Where(obj => rnd <= obj.OddsForSpawn).OrderBy(obj => obj.OddsForSpawn).FirstOrDefault();

                    Vector3 position = currentMesh.sharedMesh.vertices[currentX % partSize + (currentY % partSize) * partSize + 1 + 2 * (currentY % partSize)];

                    SpawnAsset(selected, position, currentMesh.gameObject);
                }
            }

            checkedPos.Add(new Vector2(currentX, currentY));

            checkedPos = ClusteredSpawnRecursive(data, currentX + data.MinVerticesBetweenPrefabs, currentY, checkedPos, pctForNone + data.SpawnDecay);
            checkedPos = ClusteredSpawnRecursive(data, currentX - data.MinVerticesBetweenPrefabs, currentY, checkedPos, pctForNone + data.SpawnDecay);
            checkedPos = ClusteredSpawnRecursive(data, currentX, currentY + data.MinVerticesBetweenPrefabs, checkedPos, pctForNone + data.SpawnDecay);
            checkedPos = ClusteredSpawnRecursive(data, currentX, currentY - data.MinVerticesBetweenPrefabs, checkedPos, pctForNone + data.SpawnDecay);

            return checkedPos;
        }

        private void SpawnAsset(TerrainGenerationLayeredAssetData selected, Vector3 position, GameObject currentMesh)
        {
            float zPos = Random.Range(0, _meshData.AssetPositionNoise * _meshData.SizeBetweenVertices * 2);
            float xPos = Random.Range(0, _meshData.AssetPositionNoise * _meshData.SizeBetweenVertices * 2);

            if (selected.Prefab == null)
            {
                return;
            }


            if (selected.DisableRaycastPlacement)
            {
                GameObject obj = GameObject.Instantiate(selected.Prefab, position + Vector3.up * selected.HeightOffset + Vector3.right * xPos + Vector3.forward * zPos,
                    Quaternion.Euler(0, Random.Range(0, 350), 0));
                TerrainGenerationLayeredAssetDataHolder dataHolder = obj.AddComponent<TerrainGenerationLayeredAssetDataHolder>();
                dataHolder.AssetData = selected;

                obj.transform.parent = currentMesh.transform;
                Physics.SyncTransforms();

                if (obj.GetComponent<Collider>() != null && selected.CanCollide)
                {
                    Collider[] collisions = Physics.OverlapBox(obj.transform.position, obj.GetComponent<Collider>().bounds.extents, obj.transform.rotation);

                    if (collisions.Length > 0)
                    {
                        foreach (Collider col in collisions)
                        {
                            if (!col.gameObject.layer.Equals((int)MathF.Min(31, MathF.Max(0, Mathf.RoundToInt(Mathf.Log(_groundLayerMask.value, 2))))) && !col.gameObject.Equals(obj.gameObject))
                            {
                                if (col.GetComponent<TerrainGenerationLayeredAssetDataHolder>() != null)
                                {
                                    TerrainGenerationLayeredAssetDataHolder dataHolderOther =
                                        col.GetComponent<TerrainGenerationLayeredAssetDataHolder>();
                                    if (dataHolderOther.AssetData.CanCollide)
                                    {
                                        GameObject.DestroyImmediate(obj);
                                        return;
                                    }
                                }
                                else
                                {
                                    GameObject.DestroyImmediate(obj);
                                    return;
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                GameObject obj = GameObject.Instantiate(selected.Prefab, position + Vector3.up * 2 + Vector3.right * xPos + Vector3.forward * zPos,
                    Quaternion.Euler(0, Random.Range(0, 350), 0));

                TerrainGenerationLayeredAssetDataHolder dataHolder = obj.AddComponent<TerrainGenerationLayeredAssetDataHolder>();
                dataHolder.AssetData = selected;

                obj.transform.parent = currentMesh.transform;
                RotationPlacementForChildren(obj, selected.HeightOffset, false, currentMesh, selected.CanCollide);
            }
        }

        private void InitAssets(TerrainMeshTypeLayeredLayerBaseData groundData)
        {
            float currentWeight = 0;

            if (groundData.IsWeighted)
            {
                return;
            }

            foreach (TerrainGenerationLayeredAssetBaseData assetData in groundData.AssetPlacements)
            {
                if (assetData.GetType() == typeof(TerrainGenerationLayeredAssetData))
                {
                    TerrainGenerationLayeredAssetData nonClustered = (TerrainGenerationLayeredAssetData) assetData;
                    nonClustered.Prefab = Resources.Load<GameObject>(nonClustered.PrefabPath);

                    if (nonClustered.Prefab == null)
                    {
                        Debug.LogWarning("A prefab is missing/not set for a layer and cannot be placed. The previous path was: " + nonClustered.PrefabPath);
                    }
                }
                else
                {
                    float currentClusteredWeight = 0;
                    TerrainGenerationLayeredAssetClusterData clustered = (TerrainGenerationLayeredAssetClusterData) assetData;
                    foreach (TerrainGenerationLayeredAssetData asset in clustered.Assets)
                    {
                        asset.Prefab = Resources.Load<GameObject>(asset.PrefabPath);
                        currentClusteredWeight += asset.OddsForSpawn;
                        asset.OddsForSpawn = currentClusteredWeight;

                        if (asset.Prefab == null)
                        {
                            Debug.LogWarning("A prefab is missing/not set for a layer and cannot be placed.The previous path was: " + asset.PrefabPath);
                        }
                    }
                }

                groundData.IsWeighted = true;
                currentWeight += assetData.OddsForSpawn;
                assetData.OddsForSpawn = currentWeight;
            }
        }

        private void RotationPlacementForChildren(GameObject parent, float heightOffset, bool widthPlacement, GameObject currentMeshObject, bool canCollide)
        {
            RotationPlacementForChildrenRecursiv(parent, heightOffset, widthPlacement, currentMeshObject, canCollide);

            RaycastHit hit;
            Physics.Raycast(parent.transform.position, Vector3.down, out hit, 10, _groundLayerMask);

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

                Physics.Raycast(parent.transform.position + new Vector3(bounds.x, 0, bounds.z), Vector3.down, out hit1, 10, _groundLayerMask);
                Physics.Raycast(parent.transform.position + new Vector3(-bounds.x, 0, bounds.z), Vector3.down, out hit2, 10, _groundLayerMask);
                Physics.Raycast(parent.transform.position + new Vector3(bounds.x, 0, -bounds.z), Vector3.down, out hit3, 10, _groundLayerMask);
                Physics.Raycast(parent.transform.position + new Vector3(-bounds.x, 0, -bounds.z), Vector3.down, out hit4, 10, _groundLayerMask);

                Physics.Raycast(parent.transform.position + new Vector3(bounds.x, 0, 0), Vector3.down, out hit5, 10, _groundLayerMask);
                Physics.Raycast(parent.transform.position + new Vector3(0, 0, bounds.z), Vector3.down, out hit6, 10, _groundLayerMask);
                Physics.Raycast(parent.transform.position + new Vector3(0, 0, -bounds.z), Vector3.down, out hit7, 10, _groundLayerMask);
                Physics.Raycast(parent.transform.position + new Vector3(-bounds.x, 0, 0), Vector3.down, out hit8, 10, _groundLayerMask);

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
                    parent.transform.parent = currentMeshObject.transform;
                    GameObject.DestroyImmediate(pivot);
                }

                Physics.SyncTransforms();

                if (parent.GetComponent<Collider>() != null && canCollide)
                {
                    Collider[] collisions = Physics.OverlapBox(parent.transform.position, parent.GetComponent<Collider>().bounds.extents, parent.transform.rotation);

                    if (collisions.Length > 0)
                    {
                        foreach (Collider col in collisions)
                        {
                            if (!col.gameObject.layer.Equals((int)MathF.Min(31, MathF.Max(0, Mathf.RoundToInt(Mathf.Log(_groundLayerMask.value, 2))))) && !col.gameObject.Equals(parent.gameObject))
                            {
                                if (col.GetComponent<TerrainGenerationLayeredAssetDataHolder>() != null)
                                {
                                    TerrainGenerationLayeredAssetDataHolder dataHolder =
                                        col.GetComponent<TerrainGenerationLayeredAssetDataHolder>();
                                    if (dataHolder.AssetData.CanCollide)
                                    {
                                        GameObject.DestroyImmediate(parent);
                                        return;
                                    }
                                }
                                else
                                {
                                    GameObject.DestroyImmediate(parent);
                                    return;
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                GameObject.DestroyImmediate(parent.transform.gameObject);
            }
        }

        private void RotationPlacementForChildrenRecursiv(GameObject parent, float heightOffset, bool widthPlacement, GameObject currentMeshObject, bool canCollide)
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
                    RotationPlacementForChildrenRecursiv(transform.gameObject, heightOffset, widthPlacement, currentMeshObject, canCollide);
                    transform.parent = currentMeshObject.transform;
                }
                else
                {
                    continue;
                }

                RaycastHit hit;
                Physics.Raycast(transform.position, Vector3.down, out hit, 10, _groundLayerMask);

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

                    Physics.Raycast(transform.position + new Vector3(bounds.x, 0, bounds.z), Vector3.down, out hit1, 10, _groundLayerMask);
                    Physics.Raycast(transform.position + new Vector3(-bounds.x, 0, bounds.z), Vector3.down, out hit2, 10, _groundLayerMask);
                    Physics.Raycast(transform.position + new Vector3(bounds.x, 0, -bounds.z), Vector3.down, out hit3, 10, _groundLayerMask);
                    Physics.Raycast(transform.position + new Vector3(-bounds.x, 0, -bounds.z), Vector3.down, out hit4, 10, _groundLayerMask);

                    Physics.Raycast(transform.position + new Vector3(bounds.x, 0, 0), Vector3.down, out hit5, 10, _groundLayerMask);
                    Physics.Raycast(transform.position + new Vector3(0, 0, bounds.z), Vector3.down, out hit6, 10, _groundLayerMask);
                    Physics.Raycast(transform.position + new Vector3(0, 0, -bounds.z), Vector3.down, out hit7, 10, _groundLayerMask);
                    Physics.Raycast(transform.position + new Vector3(-bounds.x, 0, 0), Vector3.down, out hit8, 10, _groundLayerMask);

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
                        transform.parent = currentMeshObject.transform;
                        GameObject.DestroyImmediate(pivot);
                    }

                    Physics.SyncTransforms();

                    if (transform.GetComponent<Collider>() != null && canCollide)
                    {
                        Collider[] collisions = Physics.OverlapBox(transform.position, transform.GetComponent<Collider>().bounds.extents, transform.transform.rotation);

                        if (collisions.Length > 0)
                        {
                            foreach (Collider col in collisions)
                            {
                                if (!col.gameObject.layer.Equals((int)MathF.Min(31, MathF.Max(0, Mathf.RoundToInt(Mathf.Log(_groundLayerMask.value, 2))))))
                                {
                                    if (col.GetComponent<TerrainGenerationLayeredAssetDataHolder>() != null)
                                    {
                                        TerrainGenerationLayeredAssetDataHolder dataHolder =
                                            col.GetComponent<TerrainGenerationLayeredAssetDataHolder>();
                                        if (dataHolder.AssetData.CanCollide)
                                        {
                                            GameObject.DestroyImmediate(parent);
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        GameObject.DestroyImmediate(parent);
                                        return;
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    GameObject.DestroyImmediate(transform.gameObject);
                    continue;
                }
            }
        }
    }
}
