using System.Collections;
using System.Collections.Generic;
using System;

public class CellularAutomata
{
    public static int[,] Generate(int[,] gridToFill, float fillPercentage, int iterationCount, int edgeSize)
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
                if(x < edgeSize || y < edgeSize || x > gridToFill.GetLength(0) - edgeSize - 1 || y > gridToFill.GetLength(1) - edgeSize - 1)
                {
                    gridToFill[y, x] = (int) TerrainValues.Wall;
                }
                else
                {
                    gridToFill[y, x] = rand.NextDouble() < fillPercentage ? (int)TerrainValues.Wall : (int)TerrainValues.Floor;
                }
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
}
