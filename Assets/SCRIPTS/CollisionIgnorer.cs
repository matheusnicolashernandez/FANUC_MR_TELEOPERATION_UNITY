using UnityEngine;

public class CollisionIgnorer : MonoBehaviour
{
    [Header("Colliders to Ignore")]
    public Collider[] collidersToIgnore;

    void Start()
    {
        // Ignores collisions between all unique pairs within the list
        for (int i = 0; i < collidersToIgnore.Length; i++)
        {
            for (int j = i + 1; j < collidersToIgnore.Length; j++)
            {
                if (collidersToIgnore[i] != null && collidersToIgnore[j] != null)
                {
                    Physics.IgnoreCollision(collidersToIgnore[i], collidersToIgnore[j]);
                }
            }
        }
    }
}