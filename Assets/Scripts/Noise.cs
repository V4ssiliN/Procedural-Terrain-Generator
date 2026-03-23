using UnityEngine;

public static class Noise
{
    public enum NormalizeMode {Local, Global}
    
    public static float[,] GenerateClassicNoise(int size)
    {
        float[,] noiseMap = new float[size, size];
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                float value = Random.value;
                noiseMap[x, y] = value;
            }
        }

        return noiseMap;
    }
    
    public static float[,] GeneratePerlinNoise(int size, int seed, float scale, int octave, float persistance, float lacunarity, Vector2 offset)
    {
        float[,] noiseMap = new float[size, size];
        System.Random prng = new System.Random(seed);
        
        float amplitude = Mathf.Pow(persistance, octave);
        float frequency = Mathf.Pow(lacunarity, octave);

        for (int i = 0; i < octave; i++)
        {
            prng.Next(-100000, 100000);
            prng.Next(-100000, 100000);
        }

        float offsetX = prng.Next(-100000, 100000) + offset.x;
        float offsetY = prng.Next(-100000, 100000) - offset.y;

        if (scale <= 0)
        {
            scale = 0.0001f;
        }

        float halfSize = size / 2;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {

                float sampleX = ((x - halfSize + offsetX) / scale) * frequency;
                float sampleY = ((y - halfSize + offsetY) / scale) * frequency;

                float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                float noiseHeight = perlinValue * amplitude;

                noiseMap[x, y] = noiseHeight;
            }
        }
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                noiseMap[x, y] = Mathf.InverseLerp(-1, 1, noiseMap[x, y]);
            }
        }

        return noiseMap;

    }

    public static float[,] GenerateNoiseMap(int size, int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 offset, NormalizeMode normalizeMode)
    {
        float[,] noiseMap = new float[size, size];
        System.Random prng = new System.Random(seed);
        Vector2[] octaveOffset = new Vector2[octaves];

        float maxPossibleHeight = 0;
        float amplitude = 1;
        float frequency = 1;

        for (int i = 0; i < octaves; i++)
        {
            float offsetX = prng.Next(-100000, 100000) + offset.x;
            float offsetY = prng.Next(-100000, 100000) - offset.y;
            octaveOffset[i] = new Vector2(offsetX, offsetY);

            maxPossibleHeight += amplitude;
            amplitude *= persistance;
        }

        if (scale  <= 0)
        {
            scale = 0.0001f; 
        }

        float maxLocalNoiseHeight = float.MinValue;
        float minLocalNoiseHeight = float.MaxValue;

        float halfSize = size / 2;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                amplitude = 1;
                frequency = 1;
                float noiseHeight = 0;

                for (int i = 0; i < octaves; i++)
                {
                    float sampleX = ((x - halfSize + octaveOffset[i].x) / scale) * frequency;
                    float sampleY = ((y - halfSize + octaveOffset[i].y) / scale) * frequency;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY)*2 - 1;
                    noiseHeight += perlinValue * amplitude;
                    amplitude *= persistance;
                    frequency *= lacunarity;
                }
                if(noiseHeight > maxLocalNoiseHeight)
                {
                    maxLocalNoiseHeight = noiseHeight;
                }
                else if(noiseHeight < minLocalNoiseHeight)
                {
                    minLocalNoiseHeight = noiseHeight;
                }

                noiseMap[x,y] = noiseHeight;
            }
        }
        if(normalizeMode == NormalizeMode.Local)
        {
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    noiseMap[x, y] = Mathf.InverseLerp(minLocalNoiseHeight, maxLocalNoiseHeight, noiseMap[x, y]);
                }
            }
        }
        else
        {
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float normalizedHeight = (noiseMap[x, y] + 1) / (maxPossibleHeight*2f/1.4f);
                    noiseMap[x, y] = Mathf.Clamp(normalizedHeight, 0, int.MaxValue);
                }
            }
        }
        return noiseMap;
    }
}
