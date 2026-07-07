using UnityEngine;
using System.Collections;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Std;
using System.Collections.Generic;
using UnityEngine.XR.Content.Interaction;

public class SmoothContinuousROS : MonoBehaviour
{
    private ROSConnection ros;

    [Header("ROS Services")]
    public string planServiceName = "/plan_trajectory";
    public string executeServiceName = "/execute_trajectory";

    [Header("Parameters")]
    public float planningInterval = 0.1f; // Planning frequency
    public float executionInterval = 0.05f; // Execution frequency
    public int maxQueueSize = 5; // Maximum number of stored trajectories

    [Header("Target Gizmo")]
    public Transform targetGizmo;

    [Header("XR Button")]
    public XRGripButton button;

    private bool isReactiveActive = false;
    private Coroutine planningCoroutine;
    private Coroutine executionCoroutine;
    private Queue<EmptyRequest> trajectoryQueue = new Queue<EmptyRequest>();

    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterRosService<EmptyRequest, EmptyResponse>(planServiceName);
        ros.RegisterRosService<EmptyRequest, EmptyResponse>(executeServiceName);

        if (button != null)
            button.onPress.AddListener(ToggleReactiveMode);
        else
            Debug.LogError("XRGripButton was not assigned!");

        if (targetGizmo == null)
            targetGizmo = transform;
    }

    public void ToggleReactiveMode()
    {
        isReactiveActive = !isReactiveActive;
        Debug.Log("Reactive mode " + (isReactiveActive ? "enabled" : "disabled"));

        if (isReactiveActive)
        {
            if (planningCoroutine == null)
                planningCoroutine = StartCoroutine(PlanningLoop());

            if (executionCoroutine == null)
                executionCoroutine = StartCoroutine(ExecutionLoop());
        }
        else
        {
            if (planningCoroutine != null)
            {
                StopCoroutine(planningCoroutine);
                planningCoroutine = null;
            }
            if (executionCoroutine != null)
            {
                StopCoroutine(executionCoroutine);
                executionCoroutine = null;
            }
        }
    }

    IEnumerator PlanningLoop()
    {
        while (isReactiveActive)
        {
            if (trajectoryQueue.Count < maxQueueSize)
            {
                Debug.Log("Requesting new plan...");
                ros.SendServiceMessage<EmptyResponse>(
                    planServiceName,
                    new EmptyRequest(),
                    (response) =>
                    {
                        Debug.Log("New plan received and stored!");
                        trajectoryQueue.Enqueue(new EmptyRequest());
                    }
                );
            }
            yield return new WaitForSeconds(planningInterval);
        }
    }

    IEnumerator ExecutionLoop()
    {
        while (isReactiveActive)
        {
            if (trajectoryQueue.Count > 0)
            {
                EmptyRequest nextTrajectory = trajectoryQueue.Dequeue();
                Debug.Log("Executing trajectory...");
                ros.SendServiceMessage<EmptyResponse>(
                    executeServiceName,
                    nextTrajectory,
                    (response) =>
                    {
                        Debug.Log("Execution completed.");
                    }
                );
            }
            yield return new WaitForSeconds(executionInterval);
        }
    }
}