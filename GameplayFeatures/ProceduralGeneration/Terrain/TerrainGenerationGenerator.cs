using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityToolbox.GameplayFeatures.ProceduralGeneration.Data;
using UnityToolbox.GameplayFeatures.ProceduralGeneration.Enums;
using UnityToolbox.General.Algorithms;

namespace UnityToolbox.GameplayFeatures.ProceduralGeneration.Terrain
{
    /// <summary>
    /// The basis for all mesh generators.
    /// </summary>
    public abstract class TerrainGenerationGenerator
    {
        /// <summary>
        /// The material used for the generation. Must be set before <see cref="TerrainGenerationGenerator.Generate"/>
        /// </summary>
        public Material Mat;


        /// <summary>
        /// Sets all relevant data for the generation.
        /// </summary>
        /// <param name="data">The terrain generation data to use. This is created within the Terrain Generation Tool Window.</param>
        /// <param name="terrainObject">The parent object for the generated terrain.</param>
        /// <param name="groundLayerMask">The ground mask. Required for the asset placement.</param>
        public abstract void SetData(TerrainGenerationData data, GameObject terrainObject, LayerMask groundLayerMask);


        /// <summary>
        /// Generates the terrain with grid, mesh and assets.
        /// </summary>
        public abstract void Generate();

        /// <summary>
        /// Required for the height color data.
        /// </summary>
        /// <returns>The last height of the terrain. </returns>
        public abstract float GetHighestHeight();

        /// <summary>
        /// Required for the height color data.
        /// </summary>
        /// <returns>The starting height of the terrain. </returns>
        public abstract float GetLowestHeight();

        /// <summary>
        /// A method which calculates all set <see cref="TerrainGenerationHeightColorData"/> and prepares their defined heights to be usable.
        /// </summary>
        /// <returns></returns>
        public abstract List<TerrainGenerationHeightColorData> CalculateHeights();
    }
}
