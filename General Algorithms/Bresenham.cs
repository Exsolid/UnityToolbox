namespace Bresenham_Algorithm
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Some bresenham algorithms.
    /// </summary>
    public static class Bresenham
    {
        /// <summary>
        /// Returns all points between <paramref name="pointOne"/> and <paramref name="pointTwo"/> that make up a line.
        /// </summary>
        /// <param name="pointOne">The first coordinate.</param>
        /// <param name="pointTwo">The second coordinate.</param>
        /// <returns>A list of coordinates</returns>
        public static List<int[]> RunLine(int[] pointOne, int[] pointTwo)
        {
            if(pointOne.Length != 2 || pointTwo.Length != 2)
            {
                throw new ArgumentException("The coordinate arrays given do not have the length of 2!");
            }

            float[] diff = new float[] { Math.Abs(pointOne[0] - pointTwo[0]), Math.Abs(pointOne[1] - pointTwo[1]) };
            float[] step = new float[] { Math.Sign(pointOne[0] - pointTwo[0]) * -1, Math.Sign(pointOne[1] - pointTwo[1]) * -1 };
            float errorStepOne = 2 * diff[0];
            float errorStepTwo = 2 * diff[1];

            List<int[]> allPoints = new List<int[]>();
            int[] current = new int[] { pointOne[0], pointOne[1] };
            allPoints.Add(new int[] { pointOne[0], pointOne[1] });
            float error = 0;
            if (diff[1] <= diff[0])
            {
                error = -diff[0];
                while (current[0] != pointTwo[0])
                {
                    error += errorStepTwo;
                    if (error > 0)
                    {
                        current[1] += (int)step[1];
                        error -= errorStepOne;
                    }
                    current[0] += (int)step[0];
                    allPoints.Add(new int[] { current[0], current[1] });
                }
            }
            else
            {
                error = -diff[1];
                while (current[1] != pointTwo[1])
                {
                    error += errorStepOne;
                    if (error > 0)
                    {
                        current[0] += (int)step[0];
                        error -= errorStepTwo;
                    }
                    current[1] += (int)step[1];
                    allPoints.Add(new int[] { current[0], current[1] });
                }
            }
            return allPoints;
        }

        /// <summary>
        /// Returns all points between the first coordinate <paramref name="pointOneX"/>, <paramref name="pointOneY"/> and the second <paramref name="pointTwoX"/>, <paramref name="pointTwoY"/> that make up a line.
        /// </summary>
        /// <param name="pointOneX"></param>
        /// <param name="pointOneY"></param>
        /// <param name="pointTwoX"></param>
        /// <param name="pointTwoY"></param>
        /// <returns>A list of coordinates</returns>
        public static List<int[]> RunLine(int pointOneX, int pointOneY, int pointTwoX, int pointTwoY)
        {
            return RunLine(new int[] { pointOneX, pointOneY }, new int[] { pointTwoX, pointTwoY });
        }

        /// <summary>
        /// Returns all coordinates that make up a circle from a given coordinate <paramref name="pointOne"/> and the <paramref name="radius"/>.
        /// </summary>
        /// <param name="pointOne">The point of where to construct the circle.</param>
        /// <param name="radius">The radius of the circle</param>
        /// <returns>A list of coordinates</returns>
        public static List<int[]> RunCircle(int[] pointOne, int radius)
        {
            List<int[]> allOctacePoints = new List<int[]>();
            int[] current = new int[]{0, radius };
            allOctacePoints.Add(current);
            int decisionParam = 3-2 * radius;
            while(current[0] <= current[1])
            {
                if (decisionParam < 0)
                {
                    decisionParam += 4 * current[0] + 6;
                    current = new int[] { current[0]+1, current[1] };
                    allOctacePoints.Add(current);
                }
                else
                {
                    decisionParam += 4 * (current[0]-current[1]) + 10;
                    current = new int[] { current[0]+1, current[1]-1};
                    allOctacePoints.Add(current);
                }
            }

            List<int[]> allPoints = new List<int[]>();
            foreach (int[] point in allOctacePoints)
            {
                current = new int[] { pointOne[0] + point[0], pointOne[1] + point[1] };
                allPoints.Add(current);
                current = new int[] { pointOne[0] - point[0], pointOne[1] + point[1] };
                allPoints.Add(current);
                current = new int[] { pointOne[0] + point[0], pointOne[1] - point[1] };
                allPoints.Add(current);
                current = new int[] { pointOne[0] - point[0], pointOne[1] - point[1] };
                allPoints.Add(current);

                current = new int[] { pointOne[0] + point[1], pointOne[1] + point[0] };
                allPoints.Add(current);
                current = new int[] { pointOne[0] - point[1], pointOne[1] + point[0] };
                allPoints.Add(current);
                current = new int[] { pointOne[0] + point[1], pointOne[1] - point[0] };
                allPoints.Add(current);
                current = new int[] { pointOne[0] - point[1], pointOne[1] - point[0] };
                allPoints.Add(current);
            }
            return allPoints;
        }
    }
}