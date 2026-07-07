using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Std;
using UnityEngine;

public class JointStatePublisher : MonoBehaviour
{
    ROSConnection ros;
    public string topicName = "/joint_states";

    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<Float64MultiArrayMsg>(topicName);
    }

    void Update()
    {
        var message = new Float64MultiArrayMsg();
        message.data = new double[] { 0.0, 1.0, 0.5, 0.8, -1.0, 0.2 }; // Example values to publish

        // Sends the message to ROS running on Linux
        ros.Publish(topicName, message);
    }
}