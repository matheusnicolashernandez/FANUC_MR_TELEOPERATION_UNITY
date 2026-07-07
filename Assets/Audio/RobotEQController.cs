using UnityEngine;
using UnityEngine.Audio;

public class RobotEQController : MonoBehaviour
{
    [Header("Mixer")]
    public AudioMixer audioMixer;

    [Header("Transform que se move (robô)")]
    public Transform robotTransform;

    [Header("Configuração de Movimento")]
    public float movementThreshold = 0.00001f; // Sensível a pequenos movimentos
    public float holdTimeAfterMove = 1.0f;     // Tempo em segundos após o último movimento

    [Header("EQ Banda de 4000 Hz")]
    public float boostGain = 10f;
    public float normalGain = 0f;

    private Vector3 lastPosition;
    private bool isBoosted = false;
    private float holdTimer = 0f;

    void Start()
    {
        if (audioMixer != null)
        {
            audioMixer.SetFloat("Gain_4000Hz", normalGain);
        }

        if (robotTransform == null)
        {
            Debug.LogError("[EQController] robotTransform não atribuído!");
        }

        lastPosition = robotTransform.position;
    }

    void Update()
    {
        if (robotTransform == null || audioMixer == null) return;

        float distance = Vector3.Distance(robotTransform.position, lastPosition);
        lastPosition = robotTransform.position;

        // Se o robô se moveu acima do threshold
        if (distance > movementThreshold)
        {
            holdTimer = holdTimeAfterMove;

            if (!isBoosted)
            {
                bool ok = audioMixer.SetFloat("Gain_4000Hz", boostGain);
                Debug.Log("[EQController] MOVIMENTO DETECTADO → Boost aplicado → sucesso: " + ok);
                isBoosted = true;
            }
        }
        else
        {
            if (holdTimer > 0f)
            {
                holdTimer -= Time.deltaTime;
            }
            else if (isBoosted)
            {
                bool ok = audioMixer.SetFloat("Gain_4000Hz", normalGain);
                Debug.Log("[EQController] TEMPO ESGOTADO → Boost removido → sucesso: " + ok);
                isBoosted = false;
            }
        }
    }
}
