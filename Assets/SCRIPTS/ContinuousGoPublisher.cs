using UnityEngine;
using System.Collections;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Std;

public class ContinuousGoPublisher : MonoBehaviour
{
    ROSConnection ros;
    public string goTopicName = "/go_trajectory_cmd";
    public float goRate = 10f;

    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<EmptyMsg>(goTopicName);
        StartCoroutine(GoLoop());
    }

    IEnumerator GoLoop()
    {
        var wait = new WaitForSeconds(1f / goRate);
        var empty = new EmptyMsg();
        while (true)
        {
            ros.Publish(goTopicName, empty);
            yield return wait;
        }
    }
}
