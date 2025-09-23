using UnityEngine;

[ExecuteAlways]
public class PerlinMeshGenerator : MonoBehaviour
{
    public int width = 100;
    public int height = 100;
    [Range(0, 100)] public float scale = 20f;
    [Range(0, 60)] public float heightMultiplier = 5f;

    public Gradient colorGradient;

    public bool useRandomSeed = false;
    public int seed = 0;

    private float offsetX;
    private float offsetZ;


    void Start()
    {
        GenerateOffsets();
    }
    void OnValidate()
    {
        GenerateOffsets();
        GenerateTerrain();
    }

    // generate offset for the seed based on the values
    void GenerateOffsets()
    {
        if (useRandomSeed)
        {
            seed = Random.Range(int.MinValue, int.MaxValue);
        }

        System.Random rngSeed = new System.Random(seed);
        offsetX = rngSeed.Next(-100000, 100000);
        offsetZ = rngSeed.Next(-100000, 100000);
    }

    void GenerateTerrain()
    {
        Mesh mesh = new Mesh();
        Vector3[] vertices = new Vector3[(width + 1) * (height + 1)];
        Color[] colors = new Color[vertices.Length];
        int[] triangles = new int[width * height * 6];

        float minHeight = float.MaxValue;
        float maxHeight = float.MinValue;

        // generate vertices for the tringles to go in
        for (int z = 0, i = 0; z <= height; z++)
        {
            for (int x = 0; x <= width; x++, i++)
            {
                float sampleX = ((float)x / width * scale) + offsetX;
                float sampleZ = ((float)z / height * scale) + offsetZ;

                float y = Mathf.PerlinNoise(sampleX, sampleZ) * heightMultiplier;
                vertices[i] = new Vector3(x, y, z);

                if (y < minHeight) minHeight = y;
                if (y > maxHeight) maxHeight = y;
            }
        }

        // generate triangles for the inside of the vertices
        int vert = 0;
        int tris = 0;
        for (int z = 0; z < height; z++)
        {
            for (int x = 0; x < width; x++)
            {
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + width + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + width + 1;
                triangles[tris + 5] = vert + width + 2;

                vert++;
                tris += 6;
            }
            vert++;
        }

        // assign the colors based on the height of the terrain
        for (int i = 0; i < vertices.Length; i++)
        {
            float normalizedHeight = Mathf.InverseLerp(minHeight, maxHeight, vertices[i].y);
            colors[i] = colorGradient.Evaluate(normalizedHeight);
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.colors = colors;
        mesh.RecalculateNormals();

        MeshFilter meshFilter = GetComponent<MeshFilter>();
        meshFilter.mesh = mesh;
    }
}
