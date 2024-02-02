using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityToolbox.GameplayFeatures.ProceduralGeneration.Data;
using UnityToolbox.GameplayFeatures.SerializationData;

namespace UnityToolbox.GameplayFeatures.ProceduralGeneration.Editor.GenerationTypes
{
    public abstract class TerrainGenerationType: ISerializedDataContainer<TerrainGenerationTypeBaseData>
    {
        protected TerrainGenerationTypeBaseData _data;

        public abstract void DrawDetails();

        public abstract int[,] GetExampleGeneration(int x, int y);

        public abstract SerializedDataErrorDetails Deserialize(TerrainGenerationTypeBaseData obj);
        public abstract TerrainGenerationTypeBaseData Serialize();
    }
}
