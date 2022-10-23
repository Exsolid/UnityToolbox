using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerateAll : MonoBehaviour
{
    public void GenerateAll()
    {
        TerrainGenerator meshGen = FindObjectOfType<TerrainGenerator>();
        if(meshGen != null)
        {
            meshGen.GenerateViaCellularAutomata();

           TerrainDecoration[] decoGens = FindObjectsOfType<TerrainDecoration>();
            foreach(TerrainDecoration gen in decoGens)
            {
                gen.PlaceObjects();
            }
        }
    }

    public void DeleteAllDecoration()
    {
        TerrainDecoration[] decoGens = FindObjectsOfType<TerrainDecoration>();
        foreach (TerrainDecoration gen in decoGens)
        {
            gen.DeleteObjects();
        }
    }
}
