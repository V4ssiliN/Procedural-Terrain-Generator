using UnityEngine;
using System;

public class MapGenerator : MonoBehaviour
{
    public int mapChunkSize;
    public enum DrawMode { NoiseMap, ColorMap, Mesh, FalloffMap, PerlinNoise, LCurve, PCurve, PLCurve, Slopes, A, U, F, B, E, ClassicNoise}
    public DrawMode drawMode;

    public Noise.NormalizeMode normalizeMode;
    
    public float noiseScale;
    [Range(1,8)]
    public int octaves;
    [Range(0,1)]
    public float persistance;
    public float lacunarity;

    public int seed;
    public Vector2 offset;

    [Range(0,1)]
    public float useFalloff;
    
    public bool applyErosion;
    public int iterations;
    public float m_talus;
    [Range(0,1)]
    public float m_fraction;

    public GameObject plane;
    public GameObject mesh;


    public float meshHeightMultiplier;
    public AnimationCurve meshHeightCurve;

    public bool autoUpdate;

    public TerrainType[] regions;

    float[,] falloffMap;
    static MapGenerator instance
    {
        get
        {
                return FindObjectOfType<MapGenerator>();
        }
    }

    public float unitThreshold;
    public float buildingThreshold;
    public int buildingSize;


    private void Awake()
    {
        falloffMap = FalloffGenerator.GenerateFalloffMap(mapChunkSize);
    }

    public MapData GenerateMapData(Vector2 center)
    {
        float[,] noiseMap;
        
        if (drawMode == DrawMode.PerlinNoise)
        {
            noiseMap = Noise.GeneratePerlinNoise(mapChunkSize, seed, noiseScale, octaves, persistance, lacunarity, offset);
        }
        else if(drawMode == DrawMode.ClassicNoise)
        {
            noiseMap = Noise.GenerateClassicNoise(mapChunkSize);
        }
        else
        {
            noiseMap = Noise.GenerateNoiseMap(mapChunkSize, seed, noiseScale, octaves, persistance, lacunarity, center + offset, normalizeMode);
        }

        if (applyErosion)
        {
            Erosion.ThermicErosion(noiseMap, mapChunkSize, iterations, m_talus, m_fraction);
        }

        Color[] colorMap = new Color[mapChunkSize * mapChunkSize];
        for (int y = 0; y < mapChunkSize; y++)
        {
            for (int x = 0; x < mapChunkSize; x++)
            {
                if (useFalloff>0)
                {
                    noiseMap[x, y] = Mathf.Clamp01(noiseMap[x, y] - useFalloff * falloffMap[x, y]);
                }
                float currentHeight = noiseMap[x, y];
                for (int i = 0; i < regions.Length; i++)
                {
                    if(currentHeight >= regions[i].height)
                    {
                        colorMap[y * mapChunkSize + x] = regions[i].colour;
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }
        return new MapData(noiseMap, colorMap);
    }
    

    public void DrawMapInEditor()
    {
        if (drawMode is DrawMode.Mesh or DrawMode.PLCurve)
        {
            plane.SetActive(false);
            mesh.SetActive(true);
        }
        else
        {
            plane.SetActive(true);
            mesh.SetActive(false);
        }


            MapData mapData = GenerateMapData(Vector2.zero);

        MapDisplay display = FindObjectOfType<MapDisplay>();
        if (drawMode == DrawMode.NoiseMap || drawMode == DrawMode.PerlinNoise || drawMode == DrawMode.ClassicNoise)
        {
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(mapData.heightMap));
        }
        else if (drawMode == DrawMode.ColorMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromColorMap(mapData.colorMap, mapChunkSize, mapChunkSize));
        }
        else if (drawMode == DrawMode.Mesh)
        {
            display.DrawMesh(MeshGenerator.GenerateTerrainMesh(mapData.heightMap, meshHeightMultiplier, meshHeightCurve), TextureGenerator.TextureFromColorMap(mapData.colorMap, mapChunkSize, mapChunkSize));
        }
        else if(drawMode == DrawMode.FalloffMap)
        {
            float[,] fallOff = FalloffGenerator.GenerateFalloffMap(mapChunkSize);
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(fallOff));
        }
        else if(drawMode == DrawMode.Slopes)
        {
            float[,] slopes = PlayabilityScore.CalculateSlopes(mapData.heightMap);
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(PlayabilityScore.NormalizeSlopes(slopes)));
        }
        else if (drawMode == DrawMode.A)
        {
            float[,] A = PlayabilityScore.ArrayIntToFloat(PlayabilityScore.CalculateAccessibilityMap(mapData.heightMap, unitThreshold));
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(A));
        }
        else if (drawMode == DrawMode.U)
        {
            float[,] U = PlayabilityScore.ArrayIntToFloat(PlayabilityScore.CalculateUnitMap(mapData.heightMap, unitThreshold));
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(U));
        }
        else if (drawMode == DrawMode.F)
        {
            float[,] F = PlayabilityScore.ArrayIntToFloat(PlayabilityScore.CalculateFlatnessMap(mapData.heightMap, buildingThreshold, buildingSize));
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(F));
        }
        else if (drawMode == DrawMode.B)
        {
            int[,] U = PlayabilityScore.CalculateUnitMap(mapData.heightMap, unitThreshold);
            float[,] B = PlayabilityScore.ArrayIntToFloat(PlayabilityScore.CalculateBuildingMap(mapData.heightMap, U, buildingThreshold, buildingSize));
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(B));
        }
        else if (drawMode == DrawMode.E)
        {
            float[,] E = PlayabilityScore.ArrayIntToFloat(PlayabilityScore.CalculateExtremaMap(mapData.heightMap));
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(E));
        }
        else if(drawMode == DrawMode.LCurve)
        {
            int size = 400;
            float[] values = new float[size + 1];
            
            for (int i = 0; i < size+1; i++)
            {
                float tempLacunarity = 2*i/(float)size +1;
                float[,] tempHeightMap = Noise.GenerateNoiseMap(mapChunkSize, seed, noiseScale, octaves, persistance, tempLacunarity, offset, normalizeMode);

                float score = PlayabilityScore.CalculatePlayabilityScore(tempHeightMap, unitThreshold, buildingThreshold, buildingSize)[0] * 4;
                values[i] = score;

            }
            display.DrawTexture(TextureGenerator.CurveTexture2D(values, size));
        }
        else if (drawMode == DrawMode.PCurve)
        {
            int size = 400;
            float[] values = new float[size + 1];

            for (int i = 0; i < size + 1; i++)
            {
                float tempPersistance = i / (float)size;
                float[,] tempHeightMap = Noise.GenerateNoiseMap(mapChunkSize, seed, noiseScale, octaves, tempPersistance, lacunarity, offset, normalizeMode);

                float score = PlayabilityScore.CalculatePlayabilityScore(tempHeightMap, unitThreshold, buildingThreshold, buildingSize)[0] * 4;
                values[i] = score;

            }

            display.DrawTexture(TextureGenerator.CurveTexture2D(values, size));
        }
        else if(drawMode == DrawMode.PLCurve)
        {
            int size = 50;
            float[,] values = new float[size + 1, size + 1];
            float maxScore = -1f;
            Vector2 maxScoreVariables= new Vector2();

            for (int x = 0; x < size + 1; x++)
            {
                for (int y = 0; y < size + 1; y++)
                {
                    float tempPersistance =(float) (x / (float)size);
                    float tempLacunarity = (9 * (float)(y / (float)size)) + 1;

                    float scoreMean = 0f;
                    int iterations = 10;

                    for (int i = 0; i < iterations; i++)
                    {
                        float[,] tempHeightMap = Noise.GenerateNoiseMap(mapChunkSize, seed + i, noiseScale, octaves, tempPersistance, tempLacunarity, offset, normalizeMode);
                        float[] scores = PlayabilityScore.CalculatePlayabilityScore(tempHeightMap, unitThreshold, buildingThreshold, buildingSize);

                        scoreMean += scores[0];
                    }
                    scoreMean = scoreMean / iterations;

                    values[x,y] = scoreMean;
                    if(scoreMean > maxScore)
                    {
                        maxScore = scoreMean;
                        maxScoreVariables.x = tempPersistance; maxScoreVariables.y = tempLacunarity;
                    }
                }
            }

            Debug.Log("score maximal : " + maxScore + " ; atteint avec persistance = " + maxScoreVariables.x + " et lacunarité =" + maxScoreVariables.y);
            display.DrawMesh(MeshGenerator.GenerateTerrainMesh(values, size, AnimationCurve.Linear(0,0,2,5)), TextureGenerator.CurveTexture3D(size));
        }
    }

    private void OnValidate()
    {
        if(lacunarity < 1)
        {
            lacunarity = 1;
        }
        if(octaves < 0)
        {
            octaves = 0;
        }

        falloffMap = FalloffGenerator.GenerateFalloffMap(mapChunkSize);
    }
}

[System.Serializable]
public struct TerrainType
{
    public string name;
    public float height;
    public Color colour;
}

public struct MapData
{
    public readonly float[,] heightMap;
    public readonly Color[] colorMap;

    public MapData(float[,] heightMap, Color[] colorMap)
    {
        this.heightMap = heightMap;
        this.colorMap = colorMap;
    }
}