using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Std;
using UnityEngine.XR.Content.Interaction;

public class ExecuteButtonROS : MonoBehaviour
{
    private ROSConnection ros;
    public string serviceName = "/execute_trajectory";
    public XRGripButton button;

    [Header("Collision Detectors")]
    public ChangeColorOnCollision[] collisionDetectors;
    public ChangeColorOnCollision_garra[] garraDetectors;

    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterRosService<EmptyRequest, EmptyResponse>(serviceName);

        if (button != null)
            button.onPress.AddListener(PressExecuteButton);
        else
            Debug.LogError("XRGripButton was not assigned to the ExecuteButtonROS script!");
    }

    public void PressExecuteButton()
    {
        // Checks whether any object with ChangeColorOnCollision is colliding
        if (collisionDetectors != null)
        {
            foreach (var detector in collisionDetectors)
            {
                if (detector != null && detector.isColliding)
                {
                    Debug.Log("Execution not allowed: active collision in " + detector.gameObject.name);
                    return;
                }
            }
        }

        // Checks whether any object with ChangeColorOnCollision_garra is colliding
        if (garraDetectors != null)
        {
            foreach (var garra in garraDetectors)
            {
                if (garra != null && garra.IsColliding) // <- USING PUBLIC GETTER
                {
                    Debug.Log("Execution not allowed: active collision in the gripper: " + garra.gameObject.name);
                    return;
                }
            }
        }

        if (ros == null)
        {
            Debug.LogError("ROSConnection not initialized!");
            return;
        }

        Debug.Log("Execute button pressed. Sending /execute_trajectory to ROS...");
        EmptyRequest request = new EmptyRequest();
        ros.SendServiceMessage<EmptyResponse>(serviceName, request, CallbackResponse);
    }

    void CallbackResponse(EmptyResponse response)
    {
        Debug.Log("Response received from ROS: Trajectory executed!");
    }
}