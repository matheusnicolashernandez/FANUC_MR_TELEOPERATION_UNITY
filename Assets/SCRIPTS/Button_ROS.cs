using UnityEngine;
using UnityEngine.XR.Content.Interaction;  // Necessário para XRGripButton
using Unity.Robotics.ROSTCPConnector;        // Necessário para ROSConnection
using RosMessageTypes.Std;

public class Button_ROS : MonoBehaviour
{
    // Atribua no Inspector o objeto que possui o componente XRGripButton
    public XRGripButton planButton;

    void Start()
    {
        if (planButton != null)
        {
            // Adiciona a função que será chamada quando o botão for pressionado
            planButton.onPress.AddListener(PressPlanButton);
            Debug.Log("Listener adicionado ao planButton.");
        }
        else
        {
            Debug.LogWarning("planButton não foi atribuído no Inspector!");
        }
    }

    void PressPlanButton()
    {
        Debug.Log("Botão pressionado. Enviando /plan_trajectory para o ROS...");
        // Chama o serviço "/plan_trajectory" com uma mensagem vazia
        ROSConnection.instance.SendServiceMessage<EmptyResponse>(
            "/plan_trajectory",
            new EmptyRequest(),
            (response) =>
            {
                Debug.Log("Recebemos resposta do serviço /plan_trajectory.");
            }
        );
    }
}