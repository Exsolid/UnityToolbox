using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityToolbox.GameplayFeatures.ProzedualGeneration.Data;
using UnityToolbox.GameplayFeatures.SerializationData;

namespace UnityToolbox.GameplayFeatures.ProzedualGeneration.Editor.GenerationTypes
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
