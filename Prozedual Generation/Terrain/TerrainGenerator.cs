using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

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
    [SerializeField] [Range(2, 125)]private int _length; //TODO generate clusters for bigger lengths
    [SerializeField] private int _iterationCount;
    [SerializeField] [Range(0f,1f)] private float _fillPct;

    [Header("Mesh Settings")]
    [SerializeField] private float _height;
    [SerializeField] private float _noiseHeightTop;
    [SerializeField] private float _noiseHeightBottom;

    [Header("Material Settings")]
    [SerializeField] private Material _material;
    [SerializeField] private bool _autoUpdate;
    [SerializeField] private TextureSizes _textureSize;
    [SerializeField] private TextureFormat _textureFormat;
    [SerializeField] private List<HeightColorTypes> _heightDefinitions;

    private Texture2DArray texture2DArray; //Reference needed so garbage collection doesnt destroy shader texture

    public void GenerateViaCellularAutomata()
    {
        _heightDefinitions = _heightDefinitions.OrderBy(x => x.HeightStart).ToList();

        int[,] cAGrid = CellularAutomata.Generate(new int[_length, _length], _fillPct, _iterationCount);
        FillGridWithFillerValues(cAGrid);
        GenerateMesh();
        UpdateMaterial();
    }

    private void OnValidate()
    {
        if (_autoUpdate)
        {
            UpdateMaterial();
        }
    }

    private Texture2DArray GenerateTextureArray(Texture2D[] textures)
    {
        texture2DArray = new Texture2DArray((int)_textureSize, (int)_textureSize, textures.Length, _textureFormat, true);
        for (int i = 0; i < textures.Length; i++)
        {
            texture2DArray.SetPixels(textures[i].GetPixels(), i);
        }
        texture2DArray.Apply();
        return texture2DArray;
    }

    private void GenerateMesh()
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
                Vector3 pos = new Vector3(x, (_grid[x, y] == (int)TerrainValues.Wall ? _height - Mathf.PerlinNoise(x * 0.3f, y * 0.3f) * _noiseHeightTop : -Mathf.PerlinNoise(x * 0.3f, y * 0.3f) * _noiseHeightBottom), y);
                if (_grid[x, y] == 2)
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
                                    all = all + new Vector3(x + xN, (_grid[x + xN, y + yN] == (int)TerrainValues.Wall ? _height - Mathf.PerlinNoise(x * 0.3f, y * 0.3f) * _noiseHeightTop : -Mathf.PerlinNoise(x * 0.3f, y * 0.3f) * _noiseHeightBottom), y + yN);
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

    private void FillGridWithFillerValues(int[,] grid)
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

    private void UpdateMaterial()
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
}
