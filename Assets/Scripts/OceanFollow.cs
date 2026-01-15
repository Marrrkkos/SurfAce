using UnityEngine;

public class OceanFollow : MonoBehaviour
{
    public Transform surfBoardTransform;

    [Header("Gitter-Einstellungen")]
    [Tooltip("Der Abstand zwischen den Vertices deines Meshes (z.B. 1.0 oder 2.0). Verhindert Flimmern.")]
    public float gridSpacing = 1.0f;

    private float offsetX;
    private float offsetZ;

    void Start()
    {
        if (transform == null) return;

        // Wir nutzen localBounds oder lossyScale, um die echte Mesh-Größe zu bekommen
        // Renderer.bounds ändert sich leider, wenn das Objekt sich bewegt/verformt.
        MeshFilter mf = transform.GetComponent<MeshFilter>();
        if (mf != null && mf.sharedMesh != null)
        {
            // Größe = Mesh-Breite * Scale des Objekts
            offsetX = (mf.sharedMesh.bounds.size.x * transform.lossyScale.x) / 2f;
            offsetZ = (mf.sharedMesh.bounds.size.z * transform.lossyScale.z) / 2f;
        }

        // Auch den Offset runden wir auf das Gitter ein
        offsetX = Mathf.Round(offsetX / gridSpacing) * gridSpacing;
        offsetZ = Mathf.Round(offsetZ / gridSpacing) * gridSpacing;
    }

    void LateUpdate()
    {
        if (surfBoardTransform == null) return;

        // 1. Berechne das Snapping basierend auf dem exakten Vertex-Abstand
        // Das verhindert das "Rutschen" der Textur/Wellen.
        float snappedX = Mathf.Round(surfBoardTransform.position.x / gridSpacing) * gridSpacing;
        float snappedZ = Mathf.Round(surfBoardTransform.position.z / gridSpacing) * gridSpacing;

        // 2. Bewege das Mesh an die gesnappte Position
        // Y bleibt auf 0 (oder deiner Wasserhöhe)
        transform.position = new Vector3(snappedX - offsetX, 0, snappedZ - offsetZ);
    }
}
