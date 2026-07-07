using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Std;

public class ButtonBExecute : MonoBehaviour
{
    [Header("Input (Escolha o botão desejado)")]
    public InputActionProperty buttonAction;

    [Header("Nome do Serviço ROS")]
    public string serviceName = "/execute_trajectory";

    [Header("Detectores de Colisão (opcional)")]
    public ChangeColorOnCollision[] collisionDetectors;
    public ChangeColorOnCollision_garra[] garraDetectors;

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
            // Verifica colisão nos objetos com ChangeColorOnCollision
            if (collisionDetectors != null && collisionDetectors.Length > 0)
            {
                foreach (var detector in collisionDetectors)
                {
                    if (detector != null && detector.isColliding)
                    {
                        Debug.LogWarning("Execução bloqueada por colisão com: " + detector.gameObject.name);
                        lastButtonState = isPressed;
                        return;
                    }
                }
            }

            // Verifica colisão nos objetos com ChangeColorOnCollision_garra
            if (garraDetectors != null && garraDetectors.Length > 0)
            {
                foreach (var garra in garraDetectors)
                {
                    if (garra != null && garra.IsColliding) // <- usa getter público
                    {
                        Debug.LogWarning("Execução bloqueada por colisão com garra: " + garra.gameObject.name);
                        lastButtonState = isPressed;
                        return;
                    }
                }
            }

            Debug.Log("Botão pressionado — enviando /execute_trajectory...");
            var request = new EmptyRequest();
            ros.SendServiceMessage<EmptyResponse>(serviceName, request, CallbackResponse);
        }

        lastButtonState = isPressed;
    }

    void CallbackResponse(EmptyResponse response)
    {
        Debug.Log("Resposta recebida do ROS: Trajetória executada com sucesso.");
    }
}
