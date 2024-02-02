using System.Collections;
using UnityEngine;

namespace UnityToolbox.GameplayFeatures.ProceduralGeneration.Terrain
{
    /// <summary>
    /// A helping script which references all terrain generation algorithms as buttons.
    /// </summary>
    public class TerrainGenerateAll : MonoBehaviour
    {
        /// <summary>
        /// Generates the terrain and all decoration excluding anchors.
        /// </summary>
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

        /// <summary>
        /// Generates the terrain and all decoration.
        /// </summary>
        public void GenerateAllWithAnchors()
        {
            TerrainGenerator meshGen = FindObjectOfType<TerrainGenerator>();
            if (meshGen != null)
            {
                meshGen.GenerateWithAnchorPoints();

                StartCoroutine(WaitNextFrame());
            }
        }

        /// <summary>
        /// Deletes all genereated objects.
        /// </summary>
        public void DeleteAllDecoration()
        {
            TerrainDecoration[] decoGens = FindObjectsOfType<TerrainDecoration>();
            foreach (TerrainDecoration gen in decoGens)
            {
                gen.DeleteObjects();
            }
        }

        private IEnumerator WaitNextFrame()
        {
            yield return new WaitForSeconds(0.1f);

            TerrainDecoration[] decoGens = FindObjectsOfType<TerrainDecoration>();
            foreach (TerrainDecoration gen in decoGens)
            {
                gen.PlaceObjects();
            }
        }
    }
}
