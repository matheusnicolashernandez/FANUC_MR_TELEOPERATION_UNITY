using UnityEngine;
using System.Collections;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Std;

public class ContinuousGoAuto : MonoBehaviour
{
    private ROSConnection ros;

    [Header("ROS Service")]
    public string goServiceName = "/go_trajectory";

    [Header("Parameters")]
    public float callInterval = 0.5f; // time between calls

    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterRosService<EmptyRequest, EmptyResponse>(goServiceName);
        StartCoroutine(GoLoop());
        Debug.Log("Continuous GO Auto started with continuous flow");
    }

    IEnumerator GoLoop()
    {
        var wait = new WaitForSeconds(callInterval);

        while (true)
        {
            Debug.Log("Calling GO service...");
            bool responseReceived = false;

            ros.SendServiceMessage<EmptyResponse>(
                goServiceName,
                new EmptyRequest(),
                (response) =>
                {
                    Debug.Log("GO response received.");
                    responseReceived = true;
                }
            );

            float timeout = 1.0f;
            float elapsed = 0f;

            while (!responseReceived && elapsed < timeout)
            {
                yield return null;
                elapsed += Time.deltaTime;
            }

            if (!responseReceived)
            {
                Debug.LogWarning("No response from GO service after 1 second.");
            }

            yield return wait;
        }
    }
}