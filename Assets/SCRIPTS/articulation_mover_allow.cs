using UnityEngine;

[RequireComponent(typeof(ArticulationBody))]
public class FreeMoveArticulation : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float rotateSpeed = 90f;

    private ArticulationBody ab;

    void Start()
    {
        ab = GetComponent<ArticulationBody>();
        ab.immovable = false; // MUITO IMPORTANTE
    }

    void Update()
    {
        // Entrada de movimento WASD
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        // Entrada de rotação (Q/E)
        float rot = 0f;
        if (Input.GetKey(KeyCode.Q)) rot += 1f;
        if (Input.GetKey(KeyCode.E)) rot -= 1f;

        Vector3 move = new Vector3(h, 0f, v) * moveSpeed * Time.deltaTime;
        Quaternion rotDelta = Quaternion.Euler(0f, rot * rotateSpeed * Time.deltaTime, 0f);

        // Aplicar movimento e rotação com TeleportRoot
        Vector3 newPosition = ab.transform.position + move;
        Quaternion newRotation = ab.transform.rotation * rotDelta;

        ab.TeleportRoot(newPosition, newRotation);
    }
}
