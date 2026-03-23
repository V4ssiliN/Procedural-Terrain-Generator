using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NoiseTree
{
    public static Texture2D GetNoiseMap(int width, int height, float scale)
    {
        int newNoise = UnityEngine.Random.Range(0, 10000);

        Texture2D noiseMapTexture = new Texture2D(width, height);
        for (int x = 0; x < width; x++)
        { 
            for (int y = 0; y < height; y++) 
            {
                float noiseValue = Mathf.PerlinNoise((float)x/ width * scale + newNoise, (float)y/ height * scale + newNoise);

                noiseMapTexture.SetPixel(x,y,new Color(0, noiseValue,0));
            } 
        }
        noiseMapTexture.Apply();
        return noiseMapTexture;
    }
}
