using System.Collections.Generic;
using UnityEngine;

public static class PlayabilityScore
{
    public static float[,] CalculateSlopes(float[,] heightmap)
    {
        int size = heightmap.GetLength(0);
        float[,] slopes = new float[size,size];

        float diffA;
        float diffB;
        float diffC;
        float diffD;

        for (int x = 1; x < size-1; x++)
        {
            diffA = Mathf.Abs(heightmap[x, 0] - heightmap[x - 1, 0]); //bord haut du tableau
            diffB = Mathf.Abs(heightmap[x, 0] - heightmap[x + 1, 0]);
            diffC = Mathf.Abs(heightmap[x, 0] - heightmap[x, 1]);
            slopes[x,0] = Mathf.Max(diffA, diffB, diffC);

            for (int y = 1; y < size-1; y++)
            {
                diffA = Mathf.Abs(heightmap[x, y] - heightmap[x - 1, y]); //intérieur du tableau
                diffB = Mathf.Abs(heightmap[x, y] - heightmap[x + 1, y]);
                diffC = Mathf.Abs(heightmap[x, y] - heightmap[x, y + 1]);
                diffD = Mathf.Abs(heightmap[x, y] - heightmap[x, y - 1]);
                slopes[x,y] = Mathf.Max(diffA, diffB, diffC, diffD);
            }

            diffA = Mathf.Abs(heightmap[x, size - 1] - heightmap[x - 1, size - 1]); //bord bas du tableau
            diffB = Mathf.Abs(heightmap[x, size - 1] - heightmap[x + 1, size - 1]);
            diffC = Mathf.Abs(heightmap[x, size - 1] - heightmap[x, size - 2]);
            slopes[x, size - 1] = Mathf.Max(diffA, diffB, diffC);
        }

        for (int y = 1; y < size-1; y++)
        {
            diffA = Mathf.Abs(heightmap[0, y] - heightmap[1, y]); //bord gauche du tableau
            diffB = Mathf.Abs(heightmap[0, y] - heightmap[0, y + 1]);
            diffC = Mathf.Abs(heightmap[0, y] - heightmap[0, y - 1]);
            slopes[0,y] = Mathf.Max(diffA, diffB, diffC);

            diffA = Mathf.Abs(heightmap[size - 1, y] - heightmap[size - 2, y]); //bord droit du tableau
            diffB = Mathf.Abs(heightmap[size - 1, y] - heightmap[size - 1, y + 1]);
            diffC = Mathf.Abs(heightmap[size - 1, y] - heightmap[size - 1, y - 1]);
            slopes[size - 1, y] = Mathf.Max(diffA, diffB, diffC);
        }

        diffA = Mathf.Abs(heightmap[0, 0] - heightmap[1, 0]); //coin supérieur gauche du tableau
        diffB = Mathf.Abs(heightmap[0, 0] - heightmap[0, 1]);
        slopes[0,0] = Mathf.Max(diffA, diffB);

        diffA = Mathf.Abs(heightmap[0, size - 1] - heightmap[1, size - 1]);//coin supérieur droit du tableau
        diffB = Mathf.Abs(heightmap[0, size - 1] - heightmap[0, size - 2]);
        slopes[0, size - 1] = Mathf.Max(diffA, diffB);

        diffA = Mathf.Abs(heightmap[size - 1, 0] - heightmap[size - 2, 0]);//coin inférieur gauche du tableau
        diffB = Mathf.Abs(heightmap[size - 1, 0] - heightmap[size - 1, 1]);
        slopes[size - 1, 0] = Mathf.Max(diffA, diffB);

        diffA = Mathf.Abs(heightmap[size - 1, size - 1] - heightmap[size - 2, size - 1]);//coin inférieur droit du tableau
        diffB = Mathf.Abs(heightmap[size - 1, size - 1] - heightmap[size - 1, size - 2]);
        slopes[size - 1, size - 1] = Mathf.Max(diffA, diffB);
        
        return slopes;
    }

    public static int[,] CalculateAccessibilityMap(float[,] heightmap, float tu)
    {
        float[,] slopes = CalculateSlopes(heightmap);

        int size = slopes.GetLength(0);
        int[,] A = new int[size, size];
        
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                if (slopes[x,y] < tu)
                {
                    A[x, y] = 1;
                }
                else
                {
                    A[x, y] = 0;
                }
            }
        }
        
        return A;
    }
    
    public static int[,] CalculateUnitMap(float[,] heightmap, float tu)
    {
        int[,] U = CalculateAccessibilityMap(heightmap, tu);
        int size = U.GetLength(0);
        
        int maxPixelsInOneArea = 0;
        int largestAreaIndex = 0;
        int areaIndex = 2;

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                if(U[x,y] == 1)
                {
                    int count = Explore(U, areaIndex, x, y);
                    if (count > maxPixelsInOneArea)
                    {
                        largestAreaIndex = areaIndex;
                        maxPixelsInOneArea = count;
                    }
                    areaIndex++;
                }
            }
        }

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                if (U[x,y] == largestAreaIndex)
                {
                    U[x,y] = 1;
                }
                else
                {
                    U[x,y] = 0;
                }
            }
        }
        
        return U;
    }

    public static int[,] CalculateFlatnessMap(float[,] heightmap, float tb, int nb)
    {
        int[,] tmp = CalculateAccessibilityMap(heightmap, tb);
        int size = tmp.GetLength(0);
        int[,] F = new int[size, size];

        for (int x = 0; x < size - nb +1; x++)
        {
            for (int y = 0; y < size - nb +1; y++)
            {
                if(tmp[x,y] == 1)
                {
                    bool test = true;
                    
                    for (int i = 0; i < nb; i++)
                    {
                        for (int j = 0; j < nb; j++)
                        {
                            if(tmp[x+i,y+j] == 0)
                            {
                                test = false;
                                break;
                            }
                        }
                    }
                    if (test)
                    {
                        for (int i = 0; i < nb; i++)
                        {
                            for (int j = 0; j < nb; j++)
                            {
                                F[x + i, y + j] = 1;
                            }
                        }
                    }
                }
            }
        }
        return F;
    }

    public static int[,] CalculateBuildingMap(float[,] heightmap, int[,] U, float tb, int nb)
    {
        int[,] F = CalculateFlatnessMap(heightmap, tb, nb);

        int size = U.GetLength(0);
        int[,] B = new int[size, size];

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                B[x, y] = U[x, y] * F[x, y];
            }
        }
        return B;
    }

    public static int[,] CalculateExtremaMap(float[,] heightmap)
    {
        int size = heightmap.GetLength(0);
        int[,] E = new int[size, size];

        for (int x = 1; x < size-1; x++)
        {
            for (int y = 1; y < size-1; y++)
            {
                bool isMaximum = heightmap[x - 1, y] < heightmap[x, y] &&
                                 heightmap[x + 1, y] < heightmap[x, y] &&
                                 heightmap[x, y - 1] < heightmap[x, y] &&
                                 heightmap[x, y + 1] < heightmap[x, y];

                bool isMinimum = heightmap[x - 1, y] > heightmap[x, y] &&
                                 heightmap[x + 1, y] > heightmap[x, y] &&
                                 heightmap[x, y - 1] > heightmap[x, y] &&
                                 heightmap[x, y + 1] > heightmap[x, y];

                bool isExtremum = isMaximum || isMinimum;

                if (isExtremum)
                {
                    E[x,y] = 1;
                }
                else
                {
                    E[x,y] = 0;
                }
            }
        }
        for (int i = 0; i < size; i++)
        {
            heightmap[i,0] = 0;
            heightmap[i, size - 1] = 0;
            heightmap[0, i] = 0;
            heightmap[size - 1, i] = 0;
        }

        return E;
    }

    public static float CalculateUnitScore(int[,] U)
    {
        float unitScore = ArrayMeanValue(U);
        return unitScore;
    }

    public static float CalculateBuildingScore(float[,] heightmap, int[,] U, float tb, int nb)
    {
        int[,] B = CalculateBuildingMap(heightmap, U, tb, nb);
        float buildingScore = ArrayMeanValue(B);
        return buildingScore;
    }

    public static float CalculateErosionScore(float[,] heightmap)
    {
        int size = heightmap.GetLength(0);
        float[,] slopes = CalculateSlopes(heightmap);

        float slopeMean = 0f;

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                slopeMean += slopes[x, y];
            }
        }
        slopeMean = slopeMean / (size * size);

        float sD = 0;

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                sD += (slopes[x, y] - slopeMean) * (slopes[x, y] - slopeMean);
            }
        }
        sD = Mathf.Sqrt(sD / (size * size));

        float erosionScore = sD / slopeMean;
        return erosionScore;
    }

    public static float CalculateDetailScore(float[,] heightmap)
    {
        int[,] E = CalculateExtremaMap(heightmap);
        float detailScore = ArrayMeanValue(E);

        return detailScore;
    }

    public static float[] CalculatePlayabilityScore(float[,] heightmap, float tu, float tb, int nb)
    {
        int[,] U = CalculateUnitMap(heightmap, tu);
        
        float unitScore = CalculateUnitScore(U);
        float buildingScore = CalculateBuildingScore(heightmap, U, tb, nb);
        float erosionScore = CalculateErosionScore(heightmap);
        float detailScore = CalculateDetailScore(heightmap);

        float playabilityScore = Mathf.Pow(unitScore * buildingScore * erosionScore * detailScore, 1/3f);
        float[] rep = { playabilityScore, unitScore, buildingScore, erosionScore, detailScore };


        return rep;
    }

    public static float ArrayMeanValue(int[,] a)
    {
        int size = a.GetLength(0);
        float sum = 0;

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                sum += a[x, y];
            }
        }
        return sum / (size * size);
    }


    public static int Explore(int[,] a, int i, int x0, int y0)
    {
        int size = a.GetLength(0);
        int count = 1;
        a[x0, y0] = i;

        Queue<(int,int)> q = new Queue<(int,int)> ();
        q.Enqueue((x0, y0));

        while(q.Count > 0)
        {
            (int, int) v = q.Dequeue();
            int x = v.Item1;
            int y = v.Item2;

            if(x != 0 && a[x-1, y] == 1)
            {
                a[x-1, y] = i;
                count++;
                q.Enqueue((x-1, y));
            }
            if (x != size-1 && a[x+1, y] == 1)
            {
                a[x+1, y] = i;
                count++;
                q.Enqueue((x+1, y));
            }
            if (y != 0 && a[x, y-1] == 1)
            {
                a[x, y-1] = i;
                count++;
                q.Enqueue((x, y-1));
            }
            if (y != size-1 && a[x, y+1] == 1)
            {
                a[x, y+1] = i;
                count++;
                q.Enqueue((x, y+1));
            }
        }
        return count;
    }

    public static float[,] ArrayIntToFloat(int[,] ints)
    {
        int size = ints.GetLength(0);
        float[,] floats = new float[size, size];

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                floats[x, y] = ints[x, y];
            }
        }

        return floats;
    }

    public static float[,] NormalizeSlopes(float[,] slopes)
    {
        int size = slopes.GetLength(0);
        float[,] nSlopes = new float[size, size];

        float maxSlope = 0f;
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                if (slopes[x, y] > maxSlope)
                {
                    maxSlope = slopes[x, y];
                }
            }
        }

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                nSlopes[x,y] = Mathf.InverseLerp(0, maxSlope, slopes[x,y]);
            }
        }

        return nSlopes;
    }
}
