using UnityEngine;

public class LockParent : MonoBehaviour
{
    private Transform originalParent;

    void Start()
    {
        originalParent = transform.parent; // Salva o parent original
    }

    void LateUpdate()
    {
        if (transform.parent != originalParent)
        {
            transform.SetParent(originalParent); // Reatribui o parent original
        }
    }
}
