using System;
using UnityToolbox.GameplayFeatures.ProceduralGeneration.Enums;

namespace UnityToolbox.General.Algorithms
{
    /// <summary>
    /// An implementation of the cellular automata algotrithm.
    /// </summary>
    public class CellularAutomata
    {
        /// <summary>
        /// Fills an array using the given information.
        /// </summary>
        /// <param name="gridToFill">The grid which should be filled and then returned. Defines the size of the generation.</param>
        /// <param name="fillPercentage">How much should be filled initially? Used to generate an initial noise.</param>
        /// <param name="iterationCount">How often the algorithm should iterate. Higher values mean smoother results but take longer.</param>
        /// <param name="borderSize">How big should the border be? The border will always be filled to be the value of <see cref="gridToFill"/>.</param>
        /// <returns>An array the size of <paramref name="gridToFill"/>, filled by the algorithm.</returns>
        /// <exception cref="TerrainValues"></exception>
        public static int[,] Generate(int[,] gridToFill, float fillPercentage, int iterationCount, int borderSize)
        {
            if(fillPercentage > 1 || fillPercentage < 0)
            {
                throw new ArgumentException(nameof(fillPercentage) + "only allows values between 0-1.");
            }

            Random rand = new Random();
            for (int y = 0; y < gridToFill.GetLength(1); y++)
            {
                for (int x = 0; x < gridToFill.GetLength(0); x++)
                {
                    gridToFill[x, y] = rand.NextDouble() < fillPercentage ? (int)TerrainValues.Wall : (int)TerrainValues.Floor;
                }
            }

            int[,] newGrid = (int[,]) gridToFill.Clone();
            for (int i = 0; i < iterationCount; i++)
            {
                for (int y = 0; y < newGrid.GetLength(1); y++)
                {
                    for (int x = 0; x < newGrid.GetLength(0); x++)
                    {
                        int wallCount = 0;
                        if (x < borderSize || y < borderSize || x > gridToFill.GetLength(0) - borderSize - 1 || y > gridToFill.GetLength(1) - borderSize - 1)
                        {
                            newGrid[x, y] = (int)TerrainValues.Wall;
                            continue;
                        }

                        for (int yN = -1; yN <= 1; yN++)
                        {
                            for (int xN = -1; xN <= 1; xN++)
                            {
                                if (yN + y >= 0 && yN + y < gridToFill.GetLength(1) && xN + x >= 0 && xN + x < gridToFill.GetLength(0) && !(x == 0 && y == 0))
                                {
                                    if (gridToFill[x + xN, y + yN].Equals((int)TerrainValues.Wall))
                                    {
                                        wallCount++;
                                    }
                                }
                            }
                        }

                        if (wallCount > 4)
                        {
                            newGrid[x, y] = (int)TerrainValues.Wall;
                        }
                        else
                        {
                            newGrid[x, y] = (int)TerrainValues.Floor;
                        }
                    }
                }
                gridToFill = (int[,]) newGrid.Clone();
            }

            return gridToFill;
        }

        public static int[,] GenerateForValue(int[,] gridToFill, float fillPercentage, int iterationCount, int borderSize, int valueToReplace, int valueToPlace1, int valueToPlace2)
        {
            if (fillPercentage > 1 || fillPercentage < 0)
            {
                throw new ArgumentException(nameof(fillPercentage) + "only allows values between 0-1.");
            }

            Random rand = new Random();
            for (int y = 0; y < gridToFill.GetLength(1); y++)
            {
                for (int x = 0; x < gridToFill.GetLength(0); x++)
                {
                    if (gridToFill[x, y] == valueToReplace)
                    {
                        gridToFill[x, y] = rand.NextDouble() < fillPercentage ? valueToPlace2 : valueToPlace1;
                    }
                }
            }

            int[,] newGrid = (int[,])gridToFill.Clone();
            for (int i = 0; i < iterationCount; i++)
            {
                for (int y = 0; y < newGrid.GetLength(1); y++)
                {
                    for (int x = 0; x < newGrid.GetLength(0); x++)
                    {
                        if (gridToFill[x, y] != valueToPlace1 && gridToFill[x, y] != valueToPlace2)
                        {
                            continue;
                        }

                        int wallCount = 0;
                        if (x < borderSize || y < borderSize || x > gridToFill.GetLength(0) - borderSize - 1 || y > gridToFill.GetLength(1) - borderSize - 1)
                        {
                            newGrid[x, y] = valueToPlace1;
                            continue;
                        }

                        for (int yN = -1; yN <= 1; yN++)
                        {
                            for (int xN = -1; xN <= 1; xN++)
                            {
                                if (yN + y >= 0 && yN + y < gridToFill.GetLength(1) && xN + x >= 0 && xN + x < gridToFill.GetLength(0) && !(x == 0 && y == 0))
                                {
                                    if (gridToFill[x + xN, y + yN].Equals(valueToPlace1))
                                    {
                                        wallCount++;
                                    }
                                }
                            }
                        }

                        if (wallCount > 4)
                        {
                            newGrid[x, y] = valueToPlace1;
                        }
                        else
                        {
                            newGrid[x, y] = valueToPlace2;
                        }
                    }
                }
                gridToFill = (int[,])newGrid.Clone();
            }

            return gridToFill;
        }
    }
}
