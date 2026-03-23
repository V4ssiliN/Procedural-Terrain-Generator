using UnityEngine;

public static class TextureGenerator
{
    public static Texture2D TextureFromColorMap(Color[] colorMap, int width, int height)
    {
        Texture2D texture = new Texture2D(width,height);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.SetPixels(colorMap);
        texture.Apply();
        return texture;
    }
    public static Texture2D TextureFromHeightMap(float[,] heightMap)
    {
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);

        Color[] colorMap = new Color[width * height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                colorMap[y * width + x] = Color.Lerp(Color.black, Color.white, heightMap[x, y]);
            }
        }

        return TextureFromColorMap(colorMap,width,height);
    }

    public static Texture2D CurveTexture2D(float[] values, int length)
    {
        float[,] map = new float[length, length];

        for (int x = 0; x < length; x++)
        {
            for (int y = 0; y < length; y++)
            {
                if(x%(length/8) == 0 || y %(length/8) == 0)
                {
                    map[x, y] = .8f;
                }
                else
                {
                    map[x, y] = 1f;
                }
                
            }
        }

        for (int i = 0; i < length; i++)
        {
            if (values[i] > 1)
            {
                Debug.LogError("score > 1");
            }
            if((int)(values[i] * length) > length)
            {
                Debug.LogError("erreur");
            }
            map[length-i-1, length -(int)(values[i]*length) - 1] = 0f;
        }

        Color[] colorMap = new Color[length * length];

        for (int y = 0; y < length; y++)
        {
            for (int x = 0; x < length; x++)
            {
                colorMap[y * length + x] = Color.Lerp(Color.black, Color.white, map[x, y]);
            }
        }

        return TextureFromColorMap(colorMap, length, length);
    }



    public static Texture2D CurveTexture3D(int size)
    {
        int width = size;
        int height = size;

        Color[] colorMap = new Color[width * height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                colorMap[y * width + x] = Color.gray;
            }
        }
        return TextureFromColorMap(colorMap, width, height);
    }
}
