using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityToolbox.GameplayFeatures.ProceduralGeneration.Data
{
    public class TerrainGenerationTypeCellularAutomataData: TerrainGenerationTypeBaseData
    {
        public int Size;
        public int IterationCount;
        public float FillPct;
        public int BorderSize;
    }
}
