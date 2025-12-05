using System.Collections.Generic;
using UnityEngine;

public class TreeSpawner : MonoBehaviour
{
    [System.Serializable]
    public class TreeType
    {
        public GameObject prefab;
        [Range(0f, 1f)] public float spawnPercent = 0.25f;
    }

    public List<TreeType> treeTypes = new List<TreeType>();

    [Header("Spawn Settings")]
    public int treeCount = 100;
    public float minHeightForTrees = 2f;
    public float maxSlope = 30f;

    private List<GameObject> spawnedTrees = new List<GameObject>();
    private GameObject treeParent;

    public void SpawnTrees()
    {
        ClearTrees();

        if (treeParent == null)
        {
            treeParent = new GameObject("Trees");
            treeParent.transform.SetParent(transform);
            treeParent.transform.localPosition = Vector3.zero;
        }

        MeshFilter meshFilter = GetComponent<MeshFilter>();
        Mesh mesh = meshFilter.sharedMesh;

        Vector3[] vertices = mesh.vertices;
        Vector3[] normals = mesh.normals;

        // calculates total spawn percent (weighting)
        float totalPercent = 0f;
        foreach (var t in treeTypes)
        {
            if (t.prefab != null)
                totalPercent += Mathf.Max(0, t.spawnPercent);
        }

            // summon trees randomly
            for (int i = 0; i < treeCount; i++)
        {
            int randomIndex = Random.Range(0, vertices.Length);
            Vector3 pos = vertices[randomIndex];
            Vector3 normal = normals[randomIndex];

            // check if cliff is too shear for trees & apply rotation
            float slope = Vector3.Angle(normal, Vector3.up);

            if (pos.y < minHeightForTrees || slope > maxSlope)
                continue;

            // weighted random tree type
            float roll = Random.Range(0f, totalPercent);
            float cumulative = 0f;
            GameObject chosenTree = null;

            foreach (var t in treeTypes)
            {
                cumulative += Mathf.Max(0, t.spawnPercent);
                if (roll <= cumulative)
                {
                    chosenTree = t.prefab;
                    break;
                }
            }

            if (chosenTree == null) continue;
            {
                Vector3 worldPos = transform.TransformPoint(pos);
                Quaternion rotation = Quaternion.FromToRotation(Vector3.up, normal) * Quaternion.Euler(0, Random.Range(0, 360), 0);
                GameObject newTree = Instantiate(chosenTree, worldPos, rotation);
                newTree.transform.SetParent(treeParent.transform, true);
                spawnedTrees.Add(newTree);
            }
        }
    }

    public void ClearTrees()
    {
        // Clear all spawned trees
        for (int i = spawnedTrees.Count - 1; i >= 0; i--)
        {
            var t = spawnedTrees[i];
            if (t == null) continue;

            if (Application.isPlaying)
                Destroy(t);
            else
                DestroyImmediate(t);

            spawnedTrees.RemoveAt(i);
        }

        // Destroy the tree parent GameObject if it exists
        if (treeParent != null)
        {
            if (Application.isPlaying)
                Destroy(treeParent);
            else
                DestroyImmediate(treeParent);

            treeParent = null;
        }
    }

    private void OnDestroy()
    {
        ClearTrees();
    }

    void OnValidate()
    {
        //SpawnTrees();
    }

}
