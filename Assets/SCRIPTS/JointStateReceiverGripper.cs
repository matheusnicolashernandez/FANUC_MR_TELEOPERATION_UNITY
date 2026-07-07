using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Sensor;
using System.Collections.Generic;

public class JointStateReceiverGripper : MonoBehaviour
{
    private Dictionary<string, ArticulationBody> rosToUnityMap;
    private ArticulationBody[] articulationBodies;

    // Dictionary that maps ROS joint names to Unity names
    private Dictionary<string, string> rosToUnityJointMapping;

    void Start()
    {
        // Defines the new mapping: ROS -> Unity
        rosToUnityJointMapping = new Dictionary<string, string>()
        {
            { "gripper_controller", "gripper_left3" },
            { "gripper_base_to_gripper_left2", "gripper_left2" },
            { "gripper_left3_to_gripper_left1", "gripper_left1" },
            { "gripper_base_to_gripper_right3", "gripper_right3" },
            { "gripper_base_to_gripper_right2", "gripper_right2" },
            { "gripper_right3_to_gripper_right1", "gripper_right1" }
        };

        rosToUnityMap = new Dictionary<string, ArticulationBody>();
        articulationBodies = GetComponentsInChildren<ArticulationBody>();

        foreach (ArticulationBody articulationBody in articulationBodies)
        {
            string unityJointName = articulationBody.name;
            rosToUnityMap[unityJointName] = articulationBody;
        }

        ROSConnection.GetOrCreateInstance().Subscribe<JointStateMsg>("joint_states", JointStateCallback);
    }

    void JointStateCallback(JointStateMsg msg)
    {
        for (int i = 0; i < msg.name.Length; i++)
        {
            string rosJointName = msg.name[i];
            double jointPosition = msg.position[i];

            if (rosToUnityJointMapping.ContainsKey(rosJointName))
            {
                string unityJointName = rosToUnityJointMapping[rosJointName];

                if (rosToUnityMap.ContainsKey(unityJointName))
                {
                    ArticulationBody unityJoint = rosToUnityMap[unityJointName];

                    float jointPositionDegrees = (float)(jointPosition * Mathf.Rad2Deg);

                    ArticulationDrive drive = unityJoint.xDrive;
                    drive.target = jointPositionDegrees;
                    unityJoint.xDrive = drive;
                }
            }
        }
    }

    void ConfigureArticulationBodies()
    {
        foreach (ArticulationBody joint in articulationBodies)
        {
            ArticulationDrive drive = joint.xDrive;
            drive.stiffness = 100000;
            drive.damping = 10000;
            drive.forceLimit = 1000;
            joint.xDrive = drive;
        }
    }

    void Update()
    {
        ConfigureArticulationBodies();
    }
}