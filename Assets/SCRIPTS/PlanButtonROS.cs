using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Std;
using UnityEngine.XR.Content.Interaction;

public class PlanButtonROS : MonoBehaviour
{
    private ROSConnection ros;
    public string serviceName = "/plan_trajectory";
    public XRGripButton button;

    // Array to store references to objects with ChangeColorOnCollision
    public ChangeColorOnCollision[] collisionDetectors;

    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterRosService<EmptyRequest, EmptyResponse>(serviceName);

        if (button != null)
            button.onPress.AddListener(PressExecuteButton);
        else
            Debug.LogError("XRGripButton was not assigned to the PlanButtonROS script!");
    }

    public void PressExecuteButton()
    {
        // Checks whether any of the objects are colliding
        if (collisionDetectors != null && collisionDetectors.Length > 0)
        {
            foreach (ChangeColorOnCollision detector in collisionDetectors)
            {
                if (detector != null && detector.isColliding)
                {
                    Debug.Log("Planning not allowed: active collision in " + detector.gameObject.name);
                    return;
                }
            }
        }

        Debug.Log("Button pressed. Sending /plan_trajectory to ROS...");
        EmptyRequest request = new EmptyRequest();
        ros.SendServiceMessage<EmptyResponse>(serviceName, request, CallbackResponse);
    }

    void CallbackResponse(EmptyResponse response)
    {
        Debug.Log("Response received from ROS: Trajectory planned!");
    }
}