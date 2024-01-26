using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityToolbox.GameplayFeatures.ProzedualGeneration.Data
{
    public class TerrainGenerationTypeCellularAutomataData: TerrainGenerationTypeBaseData
    {
        public int Size; //TODO generate clusters for bigger lengths (2, 125 max atm)
        public int IterationCount;
        public float FillPct;
        public int BorderSize;
    }
}
