using UnityEngine;

public class ChangeColorOnCollision : MonoBehaviour
{
    private Renderer objectRenderer;
    private Material[] originalMaterials;

    [Header("Material para indicar colisão")]
    public Material collisionMaterial;

    [Header("Objetos a serem ignorados")]
    public GameObject[] ignoredObjects;

    // Flag que indica se há colisão
    public bool isColliding = false;

    void Start()
    {
        // Obtém o Renderer e armazena os materiais originais
        objectRenderer = GetComponent<Renderer>();
        originalMaterials = objectRenderer.materials;
    }

    void OnTriggerEnter(Collider other)
    {
        // Verifica se o objeto deve ser ignorado
        foreach (GameObject ignored in ignoredObjects)
        {
            if (other.gameObject == ignored)
            {
                
                return; // Sai da função sem alterar nada
            }
        }

        isColliding = true;

        // Troca todos os materiais pelo material de colisão
        Material[] materials = new Material[objectRenderer.materials.Length];
        for (int i = 0; i < materials.Length; i++)
        {
            materials[i] = collisionMaterial;
        }
        objectRenderer.materials = materials;

        Debug.Log($"Trigger detectado com: {other.gameObject.name}");
    }

    void OnTriggerExit(Collider other)
    {
        // Verifica se o objeto deve ser ignorado
        foreach (GameObject ignored in ignoredObjects)
        {
            if (other.gameObject == ignored)
            {
                return; // Sai sem restaurar (esse trigger foi ignorado)
            }
        }

        isColliding = false;

        // Restaura os materiais originais
        objectRenderer.materials = originalMaterials;
        Debug.Log($"Trigger terminado com: {other.gameObject.name}");
    }
}
