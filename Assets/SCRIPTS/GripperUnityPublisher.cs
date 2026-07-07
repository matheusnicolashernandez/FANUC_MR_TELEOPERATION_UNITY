using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Sensor;
using RosMessageTypes.Std;
using RosMessageTypes.BuiltinInterfaces;

public class GripperUnityPublisher : MonoBehaviour
{
    [Header("XR Trigger Input")]
    public InputActionProperty triggerAction;

    [Header("ROS")]
    public string topicName = "/joint_states_gripper";
    public float publishRate = 0.05f; // seconds
    public float sensitivity = 0.01f; // minimum variation to consider a "change"

    private ROSConnection ros;
    private float timeElapsed = 0f;
    private float lastValue = -1f; // forces initial publication

    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<JointStateMsg>(topicName);
        Debug.Log($"[GripperPublisher] Registered on '{topicName}'");

        // Publishes the initial state (gripper open)
        PublishGripper(1f);
        lastValue = 1f; // synchronizes with the initial state
        Debug.Log("[GripperPublisher] Initial state published: gripper open (1.0)");
    }

    void Update()
    {
        timeElapsed += Time.deltaTime;

        if (timeElapsed >= publishRate)
        {
            float rawValue = triggerAction.action.ReadValue<float>();
            float triggerValue = 1f - rawValue;

            if (Mathf.Abs(triggerValue - lastValue) > sensitivity)
            {
                PublishGripper(triggerValue);
                lastValue = triggerValue;
            }

            timeElapsed = 0f;
        }
    }

    void PublishGripper(float triggerValue)
    {
        string[] jointNames = new string[]
        {
            "gripper_controller",
            "gripper_base_to_gripper_left2",
            "gripper_left3_to_gripper_left1",
            "gripper_base_to_gripper_right3",
            "gripper_base_to_gripper_right2",
            "gripper_right3_to_gripper_right1"
        };

        double[] positions = new double[jointNames.Length];
        for (int i = 0; i < positions.Length; i++)
        {
            bool invert = (i == 2 || i == 3 || i == 4);
            positions[i] = invert ? -triggerValue : triggerValue;
        }

        var msg = new JointStateMsg
        {
            name = jointNames,
            position = positions,
            velocity = new double[jointNames.Length],
            effort = new double[jointNames.Length],
            header = new HeaderMsg
            {
                stamp = new TimeMsg
                {
                    sec = 0,
                    nanosec = 0
                },
                frame_id = ""
            }
        };

        ros.Publish(topicName, msg);
    }
}