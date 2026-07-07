using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Geometry;
using RosMessageTypes.Std;
using RosMessageTypes.BuiltinInterfaces;

public class GizmoPosePublisher : MonoBehaviour
{
    private ROSConnection ros;
    public string topicName = "/unity_pose";

    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<PoseStampedMsg>(topicName);
    }

    void Update()
    {
        // Position conversion (same transformation T: (z, -x, y))
        Vector3 unityPos = transform.position;
        Vector3 rosPos = new Vector3(unityPos.z, -unityPos.x, unityPos.y);

        // For the orientation, we apply the same transformation to the forward and up vectors.
        Vector3 unityForward = transform.forward;
        Vector3 unityUp = transform.up;

        // Applying T(v) = (v.z, -v.x, v.y)
        Vector3 rosForward = new Vector3(unityForward.z, -unityForward.x, unityForward.y);
        Vector3 rosUp = new Vector3(unityUp.z, -unityUp.x, unityUp.y);

        // Reconstructs the rotation for ROS using the transformed vectors
        Quaternion rosRot = Quaternion.LookRotation(rosForward, rosUp);

        // Debug to check the vectors and angles
        //Debug.Log("Unity Forward: " + unityForward + " -> ROS Forward: " + rosForward);
        //Debug.Log("Unity Up: " + unityUp + " -> ROS Up: " + rosUp);
        //Debug.Log("Unity Euler: " + transform.rotation.eulerAngles + " | ROS Euler: " + rosRot.eulerAngles);

        // Creates the PoseStamped message for ROS
        PoseStampedMsg poseMsg = new PoseStampedMsg();
        poseMsg.header = new HeaderMsg();
        poseMsg.header.stamp = new TimeMsg((uint)Time.time, 0);
        poseMsg.header.frame_id = "base_link";

        // Sets the converted position
        poseMsg.pose.position.x = rosPos.x;
        poseMsg.pose.position.y = rosPos.y;
        poseMsg.pose.position.z = rosPos.z;

        // Sets the converted orientation
        poseMsg.pose.orientation.x = rosRot.x;
        poseMsg.pose.orientation.y = rosRot.y;
        poseMsg.pose.orientation.z = rosRot.z;
        poseMsg.pose.orientation.w = rosRot.w;

        // Publishes the message to the ROS topic
        ros.Send(topicName, poseMsg);
    }
}