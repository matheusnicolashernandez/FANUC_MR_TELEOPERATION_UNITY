using System.Collections.Generic;
using UnityEngine;

public class ChangeColorOnCollision_garra : MonoBehaviour
{
    private Renderer[] renderers;
    private Material[][] originalMaterials;
    public Material collisionMaterial;

    [Header("Objetos a Ignorar")]
    public List<GameObject> ignoreCollisionObjects;

    private bool isColliding = false;
    private bool canDetectCollision = false;

    public bool IsColliding => isColliding; // <- Adicionado aqui

    void Start()
    {
        renderers = GetComponentsInChildren<Renderer>();
        originalMaterials = new Material[renderers.Length][];

        for (int i = 0; i < renderers.Length; i++)
        {
            Material[] mats = renderers[i].materials;
            originalMaterials[i] = new Material[mats.Length];
            for (int j = 0; j < mats.Length; j++)
            {
                originalMaterials[i][j] = new Material(mats[j]); // cópia real
            }
        }

        ResetToOriginalMaterials();
        Invoke(nameof(EnableCollisionDetection), 1.0f);
    }

    void EnableCollisionDetection()
    {
        canDetectCollision = true;
        Debug.Log("Detecção de colisão ativada.");
    }

    void OnTriggerEnter(Collider other)
    {
        if (!canDetectCollision || IsIgnoredObject(other.gameObject)) return;

        isColliding = true;
        Debug.Log($"COLISÃO DETECTADA COM: {other.name}");

        foreach (Renderer rend in renderers)
        {
            Material[] newMats = new Material[rend.materials.Length];
            for (int i = 0; i < newMats.Length; i++)
            {
                newMats[i] = collisionMaterial;
            }
            rend.materials = newMats;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (!canDetectCollision || IsIgnoredObject(other.gameObject)) return;

        isColliding = false;
        Debug.Log($"FIM DA COLISÃO COM: {other.name}");

        ResetToOriginalMaterials();
    }

    void ResetToOriginalMaterials()
    {
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].materials = originalMaterials[i];
        }
    }

    bool IsIgnoredObject(GameObject obj)
    {
        return ignoreCollisionObjects.Contains(obj);
    }
}
