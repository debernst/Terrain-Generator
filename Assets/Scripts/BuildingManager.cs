using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    public BuildingGenerator buildingGeneratorPrefab;
    public MeshFilter terrainMesh;

    [Header("Building Spawn Settings")]
    public int buildingCount = 10;
    public float minHeight = 2f;
    public float maxHeight = 25f;
    public float maxSlope = 20f;

    [Header("Random Building Variations")]
    public Vector2Int widthRange = new Vector2Int(1, 4);
    public Vector2Int heightRange = new Vector2Int(1, 4);
    public Vector2Int floorsRange = new Vector2Int(1, 5);

    private Mesh mesh;

    private void Start()
    {
        mesh = terrainMesh.sharedMesh;

        SpawnBuildings();
    }

    // Main Spawning Logic
    public void SpawnBuildings()
    {
        Vector3[] verts = mesh.vertices;
        Vector3[] normals = mesh.normals;

        int placed = 0;
        int safety = 0;

        while (placed < buildingCount && safety < buildingCount * 30)
        {
            safety++;

            int index = Random.Range(0, verts.Length);

            // Convert vertex to world position
            Vector3 pos = terrainMesh.transform.TransformPoint(verts[index]);
            float slope = Vector3.Angle(normals[index], Vector3.up);

            // Height and slope checks
            if (pos.y < minHeight) continue;
            if (pos.y > maxHeight) continue;
            if (slope > maxSlope) continue;

            // Instantiate building
            BuildingGenerator building = Instantiate(buildingGeneratorPrefab, transform);

            // Random building shape
            building.width = Random.Range(widthRange.x, widthRange.y + 1);
            building.height = Random.Range(heightRange.x, heightRange.y + 1);
            building.numberOfFloors = Random.Range(floorsRange.x, floorsRange.y + 1);

            // Move building to terrain location with offset
            float halfW = building.width * building.cellUnitSize * 0.5f;
            float halfH = building.height * building.cellUnitSize * 0.5f;

            Vector3 correctedPos = new Vector3(pos.x - halfW, pos.y, pos.z - halfH);

            building.transform.position = correctedPos;

            building.GenerateBuilding();
            building.RenderBuilding();

            placed++;
        }

        Debug.Log($"Placed {placed} buildings.");
    }
}