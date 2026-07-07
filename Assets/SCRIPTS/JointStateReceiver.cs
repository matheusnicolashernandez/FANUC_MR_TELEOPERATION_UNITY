using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Sensor;
using System.Collections.Generic;

public class JointStateReceiver : MonoBehaviour
{
    private Dictionary<string, ArticulationBody> rosToUnityMap;
    private ArticulationBody[] articulationBodies;

    // Dictionary that maps ROS joint names to Unity names
    private Dictionary<string, string> rosToUnityJointMapping;

    void Start()
    {
        // Defines the mapping: ROS -> Unity
        rosToUnityJointMapping = new Dictionary<string, string>()
        {
            { "joint_1", "Link_1" },
            { "joint_2", "Link_2" },
            { "joint_3", "Link_3" },
            { "joint_4", "Link_4" },
            { "joint_5", "Link_5" },
            { "joint_6", "Link_6" }
        };

        rosToUnityMap = new Dictionary<string, ArticulationBody>();
        articulationBodies = GetComponentsInChildren<ArticulationBody>();

        // Maps the names of the Unity ArticulationBodies
        foreach (ArticulationBody articulationBody in articulationBodies)
        {
            string unityJointName = articulationBody.name;
            rosToUnityMap[unityJointName] = articulationBody;
            //  Debug.Log($"Mapped joint: {unityJointName}");
        }

        //  Debug.Log("JointStateReceiver: Starting subscription to /joint_states...");
        ROSConnection.GetOrCreateInstance().Subscribe<JointStateMsg>("joint_states", JointStateCallback);
    }

    void JointStateCallback(JointStateMsg msg)
    {
        // Debug.Log("JointStateReceiver: Received joint_states message with " + msg.name.Length + " joints.");

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

                    // Reverses the direction of joints 3, 4, and 6
                    if (rosJointName == "joint_3" || rosJointName == "joint_4" || rosJointName == "joint_6")
                    {
                        jointPositionDegrees *= -1;
                    }

                    ArticulationDrive drive = unityJoint.xDrive;
                    drive.target = jointPositionDegrees;
                    unityJoint.xDrive = drive;

                    //   Debug.Log($"Receiving {rosJointName} = {jointPosition} radians ({jointPositionDegrees} degrees) -> {unityJointName}");
                }
                else
                {
                    //  Debug.LogWarning($"Joint {unityJointName} not found in Unity!");
                }
            }
            else
            {
                //  Debug.LogWarning($"Joint {rosJointName} has no mapping to Unity.");
            }
        }
    }

    void ConfigureArticulationBodies()
    {
        foreach (ArticulationBody joint in articulationBodies)
        {
            ArticulationDrive drive = joint.xDrive;
            drive.stiffness = 100000;  // High stiffness for position control
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