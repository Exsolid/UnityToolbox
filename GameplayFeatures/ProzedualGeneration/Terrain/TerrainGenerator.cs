using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityToolbox.GameplayFeatures.ProzedualGeneration.Enums;
using UnityToolbox.General.Algorithms;

namespace UnityToolbox.GameplayFeatures.ProzedualGeneration.Terrain
{
    /// <summary>
    /// The main component of the terrain generation. It can generate one mesh, with defined settings using <see cref="CellularAutomata"/>.
    /// </summary>
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshCollider))]
    public class TerrainGenerator : MonoBehaviour
    {
        private int[,] _grid;
        private Mesh _mesh;

        public int[,] GeneratedGrid { get { return _grid; } }
        public Mesh GeneratedMesh { get { return _mesh; } }

        [Header("Generation Settings")]
        [SerializeField] [Range(2, 125)] private int _length; //TODO generate clusters for bigger lengths
        [SerializeField] private int _iterationCount;
        [SerializeField] [Range(0f,1f)] private float _fillPct;
        [SerializeField] private int _edgeSize;

        [Header("Mesh Settings")]
        [SerializeField] private float _height;
        [SerializeField] private float _noiseHeightTop;
        [SerializeField] private float _noiseHeightBottom;
        [SerializeField] private float _sizeBetweenNodes;
    
        [Header("Material Settings")]
        [SerializeField] private Material _material;
        [SerializeField] private bool _autoUpdate;
        [SerializeField] private TextureSizes _textureSize;
        [SerializeField] private TextureFormat _textureFormat;
        [SerializeField] private List<HeightColorTypes> _heightDefinitions;

        [Header("Anchor Settings")]
        [SerializeField] private List<GameObject> _toPlace;
        [SerializeField] private LayerMask _groundMask;
        [SerializeField] [HideInInspector] private List<GameObject> _spawnedObjects;

        private List<PlacementInformation> _toPlacePositions;

        private Texture2DArray texture2DArray; //Reference needed so garbage collection doesnt destroy shader texture

        /// <summary>
        /// Starts the generation with cellular automata.
        /// </summary>
        public void GenerateViaCellularAutomata()
        {
            _heightDefinitions = _heightDefinitions.OrderBy(x => x.HeightStart).ToList();

            int[,] cAGrid = CellularAutomata.Generate(new int[_length, _length], _fillPct, _iterationCount, _edgeSize);
            FillGridWithFillerValues(cAGrid);
            GenerateMesh();
            UpdateMaterial();
        }

        /// <summary>
        /// Starts the generation with anchor points. These are carved out in the terrain.
        /// </summary>
        public void GenerateWithAnchorPoints()
        {
            _toPlacePositions = new List<PlacementInformation>();
            _heightDefinitions = _heightDefinitions.OrderBy(x => x.HeightStart).ToList();

            int[,] cAGrid = CellularAutomata.Generate(new int[_length, _length], _fillPct, _iterationCount, _edgeSize);


            GenerateAnchorSpace(cAGrid);
            FillGridWithFillerValues(cAGrid);

            GenerateMesh();

            PlaceObjects();

            UpdateMaterial();
        }

        private void OnValidate()
        {
            if(_sizeBetweenNodes == 0)
            {
                _sizeBetweenNodes = 0.1f;
            }else if (_sizeBetweenNodes < 0)
            {
                _sizeBetweenNodes = Mathf.Abs(_sizeBetweenNodes);
            }

            if (_autoUpdate)
            {
                UpdateMaterial();
            }
        }

        protected Texture2DArray GenerateTextureArray(Texture2D[] textures)
        {
            texture2DArray = new Texture2DArray((int)_textureSize, (int)_textureSize, textures.Length, _textureFormat, true);
            for (int i = 0; i < textures.Length; i++)
            {
                texture2DArray.SetPixels(textures[i].GetPixels(), i);
            }
            texture2DArray.Apply();
            return texture2DArray;
        }

        protected void GenerateMesh()
        {
            MeshFilter meshFilter = GetComponent<MeshFilter>();

            MeshCollider meshCollider = GetComponent<MeshCollider>();

            Vector3[] vertices = new Vector3[_length * 4 * _length];

            List<int> triangles = new List<int>();

            Vector2[] uvs = new Vector2[_length * 4 * _length];

            //Generate vertecies
            for (int y = 0; y < _grid.GetLength(1); y++)
            {
                for (int x = 0; x < _grid.GetLength(0); x++)
                {
                    Vector3 pos = new Vector3(x * _sizeBetweenNodes, (_grid[x, y] == (int)TerrainValues.Wall ? _height - Mathf.PerlinNoise(x * 0.3f, y * 0.3f) * _noiseHeightTop : -Mathf.PerlinNoise(x * 0.3f, y * 0.3f) * _noiseHeightBottom), y * _sizeBetweenNodes);
                    if (_grid[x, y] == (int)TerrainValues.Filler)
                    {
                        Vector3 all = new Vector3();
                        float counter = 0;
                        for (int yN = -1; yN <= 1; yN++)
                        {
                            for (int xN = -1; xN <= 1; xN++)
                            {
                                if (yN + y >= 0 && yN + y < _grid.GetLength(1) && xN + x >= 0 && xN + x < _grid.GetLength(0) && _grid[x + xN, y + yN] != (int)TerrainValues.Filler && !(x == 0 && y == 0))
                                {
                                    counter++;
                                    if (vertices[_length * 2 * (y + yN) + x + xN] != Vector3.zero)
                                    {
                                        all = all + vertices[_length * 2 * (y + yN) + x + xN];
                                    }
                                    else
                                    {
                                        all = all + new Vector3((x + xN) * _sizeBetweenNodes, (_grid[x + xN, y + yN] == (int)TerrainValues.Wall ? _height - Mathf.PerlinNoise(x * 0.3f, y * 0.3f) * _noiseHeightTop : -Mathf.PerlinNoise(x * 0.3f, y * 0.3f) * _noiseHeightBottom), (y + yN) * _sizeBetweenNodes);
                                    }
                                }
                            }
                        }

                        pos = all / counter;
                    }

                    vertices[_length * 2 * y + x] = pos;
                }
            }

            //Generate triangles in both ways
            for (int y = 0; y < _grid.GetLength(1); y++)
            {
                for (int x = 0; x < _grid.GetLength(0); x++)
                {
                    if (x + 1 < _grid.GetLength(0) && y + 1 < _grid.GetLength(1))
                    {
                        triangles.Add(_length * 2 * y + x);
                        triangles.Add(_length * 2 * (y + 1) + x);
                        triangles.Add(_length * 2 * y + (x + 1));
                    }
                }
            }

            for (int y = 0; y < _grid.GetLength(1); y++)
            {
                for (int x = 0; x < _grid.GetLength(0); x++)
                {
                    if (x - 1 >= 0 && y - 1 >= 0)
                    {
                        triangles.Add(_length * 2 * y + x);
                        triangles.Add(_length * 2 * (y - 1) + x);
                        triangles.Add(_length * 2 * y + (x - 1));
                    }
                }
            }

            for (int y = 0; y < _grid.GetLength(1); y++)
            {
                for (int x = 0; x < _grid.GetLength(0); x++)
                {
                    uvs[_length * 2 * y + x] = new Vector2((float)x / _length * 2, (float)y / _length * 2);
                }
            }

            _mesh = new Mesh();
            _mesh.name = "TerrainMesh";
            _mesh.vertices = vertices;
            _mesh.triangles = triangles.ToArray();
            _mesh.uv = uvs;
            _mesh.RecalculateNormals();
            meshFilter.sharedMesh = _mesh;
            meshCollider.sharedMesh = _mesh;
        }

        protected void FillGridWithFillerValues(int[,] grid)
        {
            _grid = new int[_length * 2, _length * 2];
            for (int y = 0; y < _grid.GetLength(1); y++)
            {
                for (int x = 0; x < _grid.GetLength(0); x++)
                {
                    if (x % 2 == 0 && y % 2 == 0)
                    {
                        _grid[x, y] = grid[x / 2, y / 2];
                    }
                    else
                    {
                        _grid[x, y] = (int)TerrainValues.Filler;
                    }
                }
            }
        }

        protected void UpdateMaterial()
        {
            if (!_heightDefinitions.Any())
            {
                return;
            }

            _heightDefinitions = _heightDefinitions.OrderBy(x => x.HeightStart).ToList();

            _material.SetFloat("minHeight", -_noiseHeightBottom);
            _material.SetFloat("maxHeight", _height);

            Color[] colors = new Color[_heightDefinitions.Count];
            float[] colStrengths = new float[_heightDefinitions.Count];
            float[] textureScales = new float[_heightDefinitions.Count];
            float[] heights = new float[_heightDefinitions.Count];
            float[] blends = new float[_heightDefinitions.Count];
            Texture2D[] textures = new Texture2D[_heightDefinitions.Count];
            int count = 0;
            foreach (HeightColorTypes type in _heightDefinitions)
            {
                colors[count] = type.TerrainColor;
                colStrengths[count] = type.TerrainColorStrength;

                if (type.TerrainTextureScale == 0)
                {
                    type.TerrainTextureScale = 1;
                }

                textureScales[count] = type.TerrainTextureScale;

                heights[count] = type.HeightStart;
                blends[count] = type.BlendAmount;

                if(type.TerrainTexture == null)
                {
                    return;
                }

                textures[count] = type.TerrainTexture;

                count++;
            }

            _material.SetInt("layerCount", _heightDefinitions.Count);

            _material.SetColorArray("baseColors", colors);
            _material.SetFloatArray("baseColorStrengths", colStrengths);

            _material.SetFloatArray("baseStartHeights", heights);
            _material.SetFloatArray("baseBlends", blends);

            _material.SetFloatArray("baseTextureScales", textureScales);
            _material.SetTexture("baseTextures", GenerateTextureArray(textures));
        }

        private void GenerateAnchorSpace(int[,] cAGrid)
        {
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

            Vector2 lastPos = new Vector2(-1, -1);
            foreach (GameObject obj in _toPlace)
            {
                Bounds bounds = new Bounds(obj.transform.position, Vector3.zero);

                foreach (Renderer r in obj.GetComponentsInChildren<Renderer>())
                {
                    bounds.Encapsulate(r.bounds);
                }

                if (bounds.size.x >= cAGrid.GetLength(0) / 2 * _sizeBetweenNodes)
                {
                    Debug.LogWarning("Structure " + obj.name + "is too big for the generated map, it will be skipped.");
                    continue;
                }

                Vector2 randomPos = GetNewPosition(bounds, cAGrid);

                for (int y = (int)Mathf.Floor(randomPos.y - bounds.extents.z); y < randomPos.y + bounds.extents.z; y++)
                {
                    for (int x = (int)Mathf.Floor(randomPos.x - bounds.extents.x / 2 - 2); x < randomPos.x + bounds.extents.x / 2 + 2; x++)
                    {
                        cAGrid[x, y] = (int)TerrainValues.Floor;
                    }
                }

                PlacementInformation info = new PlacementInformation();
                Vector3 diff = bounds.center - obj.transform.position;
                info.Position = randomPos;
                info.WholeBounds = bounds;
                info.ToSpawn = obj;
                _toPlacePositions.Add(info);

                if (lastPos.x != -1)
                {
                    List<Vector2> curve = Bezier(lastPos, randomPos);
                    Vector2 lastPoint = lastPos;
                    curve.Add(randomPos);
                    foreach (Vector2 point in curve)
                    {
                        List<int[]> connectionPoints = Bresenham.RunLine((int)lastPoint.x, (int)lastPoint.y, (int)point.x, (int)point.y);

                        foreach (int[] connectionPoint in connectionPoints)
                        {
                            for (int y = connectionPoint[1] - 2; y < connectionPoint[1] + 2; y++)
                            {
                                for (int x = connectionPoint[0] - 2; x < connectionPoint[0] + 2; x++)
                                {
                                    if (x < 0 || y < 0 || x >= cAGrid.GetLength(0) || y >= cAGrid.GetLength(1))
                                    {
                                        continue;
                                    }
                                    cAGrid[x, y] = (int)TerrainValues.Floor;
                                }
                            }
                        }
                        lastPoint = point;
                    }
                }

                lastPos = randomPos;
            }
        }

        private Vector2 GetNewPosition(Bounds bounds, int[,] cAGrid)
        {
            Vector2 randomPos = new Vector2();

            bool foundPos = false;
            while (!foundPos)
            {
                randomPos = new Vector2(UnityEngine.Random.Range(2 + bounds.extents.x, cAGrid.GetLength(0) - 3 - bounds.extents.x), UnityEngine.Random.Range(2 + bounds.extents.z, cAGrid.GetLength(1) - 3 - bounds.extents.z));
                foundPos = true;
                foreach (PlacementInformation otherPos in _toPlacePositions)
                {
                    if ((otherPos.WholeBounds.extents.x + bounds.extents.x) * 1.5f > Vector2.Distance(otherPos.Position, randomPos))
                    {
                        foundPos = false;
                        break;
                    }
                }
            }

            return randomPos;
        }

        private void PlaceObjects()
        {
            foreach (PlacementInformation info in _toPlacePositions)
            {
                GameObject instance = Instantiate(info.ToSpawn, new Vector3(info.Position.x * 2 * _sizeBetweenNodes, transform.position.y + _height + _noiseHeightTop * 2, info.Position.y * 2 * _sizeBetweenNodes), Quaternion.identity);

                _spawnedObjects.Add(instance);
                instance.transform.parent = transform;

                if (instance.GetComponent<TerrainDecorationChildInfo>() == null || !instance.GetComponent<TerrainDecorationChildInfo>().RelativeToParent)
                {
                    RotationPlacementForChildren(instance, 0, true);
                }

            }
        }

        private List<Vector2> Bezier(Vector2 p1, Vector2 p2)
        {
            Vector2 p3 = new Vector2((p1.x + p2.x) / 2 - Vector2.Distance(p1, p2) / 3, (p1.y + p2.y) / 2 - Vector2.Distance(p1, p2) / 3);
            Vector2 p4 = new Vector2((p1.x + p2.x) / 2 + Vector2.Distance(p1, p2) / 3, (p1.y + p2.y) / 2 + Vector2.Distance(p1, p2) / 3);

            List<Vector2> curve = new List<Vector2>();

            for (float i = 0; i <= 1; i += 0.1f)
            {
                curve.Add(Mathf.Pow(1 - i, 3) * p1 + 3 * Mathf.Pow((1 - i), 2) * i * p3 + 3 * (1 - i) * Mathf.Pow(i, 2) * p4 + Mathf.Pow(i, 3) * p2);
            }

            return curve;
        }

        private struct PlacementInformation
        {
            public GameObject ToSpawn;
            public Bounds WholeBounds;
            public Vector2 Position;
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
                }
            }
        }
    }
}
