using UnityEngine;
using System.Collections;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Std;
using UnityEngine.XR.Content.Interaction;

public class ToggleAutoPlanExecuteROS : MonoBehaviour
{
    private ROSConnection ros;

    [Header("ROS Services")]
    public string planServiceName = "/plan_trajectory";
    public string executeServiceName = "/execute_trajectory";

    [Header("Loop Parameters")]
    [Tooltip("Interval (in seconds) between each planning and execution cycle.")]
    public float updateInterval = 1.0f;

    [Header("Collision Detection")]
    [Tooltip("Array containing the objects with the ChangeColorOnCollision script.")]
    public ChangeColorOnCollision[] collisionDetectors;

    [Header("XR Button")]
    [Tooltip("Reference to the XRGripButton that will trigger the toggle.")]
    public XRGripButton button;

    private Coroutine autoLoopCoroutine = null;
    private bool isAutoLoopRunning = false;

    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterRosService<EmptyRequest, EmptyResponse>(planServiceName);
        ros.RegisterRosService<EmptyRequest, EmptyResponse>(executeServiceName);

        if (button != null)
            button.onPress.AddListener(ToggleAutoPlanExecute);
        else
            Debug.LogError("XRGripButton was not assigned to the ToggleAutoPlanExecuteROS script!");
    }

    /// <summary>
    /// Toggles between starting and stopping the automatic planning/execution loop.
    /// </summary>
    public void ToggleAutoPlanExecute()
    {
        if (isAutoLoopRunning)
            StopAutoLoop();
        else
            StartAutoLoop();
    }

    private void StartAutoLoop()
    {
        Debug.Log("Starting Auto Plan/Execute loop...");
        autoLoopCoroutine = StartCoroutine(AutoPlanExecuteLoop());
        isAutoLoopRunning = true;
    }

    private void StopAutoLoop()
    {
        Debug.Log("Stopping Auto Plan/Execute loop...");
        if (autoLoopCoroutine != null)
        {
            StopCoroutine(autoLoopCoroutine);
            autoLoopCoroutine = null;
        }
        isAutoLoopRunning = false;
    }

    IEnumerator AutoPlanExecuteLoop()
    {
        while (true)
        {
            // Checks whether there is a collision in any of the monitored objects
            bool collisionActive = false;
            if (collisionDetectors != null)
            {
                foreach (ChangeColorOnCollision detector in collisionDetectors)
                {
                    if (detector != null && detector.isColliding)
                    {
                        Debug.Log("Loop aborted: active collision in " + detector.gameObject.name);
                        collisionActive = true;
                        break;
                    }
                }
            }

            if (!collisionActive)
            {
                // Calls the planning service
                Debug.Log("Calling planning service...");
                bool planDone = false;
                ros.SendServiceMessage<EmptyResponse>(
                    planServiceName,
                    new EmptyRequest(),
                    (response) =>
                    {
                        Debug.Log("Planning response received.");
                        planDone = true;
                    }
                );
                yield return new WaitUntil(() => planDone);

                // Calls the execution service
                Debug.Log("Calling execution service...");
                bool executeDone = false;
                ros.SendServiceMessage<EmptyResponse>(
                    executeServiceName,
                    new EmptyRequest(),
                    (response) =>
                    {
                        Debug.Log("Execution response received.");
                        executeDone = true;
                    }
                );
                yield return new WaitUntil(() => executeDone);
            }
            else
            {
                Debug.Log("Cycle skipped due to active collision.");
            }

            yield return new WaitForSeconds(updateInterval);
        }
    }
}