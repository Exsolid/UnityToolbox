using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityToolbox.GameplayFeatures.ProceduralGeneration.Data;
using UnityToolbox.GameplayFeatures.ProceduralGeneration.Enums;
using UnityToolbox.General.Algorithms;
using Object = UnityEngine.Object;

namespace UnityToolbox.GameplayFeatures.ProceduralGeneration.Terrain.Layered
{
    public class TerrainGenerationLayered : TerrainGenerationGenerator
    {
        private GameObject _terrainObject;
        private TerrainGenerationData _data;
        private TerrainMeshTypeLayeredData _meshData;
        private int[,] _grid;
        private TerrainMeshTypeLayeredLayerBaseData[,] _dataGrid;
        private TerrainGenerationLayeredAssetPlacement _assetPlacement;
        private MeshFilter[,] _meshes;
        private LayerMask _groundLayerMask;

        public override void SetData(TerrainGenerationData data, GameObject terrainObject, LayerMask grounLayerMask)
        {
            _dataGrid = null;
            _grid = null;
            _data = data;
            _meshData = _data.MeshData as TerrainMeshTypeLayeredData;
            _terrainObject = terrainObject;
            _assetPlacement = new TerrainGenerationLayeredAssetPlacement();
            _groundLayerMask = grounLayerMask;
        }

        public override void Generate()
        {
            GetInitialGrid();
            GenerateLayersForGrid();
            FillGridWithFillerValuesAndData();
            FillGridWithNoise();
            GenerateMesh();
            _assetPlacement.SetAssets(_dataGrid, _meshData, _meshes, _groundLayerMask);
        }

        private void FillGridWithNoise()
        {
            for (int x = 0; x < _dataGrid.GetLength(0); x++)
            {
                for (int y = 0; y < _dataGrid.GetLength(1); y++)
                {
                    TerrainMeshTypeLayeredLayerGroundData groundData =
                        _dataGrid[x, y] as TerrainMeshTypeLayeredLayerGroundData;
                    groundData.Height -= Mathf.PerlinNoise(x * 0.3f, y * 0.3f) * groundData.NoiseGround;
                    _dataGrid[x, y] = groundData;
                }
            }
        }

        private void GenerateLayersForGrid()
        {
            int baseCount = 0;
            int currentWallLayer = (int)TerrainValues.Wall;
            int currentLayerBelow = (int)TerrainValues.Floor;
            foreach (TerrainMeshTypeLayeredLayerBaseData layerData in _meshData.Layers)
            {
                if (layerData.GetType() == typeof(TerrainMeshTypeLayeredLayerGroundData))
                {
                    if (baseCount == 0 || baseCount == 1)
                    {
                        baseCount++;
                        continue;
                    }

                    int newWallLayer = int.Parse(currentWallLayer + "" + (int) TerrainValues.Wall);
                    ReplaceContainedValues(currentWallLayer, newWallLayer, currentLayerBelow);
                    if (_data.GenerationData.GetType() == typeof(TerrainGenerationTypeCellularAutomataData))
                    {
                        TerrainGenerationTypeCellularAutomataData caData = _data.GenerationData as TerrainGenerationTypeCellularAutomataData;
                        //_grid = CellularAutomata.GenerateForValue(_grid, caData.FillPct,
                        //    caData.IterationCount, caData.BorderSize, -2, newWallLayer, -1);
                    }

                    for (int y = 0; y < _grid.GetLength(1); y++)
                    {
                        for (int x = 0; x < _grid.GetLength(0); x++)
                        {
                            if (_grid[x, y] == -1)
                            {
                                _grid[x, y] = currentWallLayer;
                            }
                        }
                    }

                    currentLayerBelow = currentWallLayer;
                    currentWallLayer = newWallLayer;
                }
            }
        }

        private void GetInitialGrid()
        {
            _grid = null;
            if (_data.GenerationData.GetType() == typeof(TerrainGenerationTypeCellularAutomataData))
            {
                TerrainGenerationTypeCellularAutomataData caData = _data.GenerationData as TerrainGenerationTypeCellularAutomataData;
                _grid = CellularAutomata.Generate(new int[caData.Size, caData.Size], caData.FillPct, caData.IterationCount, caData.BorderSize);
            }
        }

        private void ReplaceContainedValues(int valueToReplace, int valueToPlace, int floorValue)
        {
            for (int y = 0; y < _grid.GetLength(1); y++)
            {
                for (int x = 0; x < _grid.GetLength(0); x++)
                {
                    if (_grid[x, y] != valueToReplace)
                    {
                        continue;
                    }

                    bool replace = true;
                    for (int yN = -1; yN <= 1; yN++)
                    {
                        for (int xN = -1; xN <= 1; xN++)
                        {
                            if (yN + y >= 0 && yN + y < _grid.GetLength(1) && xN + x >= 0 && xN + x < _grid.GetLength(0) && !(x == 0 && y == 0))
                            {
                                if (_grid[x + xN, y + yN].Equals(floorValue))
                                {
                                    replace = false;
                                }
                            }
                        }
                    }

                    if (replace)
                    {
                        _grid[x, y] = valueToPlace;
                    }
                }
            }
        }

        private void FillGridWithFillerValuesAndData()
        {
            int size = _grid.GetLength(0);
            int fillerCount = _meshData.VertexMultiplier;
            _dataGrid = new TerrainMeshTypeLayeredLayerBaseData[size * fillerCount, size * fillerCount];

            TerrainMeshTypeLayeredLayerGroundData prev = null;
            TerrainMeshTypeLayeredLayerGroundData next = null;
            for (int y = 0; y < _dataGrid.GetLength(1); y++)
            {
                for (int x = 0; x < _dataGrid.GetLength(0); x++)
                {
                    if (x % fillerCount == 0 && y % fillerCount == 0)
                    {
                        //Set non filler data
                        _dataGrid[x, y] = GetLayeredData((int)MathF.Floor(x / (float)fillerCount), (int)MathF.Floor(y / (float)fillerCount)).Clone() as TerrainMeshTypeLayeredLayerGroundData;
                        prev = (TerrainMeshTypeLayeredLayerGroundData) _dataGrid[x, y];
                        if (MathF.Ceiling((x + 1) / (float) fillerCount) < _grid.GetLength(0))
                        {
                            next = GetLayeredData((int) MathF.Ceiling((x + 1) / (float)fillerCount), (int) MathF.Floor(y / (float)fillerCount));
                        }
                    }
                    else if (x % fillerCount != 0 && y % fillerCount == 0)
                    {
                        //Set filler data for x
                        _dataGrid[x, y] = GetFillerData(prev, next, x);
                    }
                    else
                    {
                        prev = null;
                        next = null;
                    }
                }
            }

            for (int x = 0; x < _dataGrid.GetLength(0); x++)
            {
                for (int y = 0; y < _dataGrid.GetLength(1); y++)
                {
                    if (y % fillerCount == 0)
                    {
                        prev = (TerrainMeshTypeLayeredLayerGroundData)_dataGrid[x, y];
                        if (y + fillerCount < _dataGrid.GetLength(1))
                        {
                            next = (TerrainMeshTypeLayeredLayerGroundData)_dataGrid[x, y + fillerCount];
                        }
                    }
                    else
                    {
                        _dataGrid[x, y] = GetFillerData(prev, next, y);
                    }
                }
            }
        }
        
        private TerrainMeshTypeLayeredLayerGroundData GetFillerData(TerrainMeshTypeLayeredLayerGroundData prev, TerrainMeshTypeLayeredLayerGroundData next, int pos)
        {
            int fillerCount = _meshData.VertexMultiplier;
            TerrainMeshTypeLayeredLayerGroundData filledData = new TerrainMeshTypeLayeredLayerGroundData();
            //TODO height data
            if (next == null)
            {
                filledData.Height = prev.Height;
                filledData.NoiseGround = prev.NoiseGround;
                filledData.AssetPlacements.AddRange(prev.AssetPlacements);
                filledData.AssetPositionType = prev.AssetPositionType;
                filledData.PctForNoAsset = prev.PctForNoAsset;
            }
            else
            {
                float posPct = pos % fillerCount / (float) fillerCount;

                if ((prev.Height - next.Height) > 0)
                {
                    posPct = 1 - posPct;
                }

                float range = MathF.Abs(prev.Height - next.Height);
                float min = MathF.Min(prev.Height, next.Height);

                filledData.Height = min + range * posPct;

                range = MathF.Abs(prev.NoiseGround - next.NoiseGround);
                min = MathF.Min(prev.NoiseGround, next.NoiseGround);

                filledData.NoiseGround = min + range * posPct;

                range = MathF.Abs(prev.PctForNoAsset - next.PctForNoAsset);
                min = MathF.Min(prev.PctForNoAsset, next.PctForNoAsset);

                filledData.PctForNoAsset = min + range * posPct;

                filledData.AssetPlacements = new List<TerrainGenerationLayeredAssetBaseData>();

                foreach (TerrainGenerationLayeredAssetBaseData asset in prev.AssetPlacements)
                {
                    filledData.AssetPlacements.Add((TerrainGenerationLayeredAssetBaseData) asset.Clone());
                }

                foreach (TerrainGenerationLayeredAssetBaseData asset in next.AssetPlacements)
                {
                    filledData.AssetPlacements.Add((TerrainGenerationLayeredAssetBaseData)asset.Clone());
                }

                if (prev.AssetPositionType == next.AssetPositionType)
                {
                    filledData.AssetPositionType = prev.AssetPositionType;
                }
                else
                {
                    filledData.AssetPositionType = TerrainGenerationAssetPosition.Cliff;
                }
            }
            return filledData;
        }

        private TerrainMeshTypeLayeredLayerGroundData GetLayeredData(int x, int y)
        {
            if (_grid[x, y] == (int)TerrainValues.Floor)
            {
                TerrainMeshTypeLayeredLayerGroundData layer = _meshData.Layers[0] as TerrainMeshTypeLayeredLayerGroundData;
                layer.AssetPositionType = TerrainGenerationAssetPosition.Ground;
                return layer;
            }
            else if (_grid[x, y] == (int) TerrainValues.Wall)
            {
                if (_meshData.Layers.Count == 1)
                {
                    TerrainMeshTypeLayeredLayerGroundData layer = _meshData.Layers[0] as TerrainMeshTypeLayeredLayerGroundData;
                    layer.AssetPositionType = TerrainGenerationAssetPosition.Cliff;
                    return _meshData.Layers[0] as TerrainMeshTypeLayeredLayerGroundData;
                }
                else
                {
                    TerrainMeshTypeLayeredLayerGroundData layer = _meshData.Layers[1] as TerrainMeshTypeLayeredLayerGroundData;
                    layer.AssetPositionType = TerrainGenerationAssetPosition.Cliff;
                    return _meshData.Layers[1] as TerrainMeshTypeLayeredLayerGroundData;
                }
            }
            else
            {
                TerrainMeshTypeLayeredLayerGroundData layer = _meshData.Layers.ElementAt(_grid[x, y].ToString().Length) as
                    TerrainMeshTypeLayeredLayerGroundData;

                bool cliff = false;

                for (int yN = -1; yN <= 1; yN++)
                {
                    for (int xN = -1; xN <= 1; xN++)
                    {
                        if (yN + y >= 0 && yN + y < _grid.GetLength(1) && xN + x >= 0 && xN + x < _grid.GetLength(0) && !(x == 0 && y == 0))
                        {
                            if (_grid[x + xN, y + yN].ToString() != _grid[x, y].ToString())
                            {
                                cliff = true;
                            }
                        }
                    }
                }

                if (cliff)
                {
                    layer.AssetPositionType = TerrainGenerationAssetPosition.Cliff;
                }
                else
                {
                    layer.AssetPositionType = TerrainGenerationAssetPosition.Ground;
                }

                return layer;
            }
        }

        private void GenerateMesh()
        {
            Transform prev = _terrainObject.transform.Find("Terrain Data");
            while (prev != null)
            {
                Object.DestroyImmediate(prev.gameObject);
                prev = _terrainObject.transform.Find("Terrain Data");
            } 

            int partSize = 50;
            int partsX = (int) MathF.Ceiling(_dataGrid.GetLength(0) / (float) partSize);
            int partsY = (int) MathF.Ceiling(_dataGrid.GetLength(1) / (float) partSize);

            _meshes = new MeshFilter[partsX, partsY];
            GameObject parent = new GameObject("Terrain Data");
            parent.transform.parent = _terrainObject.transform;

            Vector3 objPos = new Vector3();
            for (int currentPartY = 0; currentPartY < partsY; currentPartY++)
            {
                for (int currentPartX = 0; currentPartX < partsX; currentPartX++)
                {
                    int dataGridYStart = (int) partSize * currentPartY;
                    int dataGridXStart = (int) partSize * currentPartX;
                    int dataGridYEnd = (int) MathF.Min(_dataGrid.GetLength(1) - 1,partSize * currentPartY + partSize - 1);
                    int dataGridXEnd = (int) MathF.Min(_dataGrid.GetLength(0) - 1, (partSize * currentPartX + partSize) - 1);

                    GameObject obj = new GameObject
                    {
                        name = currentPartY + " " + currentPartX,
                        transform =
                        {
                            localPosition = objPos,
                            parent = parent.transform
                        }
                    };
                    MeshFilter meshFilter = obj.AddComponent<MeshFilter>();
                    MeshRenderer meshRenderer = obj.AddComponent<MeshRenderer>();

                    MeshCollider meshCollider = obj.AddComponent<MeshCollider>();
                    Vector3[] vertices = new Vector3[(int) (partSize * partSize + (partSize + 2) * 4 - 4)]; //Size = length * width + additional border for equal vertices
                    List<int> triangles = new List<int>();

                    Vector2[] uvs = new Vector2[(int)(partSize * partSize + (partSize + 2) * 4 - 4)]; //Size = length * width + additional border for equal vertices

                    //Generate vertecies
                    for (int y = dataGridYStart - 1; y <= dataGridYEnd + 1; y++)
                    {
                        for (int x = dataGridXStart - 1; x <= dataGridXEnd + 1; x++)
                        {
                            int dataGridX = (x >= dataGridXStart && x <= dataGridXEnd ? x : -1);
                            int dataGridY = (y >= dataGridYStart && y <= dataGridYEnd ? y : -1);
                            Vector3 pos = new Vector3();

                            if (dataGridY == -1 || dataGridX == -1)
                            {
                                pos = GetBorderVector(x, y, dataGridXStart - 1, dataGridXEnd + 1, dataGridYStart - 1, dataGridYEnd + 1);
                            }
                            else
                            {
                                TerrainMeshTypeLayeredLayerGroundData groundData = _dataGrid[x, y] as TerrainMeshTypeLayeredLayerGroundData;
                                pos = new Vector3(x * _meshData.SizeBetweenVertices, groundData.Height, y * _meshData.SizeBetweenVertices);
                            }

                            vertices[(x - dataGridXStart + 1) + (y - dataGridYStart + 1) * (partSize + 2)] = pos;
                        }
                    }

                    //Generate triangles in both ways
                    for (int y = dataGridYStart - 1; y <= dataGridYEnd + 1; y++)
                    {
                        for (int x = dataGridXStart - 1; x <= dataGridXEnd + 1; x++)
                        {
                            if (x + 1 <= dataGridXEnd + 1 && y + 1 <= dataGridYEnd + 1)
                            {
                                triangles.Add((x - dataGridXStart + 1) + (y - dataGridYStart + 1) * (partSize + 2));
                                triangles.Add((x - dataGridXStart + 1) + (y - dataGridYStart + 2) * (partSize + 2));
                                triangles.Add((x - dataGridXStart + 2) + (y - dataGridYStart + 1) * (partSize + 2));
                            }
                        }
                    }

                    for (int y = dataGridYStart - 1; y <= dataGridYEnd + 1; y++)
                    {
                        for (int x = dataGridXStart - 1; x <= dataGridXEnd + 1; x++)
                        {
                            if (x - 1 >= dataGridXStart - 1 && y - 1 >= dataGridYStart - 1)
                            {
                                triangles.Add((x - dataGridXStart + 1) + (y - dataGridYStart + 1) * (partSize + 2));
                                triangles.Add((x - dataGridXStart + 1) + (y - dataGridYStart + 0) * (partSize + 2));
                                triangles.Add((x - dataGridXStart + 0) + (y - dataGridYStart + 1) * (partSize + 2));
                            }
                        }
                    }

                    for (int y = dataGridYStart - 1; y <= dataGridYEnd + 1; y++)
                    {
                        for (int x = dataGridXStart - 1; x <= dataGridXEnd + 1; x++)
                        {
                            uvs[(x - dataGridXStart + 1) + (y - dataGridYStart + 1) * (partSize + 2)] = new Vector2((float) (x + 1 - dataGridXStart) / partSize, (float)(y + 1 - dataGridYStart) / partSize);
                        }
                    }

                    Mesh mesh = new Mesh();

                    mesh.name = "TerrainMesh";
                    mesh.vertices = vertices;
                    mesh.triangles = triangles.ToArray();
                    mesh.uv = uvs;
                    mesh.RecalculateNormals();
                    meshFilter.sharedMesh = mesh;
                    meshCollider.sharedMesh = mesh;
                    meshRenderer.material = Mat;
                    meshRenderer.sharedMaterial = Mat;

                    obj.layer = Mathf.RoundToInt(Mathf.Log(_groundLayerMask.value, 2));

                    _meshes[currentPartX, currentPartY] = meshFilter;
                }
            }
        }

        private Vector3 GetBorderVector(int x, int y, int dataGridXStart, int dataGridXEnd, int dataGridYStart, int dataGridYEnd)
        {
            Vector3 pos = new Vector3(x * _meshData.SizeBetweenVertices, 0, y * _meshData.SizeBetweenVertices);
            HashSet<TerrainMeshTypeLayeredLayerGroundData> dataToAvg = new HashSet<TerrainMeshTypeLayeredLayerGroundData>();

            //Base
            if (x > 0 && y > 0 && x < _dataGrid.GetLength(0) && y < _dataGrid.GetLength(1))
            {
                dataToAvg.Add(_dataGrid[x, y] as TerrainMeshTypeLayeredLayerGroundData);
            }

            //Right
            if (x == dataGridXStart && y >= 0 && y < _dataGrid.GetLength(1))
            {
                dataToAvg.Add(_dataGrid[x + 1, y] as TerrainMeshTypeLayeredLayerGroundData);
                pos.x += _meshData.SizeBetweenVertices / 2f;
            }

            //Down
            if (y == dataGridYStart && x >= 0 && x < _dataGrid.GetLength(0))
            {
                dataToAvg.Add(_dataGrid[x, y + 1] as TerrainMeshTypeLayeredLayerGroundData);
                pos.z += _meshData.SizeBetweenVertices /  2f;
            }

            //Right + Down
            if (y < _dataGrid.GetLength(1) && y == dataGridYStart && x < _dataGrid.GetLength(0) && x == dataGridXStart)
            {
                dataToAvg.Add(_dataGrid[x + 1, y + 1] as TerrainMeshTypeLayeredLayerGroundData);
            }

            //Left
            if (x > 0 && x == dataGridXEnd && y >= 0 && y < _dataGrid.GetLength(1))
            {
                dataToAvg.Add(_dataGrid[x - 1, y] as TerrainMeshTypeLayeredLayerGroundData);
                pos.x -= _meshData.SizeBetweenVertices / 2f;
            }

            //Up
            if (y > 0 && y == dataGridYEnd && x >= 0 && x < _dataGrid.GetLength(0))
            {
                dataToAvg.Add(_dataGrid[x, y - 1] as TerrainMeshTypeLayeredLayerGroundData);
                pos.z -= _meshData.SizeBetweenVertices / 2f;
            }

            //Left + Up
            if (y > 0 && y == dataGridYEnd && x > 0 && x == dataGridXEnd)
            {
                dataToAvg.Add(_dataGrid[x - 1, y - 1] as TerrainMeshTypeLayeredLayerGroundData);
            }

            //Right + Up
            if (y == dataGridYEnd && x == dataGridXStart)
            {
                dataToAvg.Add(_dataGrid[x + 1, y - 1] as TerrainMeshTypeLayeredLayerGroundData);
            }

            //Left + Down
            if (y == dataGridYStart && x == dataGridXEnd)
            {
                dataToAvg.Add(_dataGrid[x - 1, y + 1] as TerrainMeshTypeLayeredLayerGroundData);
            }

            foreach (TerrainMeshTypeLayeredLayerGroundData avg in dataToAvg)
            {
                pos.y += avg.Height;
            }

            pos.y /= dataToAvg.Count;

            return pos;
        }

        public override List<TerrainGenerationHeightColorData> CalculateHeights()
        {
            List<TerrainGenerationHeightColorData> allHeightData = new List<TerrainGenerationHeightColorData>();
            float currentHeight = 0;
            float maxHeight = _meshData.Layers.Sum(x => (x as TerrainMeshTypeLayeredLayerGroundData).Height);
            foreach (TerrainMeshTypeLayeredLayerBaseData layerData in _meshData.Layers)
            {
                if (layerData.GetType() == typeof(TerrainMeshTypeLayeredLayerGroundData))
                {
                    TerrainMeshTypeLayeredLayerGroundData ground = (TerrainMeshTypeLayeredLayerGroundData)layerData;

                    if (ground.HeightData == null)
                    {
                        Debug.LogWarning("The height data for layer " + layerData.Name + " is not defined.");
                        continue;
                    }

                    for (var index = 0; index < ground.HeightData.Count; index++)
                    {
                        TerrainGenerationHeightColorData heightLayer = ground.HeightData[index];
                        heightLayer.StartingHeightPCT = (heightLayer.StartingHeightPCT * ground.Height + currentHeight) / maxHeight;
                        allHeightData.Add(heightLayer);

                        if (heightLayer.TerrainTexturePath is null or "")
                        {
                            Debug.LogWarning("The a height data layer for layer " + layerData.Name + " is not defined.");
                        }
                    }
                    currentHeight += ground.Height;

                    ground.Height = currentHeight;
                }
            }

            return allHeightData;
        }

        public override float GetHighestHeight()
        {
            TerrainMeshTypeLayeredLayerGroundData
                last = (_meshData.Layers.Last() as TerrainMeshTypeLayeredLayerGroundData);
            return last.Height;
        }
    }
}
