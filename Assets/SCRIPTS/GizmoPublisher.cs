using UnityEngine;
using RosMessageTypes.Geometry; // To use PoseMsg, PointMsg, QuaternionMsg
using Unity.Robotics.ROSTCPConnector;

public class GizmoPublisher : MonoBehaviour
{
    // ROS topic name
    public string topicName = "/unity/robot_goal";
    // Publishing rate (in seconds)
    public float publishInterval = 0.1f;
    private float timeElapsed;

    private ROSConnection ros;

    void Start()
    {
        // Gets the ROSConnection instance already configured in the project
        ros = ROSConnection.instance;
        timeElapsed = 0f;
    }

    void Update()
    {
        timeElapsed += Time.deltaTime;
        if (timeElapsed > publishInterval)
        {
            PublishGizmoPose();
            timeElapsed = 0f;
        }
    }

    void PublishGizmoPose()
    {
        // Captures the gizmo's current position and rotation
        Vector3 position = transform.position;
        Quaternion rotation = transform.rotation;

        // If necessary, perform the coordinate conversion here.
        // Example: convert Unity's Y-axis to ROS's Z-axis, etc.

        // Creates the ROS message of type PoseMsg
        PoseMsg pose = new PoseMsg
        {
            position = new PointMsg(position.x, position.y, position.z),
            orientation = new QuaternionMsg(rotation.x, rotation.y, rotation.z, rotation.w)
        };

        // Publishes the message to the specified topic
        ros.Publish(topicName, pose);
    }
}