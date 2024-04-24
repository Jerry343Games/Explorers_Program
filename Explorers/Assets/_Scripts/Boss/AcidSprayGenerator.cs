using UnityEngine;

public class AcidSprayGenerator : MonoBehaviour
{
    public int numSegments = 10; // Number of segments in the sector
    public float startRadius = 1f; // Starting radius
    public float radiusIncrement = 0.1f; // Radius increment for each segment
    public float sectorAngle = 90f; // Angle of the sector
    public Material material; // Material for the area

    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private MeshCollider meshCollider;
    private Mesh mesh;
    private Vector3[] vertices;
    private int[] triangles;
    private bool isGenerated = false;

    private float maxRange = 1f; // Maximum range of spray
    private float sprayWidth = 20f; // Initial spray width
    private float sprayWidthIncrement = 1f; // Increment for spray width

    void Start()
    {
        // Initialize components
        InitializeComponents();
    }

    void Update()
    {
        if (!isGenerated)
        {
            GenerateAcidSpray(); // Generate acid spray gradually
        }
    }

    void InitializeComponents()
    {
        // Create acid spray game object
        GameObject acidSpray = new GameObject("AcidSpray");

        // Add MeshFilter, MeshRenderer, and MeshCollider components
        meshFilter = acidSpray.AddComponent<MeshFilter>();
        meshRenderer = acidSpray.AddComponent<MeshRenderer>();
        meshCollider = acidSpray.AddComponent<MeshCollider>();

        // Set material for the area
        meshRenderer.material = material;

        // Create new mesh
        mesh = new Mesh();
        meshFilter.mesh = mesh;

        // Initialize arrays for vertices and triangles
        vertices = new Vector3[numSegments * 3 + 1];
        triangles = new int[numSegments * 3 * 2];

        // Set vertex at the center of the sector to origin
        vertices[0] = Vector3.zero;
    }

    void GenerateAcidSpray()
    {
        float angleIncrement = sectorAngle / numSegments; // Calculate angle increment for each segment
        float currentAngle = -sectorAngle / 2f; // Current angle starts from the starting angle of the sector

        for (int i = 0; i < numSegments; i++)
        {
            float currentRadius = startRadius + i * radiusIncrement; // Calculate radius for the current segment

            // Calculate vertices for the current segment
            Vector3 vertex1 = new Vector3(currentRadius * Mathf.Cos(Mathf.Deg2Rad * currentAngle), currentRadius * Mathf.Sin(Mathf.Deg2Rad * currentAngle), 0f);
            Vector3 vertex2 = new Vector3(currentRadius * Mathf.Cos(Mathf.Deg2Rad * (currentAngle + angleIncrement)), currentRadius * Mathf.Sin(Mathf.Deg2Rad * (currentAngle + angleIncrement)), 0f);

            // Put vertices into the array
            vertices[i * 3 + 1] = Vector3.zero;
            vertices[i * 3 + 2] = vertex1;
            vertices[i * 3 + 3] = vertex2;

            // Build triangle indices
            triangles[i * 6] = 0;
            triangles[i * 6 + 1] = i * 3 + 1;
            triangles[i * 6 + 2] = i * 3 + 2;
            triangles[i * 6 + 3] = 0;
            triangles[i * 6 + 4] = i * 3 + 2;
            triangles[i * 6 + 5] = i * 3 + 3;

            currentAngle += angleIncrement; // Increase angle to process the next segment
        }

        // Set mesh vertices and triangles
        mesh.vertices = vertices;
        mesh.triangles = triangles;

        // Recalculate normals
        mesh.RecalculateNormals();

        // Set MeshCollider's mesh to the generated mesh
        meshCollider.sharedMesh = mesh;

        // Increase spray width and maximum range
        sprayWidth += sprayWidthIncrement;

        // Mark as generated
        isGenerated = true;
    }
}