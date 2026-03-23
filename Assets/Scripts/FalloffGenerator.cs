using UnityEngine;

public static class FalloffGenerator
{
    
    public static float[,] GenerateFalloffMap(int size)
    {
        float[,] map = new float[size, size];

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                float x = i / (float)size * 2 - 1;
                float y = j / (float)size * 2 - 1;

                map[i, j] = Circle(x, y);
            }
        }
        return map;
    }

    static float Circle(float x, float y)
    {
        float value = Mathf.Sqrt(x*x + y*y);
        value = Mathf.Clamp(value, 0, 0.9f);
        value = Mathf.InverseLerp(0, 0.9f, value);
        value = Mathf.Pow(value, 3);
        return value;
    }
}
