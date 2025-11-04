using UnityEngine;

public class TreeSpawner : MonoBehaviour
{
    public GameObject treePrefab;
    public int treeCount = 100;
    public float minHeightForTrees = 2f;
    public float maxSlope = 30f;

    public void SpawnTrees()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        Mesh mesh = meshFilter.sharedMesh;

        Vector3[] vertices = mesh.vertices;
        Vector3[] normals = mesh.normals;

        // summon trees randomly
        for (int i = 0; i < treeCount; i++)
        {
            int randomIndex = Random.Range(0, vertices.Length);
            Vector3 pos = vertices[randomIndex];
            Vector3 normal = normals[randomIndex];

            // check if cliff is too shear for trees & apply rotation
            float slope = Vector3.Angle(normal, Vector3.up);

            if (pos.y > minHeightForTrees && slope < maxSlope)
            {
                Vector3 worldPos = transform.TransformPoint(pos);
                Quaternion rotation = Quaternion.FromToRotation(Vector3.up, normal);

                Instantiate(treePrefab, worldPos, rotation, transform);
            }
        }
    }

    void OnValidate()
    {
        SpawnTrees();
    }

    // TODO:
    // Auto Remove Trees when chaning seeds
    // Create & refrence different tree types


}
