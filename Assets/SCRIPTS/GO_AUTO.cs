using UnityEngine;
using System.Collections;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Std;  // for EmptyMsg
using RosMessageTypes.Std;  // in case plan/execute services are also used

public class GO_AUTO : MonoBehaviour
{
    ROSConnection ros;

    [Header("GO Topic")]
    [Tooltip("Topic name for the GO command")]
    public string goTopicName = "/go_trajectory_cmd";

    [Tooltip("GO frequency (Hz)")]
    public float goRate = 10f;

    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();

        // **Only** registers the EmptyMsg publisher for GO
        ros.RegisterPublisher<EmptyMsg>(goTopicName);

        // Starts continuous GO publishing
        StartCoroutine(GoLoop());
    }

    IEnumerator GoLoop()
    {
        var wait = new WaitForSeconds(1f / goRate);
        var empty = new EmptyMsg();

        while (true)
        {
            // Publishes the GO command and does nothing else
            ros.Publish(goTopicName, empty);
            yield return wait;
        }
    }
}