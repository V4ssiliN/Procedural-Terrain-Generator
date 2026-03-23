public static class Erosion
{
    public static void ThermicErosion(float[,] heightMap, int mapSize, int iterations, float m_talus, float m_fraction)
    {
        float[,] d = new float[3, 3];
        float[,] material = new float[mapSize, mapSize];

        for (int k = 0; k < iterations; ++k)
        {
            for (int x = 1; x < mapSize - 1; ++x)
            {
                for (int y = 1; y < mapSize - 1; ++y)
                {
                    material[x, y] = 0f;
                }
            }
            for (int x = 1; x < mapSize - 1; ++x)
            {
                for (int y = 1; y < mapSize - 1; ++y)
                {
                    float d_total = 0f;
                    float d_max = 0f;

                    for (int i = -1; i <= 1; ++i)
                    {
                        for (int j = -1; j <= 1; ++j)
                        {
                            float diff = heightMap[x, y] - heightMap[x + i, y + j];
                            d[1 + i, 1 + j] = diff;

                            if (diff > m_talus)
                            {
                                d_total += diff;

                                if (diff > d_max)
                                {
                                    d_max = diff;
                                }
                            }
                        }
                    }

                    for (int i = -1; i <= 1; ++i)
                    {
                        for (int j = -1; j <= 1; ++j)
                        {
                            float diff = d[1 + i, 1 + j];

                            if (diff > m_talus)
                            {
                                material[x + i, y + j] += m_fraction * (d_max - m_talus) * (diff / d_total);
                            }
                        }
                    }
                }
            }
            for (int x = 1; x <= mapSize - 2; ++x)
            {
                for (int y = 1; y <= mapSize - 2; ++y)
                {
                    heightMap[x, y] += material[x, y];
                }
            }
        }

    }
}