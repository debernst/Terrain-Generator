using UnityEngine;

[ExecuteAlways]
public class TerrainChunkManager : MonoBehaviour
{
    public OctaveMeshGenerator chunkPrefab;

    public int chunksX = 3;
    public int chunksZ = 3;

    private OctaveMeshGenerator[,] chunks;

    void OnValidate()
    {
        GenerateChunkGrid();
    }

    void GenerateChunkGrid()
    {
        chunks = new OctaveMeshGenerator[chunksX, chunksZ];

        // Size of chunk
        int w = chunkPrefab.width;
        int h = chunkPrefab.height;

        // Create grid
        for (int x = 0; x < chunksX; x++)
        {
            for (int z = 0; z < chunksZ; z++)
            {
                OctaveMeshGenerator newChunk = Instantiate(chunkPrefab, transform);
                newChunk.name = $"Chunk_{x}_{z}";

                // Chunk position in world
                newChunk.transform.localPosition = new Vector3(x * w, 0, z * h);

                // MAKES NOISE SEAMLESS (shifts noise for the offsets)
                newChunk.offsetX = x * (newChunk.scale);
                newChunk.offsetZ = z * (newChunk.scale);

                chunks[x, z] = newChunk;

                // Generates terrain
                newChunk.GenerateTerrain();
            }
        }
    }  }
    }
}
