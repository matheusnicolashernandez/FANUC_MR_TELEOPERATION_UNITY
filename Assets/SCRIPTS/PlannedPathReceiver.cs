using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Moveit;
using System.Collections;
using System.Collections.Generic;

public class PlannedPathReceiver : MonoBehaviour
{
    private ArticulationBody[] articulationBodies;
    private List<JointTrajectoryPoint> trajectoryPoints;
    private Dictionary<string, ArticulationBody> rosToUnityMap;
    private Dictionary<string, string> rosToUnityJointMapping;
    private ROSConnection rosConnection;

    private bool firstTrajectoryReceived = false;
    private bool isMoving = false;
    private bool isVisible = false;

    private Coroutine visibilityCoroutine; // 👈 Stores the Coroutine

    [Header("Automatic Mode Active")]
    public bool isAutoModeEnabled = false;

    public bool IsRobotVisible => isVisible;

    void Start()
    {
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

        foreach (ArticulationBody articulationBody in articulationBodies)
        {
            rosToUnityMap[articulationBody.name] = articulationBody;
        }

        rosConnection = ROSConnection.GetOrCreateInstance();
        rosConnection.Subscribe<DisplayTrajectoryMsg>("/move_group/display_planned_path", HandlePlannedPath);

        trajectoryPoints = new List<JointTrajectoryPoint>();
        SetRobotVisibility(false);
    }

    private void HandlePlannedPath(DisplayTrajectoryMsg displayTrajectory)
    {
        if (isMoving || isAutoModeEnabled)
        {
            Debug.Log("🚫 Trajectory ignored (moving or automatic mode enabled).");
            return;
        }

        trajectoryPoints.Clear();

        foreach (var trajectory in displayTrajectory.trajectory)
        {
            foreach (var rosPoint in trajectory.joint_trajectory.points)
            {
                int numJoints = rosPoint.positions.Length;
                JointTrajectoryPoint point = new JointTrajectoryPoint(numJoints);
                point.positions = System.Array.ConvertAll(rosPoint.positions, item => (float)item);
                point.velocities = System.Array.ConvertAll(rosPoint.velocities, item => (float)item);
                trajectoryPoints.Add(point);
            }
        }

        if (!firstTrajectoryReceived)
        {
            firstTrajectoryReceived = true;
            Debug.Log("📦 First trajectory received; movement ignored.");
            return;
        }

        if (!TrajectoryHasMovement())
        {
            Debug.Log("❌ Trajectory does not contain actual movement. Ignored.");
            SetRobotVisibility(false);
            return;
        }

        Debug.Log("✅ Starting robot movement along the trajectory.");
        StartCoroutine(MoveRobotAlongTrajectory());
    }

    private bool TrajectoryHasMovement()
    {
        if (trajectoryPoints.Count < 2) return false;

        JointTrajectoryPoint start = trajectoryPoints[0];
        JointTrajectoryPoint end = trajectoryPoints[trajectoryPoints.Count - 1];

        for (int i = 0; i < start.positions.Length; i++)
        {
            if (Mathf.Abs(start.positions[i] - end.positions[i]) > 0.001f)
                return true;
        }

        return false;
    }

    private IEnumerator MoveRobotAlongTrajectory()
    {
        isMoving = true;

        // 👁️ Starts the coroutine that makes the robot visible after 0.1s
        visibilityCoroutine = StartCoroutine(ShowRobotAfterDelay(0.1f));

        float fastDuration = 0.0013f;
        float fastMoveTime = 0.0000001f;
        float normalMoveTime = 0.07f;

        float totalElapsed = 0f;

        for (int i = 0; i < trajectoryPoints.Count - 1; i++)
        {
            JointTrajectoryPoint currentPoint = trajectoryPoints[i];
            JointTrajectoryPoint targetPoint = trajectoryPoints[i + 1];
            float elapsedTime = 0f;

            float moveTime = (totalElapsed < fastDuration) ? fastMoveTime : normalMoveTime;

            while (elapsedTime < moveTime)
            {
                elapsedTime += Time.deltaTime;
                totalElapsed += Time.deltaTime;
                float t = elapsedTime / moveTime;

                for (int j = 0; j < currentPoint.positions.Length; j++)
                {
                    float jointPosition = Mathf.Lerp(currentPoint.positions[j], targetPoint.positions[j], t);
                    ApplyJointPosition(j, jointPosition);
                }

                yield return null;
            }
        }

        // 🧹 Ensures that the robot does not appear after the movement has already ended
        if (visibilityCoroutine != null)
        {
            StopCoroutine(visibilityCoroutine);
            visibilityCoroutine = null;

            // 👁️ Shows the robot for 1 frame if it never became visible
            if (!isVisible)
            {
                SetRobotVisibility(true);
                yield return null; // shows for 1 frame
            }
        }

        SetRobotVisibility(false);
        isMoving = false;
        Debug.Log("🏁 Trajectory completed. Robot hidden.");
    }

    private IEnumerator ShowRobotAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SetRobotVisibility(true);
        Debug.Log("👁️ Robot is now visible after the delay.");
    }

    private void ApplyJointPosition(int jointIndex, float jointPosition)
    {
        string rosJointName = $"joint_{jointIndex + 1}";

        if (rosToUnityJointMapping.TryGetValue(rosJointName, out string unityJointName) &&
            rosToUnityMap.TryGetValue(unityJointName, out ArticulationBody unityJoint))
        {
            float jointPositionDegrees = jointPosition * Mathf.Rad2Deg;

            if (rosJointName == "joint_3" || rosJointName == "joint_4" || rosJointName == "joint_6")
            {
                jointPositionDegrees *= -1;
            }

            ArticulationDrive drive = unityJoint.xDrive;
            drive.target = jointPositionDegrees;
            unityJoint.xDrive = drive;
        }
    }

    void ConfigureArticulationBodies()
    {
        foreach (ArticulationBody joint in articulationBodies)
        {
            ArticulationDrive drive = joint.xDrive;
            drive.stiffness = 100000;
            drive.damping = 100;
            drive.forceLimit = 1000;
            joint.xDrive = drive;
        }
    }

    void Update()
    {
        ConfigureArticulationBodies();
    }

    private void SetRobotVisibility(bool visible)
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer rend in renderers)
        {
            rend.enabled = visible;
        }

        isVisible = visible;
        Debug.Log($"[PlannedPathReceiver] Render set to: {visible}");
    }

    public void SetRenderersActive(bool active)
    {
        SetRobotVisibility(active);
    }

    public bool IsRobotMoving()
    {
        return isMoving;
    }
}