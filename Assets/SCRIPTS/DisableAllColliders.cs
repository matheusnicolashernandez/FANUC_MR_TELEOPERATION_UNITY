using UnityEngine;

public class DisableAllColliders : MonoBehaviour
{
    void Start()
    {
        Collider[] allColliders = GetComponentsInChildren<Collider>(true);
        int count = 0;

        foreach (var col in allColliders)
        {
            col.enabled = false;
            count++;
        }

        Debug.Log($"[DisableAllColliders] {count} colliders disabled on GameObject '{gameObject.name}' and its children.");
    }
}