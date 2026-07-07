using UnityEngine;

public class PreventOverlap : MonoBehaviour
{
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Garante que o Rigidbody está configurado corretamente
        rb.useGravity = false; // Desativa a gravidade se necessário
    }

    void OnCollisionEnter(Collision collision)
    {
        // Exemplo: Loga quando algo colide com o bloco
        Debug.Log($"Colisão detectada com: {collision.gameObject.name}");

        // Impede a sobreposição ajustando a posição do objeto
        Vector3 closestPoint = collision.collider.ClosestPointOnBounds(transform.position);
        transform.position = closestPoint;  // Ajusta a posição para evitar sobreposição
    }
}