using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Std;

public class RosLatencyMonitor : MonoBehaviour
{
    ROSConnection ros;

    string pingTopic = "/latency_ping";
    string pongTopic = "/latency_pong";

    float lastPingTime;

    // média exponencial
    float avgLatencyMs = 0f;

    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();

        ros.RegisterPublisher<Float32Msg>(pingTopic);
        ros.Subscribe<Float32Msg>(pongTopic, OnPongReceived);

        InvokeRepeating(nameof(SendPing), 1f, 0.1f); // 10 Hz
    }

    void SendPing()
    {
        lastPingTime = Time.realtimeSinceStartup;

        ros.Publish(pingTopic, new Float32Msg(lastPingTime));
    }

    void OnPongReceived(Float32Msg msg)
    {
        float now = Time.realtimeSinceStartup;

        float rtt = now - msg.data;
        float latency = rtt / 2f;

        // 🔥 converter para ms
        float rtt_ms = rtt * 1000f;
        float latency_ms = latency * 1000f;

        // 🔥 média exponencial (suaviza ruído)
        avgLatencyMs = avgLatencyMs * 0.9f + latency_ms * 0.1f;

        Debug.Log($"[LATÊNCIA] {latency_ms:F1} ms | média: {avgLatencyMs:F1} ms | RTT: {rtt_ms:F1} ms");
    }
}