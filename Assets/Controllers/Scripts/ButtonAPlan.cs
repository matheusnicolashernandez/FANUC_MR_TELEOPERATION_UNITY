using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Std;

public class ButtonAPlan : MonoBehaviour
{
    [Header("Input (Escolha o botão desejado)")]
    public InputActionProperty buttonAction;

    [Header("Nome do Serviço ROS")]
    public string serviceName = "/plan_trajectory";

    [Header("Detectores de Colisão (opcional)")]
    public ChangeColorOnCollision[] collisionDetectors;

    private ROSConnection ros;
    private bool lastButtonState = false;

    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterRosService<EmptyRequest, EmptyResponse>(serviceName);
    }

    void OnEnable()
    {
        if (buttonAction.action != null)
            buttonAction.action.Enable();
    }

    void OnDisable()
    {
        if (buttonAction.action != null)
            buttonAction.action.Disable();
    }

    void Update()
    {
        if (buttonAction.action == null) return;

        bool isPressed = buttonAction.action.ReadValue<float>() > 0.5f;

        if (isPressed && !lastButtonState)
        {
            // Verifica colisão antes de enviar
            if (collisionDetectors != null && collisionDetectors.Length > 0)
            {
                foreach (var detector in collisionDetectors)
                {
                    if (detector != null && detector.isColliding)
                    {
                        Debug.LogWarning("Planejamento bloqueado por colisão com: " + detector.gameObject.name);
                        lastButtonState = isPressed;
                        return;
                    }
                }
            }

            Debug.Log("Botão pressionado — enviando /plan_trajectory...");
            var request = new EmptyRequest();
            ros.SendServiceMessage<EmptyResponse>(serviceName, request, CallbackResponse);
        }

        lastButtonState = isPressed;
    }

    void CallbackResponse(EmptyResponse response)
    {
        Debug.Log("Resposta recebida do ROS: Planejamento de trajetória concluído.");
    }
}
