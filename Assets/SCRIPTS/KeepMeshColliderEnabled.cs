using UnityEngine;

public class KeepMeshColliderEnabled : MonoBehaviour
{
    private MeshCollider meshCollider;

    void Start()
    {
        // Attempts to get the MeshCollider from the GameObject
        meshCollider = GetComponent<MeshCollider>();

        if (meshCollider == null)
        {
            Debug.LogWarning("No MeshCollider found on object " + gameObject.name);
        }
    }

    void Update()
    {
        if (meshCollider != null && !meshCollider.enabled)
        {
            meshCollider.enabled = true;
        }
    }
}