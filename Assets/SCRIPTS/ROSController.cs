using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Sensor;
using System.Collections.Generic;

namespace Unity.Robotics.UrdfImporter.Control
{
    public class ROSController : MonoBehaviour
    {
        // All ArticulationBodies in the robot chain
        private ArticulationBody[] articulationChain;
        // Maps the joint name to its corresponding ArticulationBody
        private Dictionary<string, ArticulationBody> jointMap;

        // Drive parameters (you can adjust them according to your robot)
        public float stiffness = 1000f;
        public float damping = 100f;
        public float forceLimit = 100f;

        void Start()
        {
            // Initializes the joint chain (for example, gets all ArticulationBodies from the children of this GameObject)
            articulationChain = GetComponentsInChildren<ArticulationBody>();
            jointMap = new Dictionary<string, ArticulationBody>();

            // Configures each ArticulationBody and populates the dictionary
            foreach (ArticulationBody ab in articulationChain)
            {
                // Here, we assume that the GameObject name (or the ArticulationBody name itself) is the same as the joint name published in ROS.
                jointMap[ab.name] = ab;

                // Configures the drive parameters
                ArticulationDrive drive = ab.xDrive; // Adjust the axis if necessary (it can be yDrive or zDrive, depending on the joint)
                drive.forceLimit = forceLimit;
                drive.stiffness = stiffness;
                drive.damping = damping;
                ab.xDrive = drive;
            }

            // Subscribes to the /joint_states topic to receive joint states
            Debug.Log("ROSController: Subscribing to /joint_states...");
            ROSConnection.GetOrCreateInstance().Subscribe<JointStateMsg>("joint_states", JointStateCallback);
        }

        // Callback that is called whenever a JointState message is received
        void JointStateCallback(JointStateMsg msg)
        {
            // For each joint sent in the message, updates the drive of the corresponding ArticulationBody
            for (int i = 0; i < msg.name.Length; i++)
            {
                string jointName = msg.name[i];
                if (jointMap.ContainsKey(jointName))
                {
                    ArticulationBody ab = jointMap[jointName];
                    // Converts from radians to degrees (assuming the drive operates in degrees)
                    float targetAngle = (float)(msg.position[i] * Mathf.Rad2Deg);

                    // Updates the drive; adjust to the correct axis if it is not X
                    ArticulationDrive drive = ab.xDrive;
                    drive.target = targetAngle;
                    ab.xDrive = drive;

                    Debug.Log($"Joint {jointName} updated to: {targetAngle}°");
                }
                else
                {
                    Debug.LogWarning($"Joint {jointName} not found in the ArticulationBodies configuration.");
                }
            }
        }
    }
}