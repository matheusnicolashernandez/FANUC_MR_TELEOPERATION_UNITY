using System;
using Unity.Robotics;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Std;

namespace Unity.Robotics.UrdfImporter.Control
{
    public class RobotJointController : MonoBehaviour
    {
        private ArticulationBody[] articulationChain;
        private ROSConnection ros;
        public string jointTopicName = "joint_unity";

        void Start()
        {
            articulationChain = this.GetComponentsInChildren<ArticulationBody>();
            ros = ROSConnection.GetOrCreateInstance();
            ros.RegisterPublisher<Float32MultiArrayMsg>(jointTopicName); // Registers the publisher for the new message
        }

        void Update()
        {
            // Sends the current angles to ROS
            SendJointAnglesToRos();
        }

        void SendJointAnglesToRos()
        {
            // Creates an array to store the joint angles
            float[] jointAngles = new float[articulationChain.Length - 1]; // Subtracts one for the first joint

            // Gets the current angle of each joint, starting from index 1
            for (int i = 1; i < articulationChain.Length; i++)
            {
                jointAngles[i - 1] = articulationChain[i].jointPosition[0]; // Uses jointPosition to get the current angle
            }

            // Creates the array layout (can be null if not necessary)
            var layout = new MultiArrayLayoutMsg(); // Creates an empty layout instance

            // Creates the message with the joint angles
            Float32MultiArrayMsg jointAnglesMsg = new Float32MultiArrayMsg(layout, jointAngles);

            // Publishes the message to the specified topic
            ros.Publish(jointTopicName, jointAnglesMsg);
        }
    }
}