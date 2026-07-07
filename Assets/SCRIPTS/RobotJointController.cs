using UnityEngine;
using UnityEngine.UI;

public class RobotJointController : MonoBehaviour
{
    public RevoluteRobot robot;  // Reference to the robot script
    public Slider[] jointSliders; // Array of sliders for each joint

    void Start()
    {
        if (robot == null || jointSliders.Length != robot.Links.Count)
        {
            Debug.LogError("Error: Make sure all sliders are assigned correctly!");
            return;
        }

        // Initializes the sliders with the current joint values
        for (int i = 0; i < jointSliders.Length; i++)
        {
            int jointIndex = i; // To avoid problems with closures
            jointSliders[i].minValue = (float)robot.jointLimits[i].MinAngle;
            jointSliders[i].maxValue = (float)robot.jointLimits[i].MaxAngle;
            jointSliders[i].value = (float)robot.ForwardKinematicsQ[i];

            // Adds a listener to capture slider changes
            jointSliders[i].onValueChanged.AddListener(value => UpdateJoint(jointIndex, value));
        }
    }

    void UpdateJoint(int jointIndex, float value)
    {
        if (robot.ForwardKinematicsQ != null && jointIndex < robot.ForwardKinematicsQ.Length)
        {
            robot.ForwardKinematicsQ[jointIndex] = value;
            robot.ComputeForwardKinematics(); // Updates the robot position with the new values
        }
    }
}