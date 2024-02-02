using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using UnityEngine;
using UnityToolbox.GameplayFeatures.ProceduralGeneration.Data;
using UnityToolbox.General.Management;

namespace UnityToolbox.GameplayFeatures.ProceduralGeneration.Terrain
{
    public class TerrainGeneration : Module
    {
        [SerializeField] private GameObject _terrainObject;
        public Dictionary<string, TerrainGenerationData> Data;
        [SerializeField][HideInInspector] public int SelectedData;

        public void GenerateTerrain()
        {
            Data = TerrainGenerationIO.Instance.ReadData();
            if (Data.Count == 0)
            {
                throw new DataException("Cannot generate terrain without data.");
            }

            if (SelectedData >= Data.Count)
            {
                throw new DataException("The selected data index exceeds the data count.");
            }

            try
            {
                TerrainGenerationGenerator generator = null;
                TerrainGenerationData dataToUse = Data.Values.ElementAt(SelectedData);

                if (dataToUse.MeshData.GetType() == typeof(TerrainMeshTypeLayeredData))
                {
                    generator = new TerrainGenerationLayered();
                }

                generator.Generate(dataToUse, _terrainObject);
            }
            catch (Exception ex)
            {
                throw new Exception("The terrain generation has encountered an error. Is the data still valid?", ex);
            }
        }
    }
}
