using UnityEngine;

[ExecuteInEditMode]  // Permite que o código seja executado também no modo de edição
public class SphereRadiusAdjuster : MonoBehaviour
{
    public float radius = 2.0f;  // Novo raio desejado
    private Mesh sphereMesh;     // Referência à malha da esfera
    private Vector3[] originalVertices; // Vértices da malha original

    void Awake()
    {
        // Chama o ajuste do raio sempre que o valor for alterado no Inspector
        AdjustRadius(radius);
    }

    void AdjustRadius(float newRadius)
    {
        // Verifica se o MeshFilter está presente no objeto
        if (TryGetComponent<MeshFilter>(out MeshFilter meshFilter))
        {
            // Usando sharedMesh para evitar o vazamento de memória
            sphereMesh = meshFilter.sharedMesh;
            originalVertices = sphereMesh.vertices;

            // Cria uma cópia da malha para alterá-la
            Vector3[] newVertices = new Vector3[originalVertices.Length];

            // Atualiza os vértices com o novo raio
            for (int i = 0; i < originalVertices.Length; i++)
            {
                newVertices[i] = originalVertices[i].normalized * newRadius;
            }

            // Atualiza a malha com os novos vértices
            sphereMesh.vertices = newVertices;
            sphereMesh.RecalculateNormals();
            sphereMesh.RecalculateBounds();
        }
        else
        {
            Debug.LogError("MeshFilter não encontrado no objeto!");
        }
    }
}
