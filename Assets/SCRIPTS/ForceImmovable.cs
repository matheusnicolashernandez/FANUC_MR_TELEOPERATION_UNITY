using UnityEngine;

[RequireComponent(typeof(ArticulationBody))]
public class ForceImmovable : MonoBehaviour
{
    private ArticulationBody ab;

    void Awake()
    {
        ab = GetComponent<ArticulationBody>();

        // Forces Immovable mode in Awake
        ab.immovable = true;

        Debug.Log($"[ForceImmovable] ArticulationBody '{gameObject.name}' configured as immovable.");
    }

    void FixedUpdate()
    {
        // Ensures it remains immovable even if Unity attempts to override it
        if (!ab.immovable)
        {
            ab.immovable = true;
            Debug.LogWarning($"[ForceImmovable] Forcing '{gameObject.name}' to be immovable again.");
        }
    }
}