using UnityEngine;

[ExecuteAlways]
public class PerlinNoiseGenerator : MonoBehaviour
{
    [Range(0,100)]public int depth = 20;

    public int width = 256; //x-axis
    public int height = 256; //y-axis

    [Range(0, 60)] public float scale = 20f; 

    [Range(0, 50)] public float offsetX = 0f;
    [Range(0, 50)] public float offsetY = 0f;


    private void Start()
    {
        offsetX = Random.Range(0f, 50f);
        offsetY = Random.Range(0f, 50f);
    }

    void Update ()
    {
        Terrain terrain = GetComponent<Terrain>();
        terrain.terrainData = GenerateTerrain(terrain.terrainData);
    }

    TerrainData GenerateTerrain (TerrainData terrainData)
    {
        terrainData.heightmapResolution = width + 1;

        terrainData.size = new Vector3(width, depth, height);
        terrainData.SetHeights(0, 0, GenerateHeights());
        return terrainData;
    }

    float[,] GenerateHeights ()
    {
        float[,] heights = new float[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                heights[x, y] = CalculatedHeight(x, y);
            }
        }

        return heights;
    }

    float CalculatedHeight(int x, int y)
    {
        float xCoord = (float)x / width * scale + offsetX;
        float yCoord = (float)y / height * scale + offsetY;

        return Mathf.PerlinNoise(xCoord, yCoord);
    }
}
