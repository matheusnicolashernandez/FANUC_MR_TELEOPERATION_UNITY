using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Std;

public class LatencyExecuteButton : MonoBehaviour
{
    public InputActionProperty buttonAction;

    public string latencyTopic = "/latency_execute";
    public string executeService = "/execute_trajectory";

    private ROSConnection ros;
    private bool lastButtonState = false;

    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();

        ros.RegisterPublisher<Float32Msg>(latencyTopic);
        ros.RegisterRosService<EmptyRequest, EmptyResponse>(executeService);
    }

    void OnEnable()
    {
        buttonAction.action?.Enable();
    }

    void OnDisable()
    {
        buttonAction.action?.Disable();
    }

    void Update()
    {
        if (buttonAction.action == null)
            return;

        bool isPressed = buttonAction.action.ReadValue<float>() > 0.5f;

        if (isPressed && !lastButtonState)
        {
            // avisa o ROS que o botão foi apertado
            ros.Publish(
                latencyTopic,
                new Float32Msg(1.0f)
            );

            Debug.Log("[Latency] Evento enviado.");

            // executa trajetória
            ros.SendServiceMessage<EmptyResponse>(
                executeService,
                new EmptyRequest(),
                CallbackResponse
            );
        }

        lastButtonState = isPressed;
    }

    void CallbackResponse(EmptyResponse response)
    {
        Debug.Log("[Latency] Service respondeu.");
    }
}