using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class CaveGenerator : MonoBehaviour
{
    public int width = 32;
    public int height = 32;
    public int depth = 32;

    [Range(0, 100)] public float noiseScale = 20f;
    public float surfaceThreshold = 0.5f;

    public bool useRandomSeed = false;
    public int seed = 0;

    private float offsetX;
    private float offsetY;
    private float offsetZ;

    void Start()
    {
        GenerateOffsets();
        GenerateCaves();
    }

    void OnValidate()
    {
        GenerateOffsets();
        GenerateCaves();
    }

    void GenerateOffsets()
    {
        if (useRandomSeed)
        {
            seed = Random.Range(int.MinValue, int.MaxValue);
        }

        System.Random rngSeed = new System.Random(seed);
        offsetX = rngSeed.Next(-100000, 100000);
        offsetY = rngSeed.Next(-100000, 100000);
        offsetZ = rngSeed.Next(-100000, 100000);
    }

    void GenerateCaves()
    {
        Mesh mesh = new Mesh();
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        int[,,] map = new int[width, height, depth];

        // bulding using the noise generation
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < depth; z++)
                {
                    float sampleX = (x + offsetX) / noiseScale;
                    float sampleY = (y + offsetY) / noiseScale;
                    float sampleZ = (z + offsetZ) / noiseScale;

                    float noiseValue = Perlin3D(sampleX, sampleY, sampleZ);
                    map[x, y, z] = (noiseValue > surfaceThreshold) ? 1 : 0;
                }
            }
        }

        // builds the mesh from the voxel data
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < depth; z++)
                {
                    if (map[x, y, z] == 1)
                    {
                        AddCube(vertices, triangles, x, y, z, map);
                    }
                }
            }
        }

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();

        MeshFilter meshFilter = GetComponent<MeshFilter>();
        meshFilter.mesh = mesh;
    }

    // Perlin noise :D
    float Perlin3D(float x, float y, float z)
    {
        float xy = Mathf.PerlinNoise(x, y);
        float yz = Mathf.PerlinNoise(y, z);
        float xz = Mathf.PerlinNoise(x, z);
        float yx = Mathf.PerlinNoise(y, x);
        float zx = Mathf.PerlinNoise(z, x);
        float zy = Mathf.PerlinNoise(z, y);

        return (xy + yz + xz + yx + zx + zy) / 6f;
    }

    // create a voxle for each face
    void AddCube(List<Vector3> vertices, List<int> triangles, int x, int y, int z, int[,,] map)
    {
        Vector3 cubePos = new Vector3(x, y, z);

        Vector3[] cubeVerts = new Vector3[]
        {
            cubePos + new Vector3(0,0,0),
            cubePos + new Vector3(1,0,0),
            cubePos + new Vector3(1,1,0),
            cubePos + new Vector3(0,1,0),
            cubePos + new Vector3(0,0,1),
            cubePos + new Vector3(1,0,1),
            cubePos + new Vector3(1,1,1),
            cubePos + new Vector3(0,1,1)
        };

        int[][] cubeFaces = new int[][]
        {
            new int[]{0,1,2,3}, // front
            new int[]{5,4,7,6}, // back
            new int[]{4,0,3,7}, // left
            new int[]{1,5,6,2}, // right
            new int[]{3,2,6,7}, // top
            new int[]{4,5,1,0}  // bottom
        };

        Vector3[] faceNormals = {
            Vector3.back, Vector3.forward, Vector3.left, Vector3.right, Vector3.up, Vector3.down
        };

        Vector3[] neighborChecks = {
            Vector3.back, Vector3.forward, Vector3.left, Vector3.right, Vector3.up, Vector3.down
        };

        for (int i = 0; i < cubeFaces.Length; i++)
        {
            Vector3 check = cubePos + neighborChecks[i];
            int cx = Mathf.RoundToInt(check.x);
            int cy = Mathf.RoundToInt(check.y);
            int cz = Mathf.RoundToInt(check.z);

            bool neighborSolid = (cx >= 0 && cy >= 0 && cz >= 0 && cx < width && cy < height && cz < depth && map[cx, cy, cz] == 1);

            if (!neighborSolid)
            {
                int vertStart = vertices.Count;
                vertices.Add(cubeVerts[cubeFaces[i][0]]);
                vertices.Add(cubeVerts[cubeFaces[i][1]]);
                vertices.Add(cubeVerts[cubeFaces[i][2]]);
                vertices.Add(cubeVerts[cubeFaces[i][3]]);

                triangles.Add(vertStart + 0);
                triangles.Add(vertStart + 1);
                triangles.Add(vertStart + 2);
                triangles.Add(vertStart + 0);
                triangles.Add(vertStart + 2);
                triangles.Add(vertStart + 3);
            }
        }
    }
}
