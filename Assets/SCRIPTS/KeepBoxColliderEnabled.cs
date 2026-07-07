using UnityEngine;

public class KeepBoxColliderEnabled : MonoBehaviour
{
    private BoxCollider boxCollider;

    void Start()
    {
        // Attempts to get the BoxCollider from the GameObject
        boxCollider = GetComponent<BoxCollider>();

        if (boxCollider == null)
        {
            Debug.LogWarning("No BoxCollider found on object " + gameObject.name);
        }
    }

    void Update()
    {
        if (boxCollider != null && !boxCollider.enabled)
        {
            boxCollider.enabled = true;
        }
    }
}