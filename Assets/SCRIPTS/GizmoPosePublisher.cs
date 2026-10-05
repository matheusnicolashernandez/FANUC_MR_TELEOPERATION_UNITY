using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Geometry;
using RosMessageTypes.Std;
using RosMessageTypes.BuiltinInterfaces;

public class GizmoPosePublisher : MonoBehaviour
{
    private ROSConnection ros;
    public string topicName = "/unity_pose";

    // Frequência de publicação
    public float publishRate = 90f;
    private float publishTimer = 0f;

    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<PoseStampedMsg>(topicName);
    }

    void Update()
    {
        // Controla a frequência de publicação
        publishTimer += Time.deltaTime;

        if (publishTimer < 1f / publishRate)
            return;

        publishTimer = 0f;

        // Conversão da posição (mesma transformação T: (z, -x, y))
        Vector3 unityPos = transform.position;
        Vector3 rosPos = new Vector3(unityPos.z, -unityPos.x, unityPos.y);

        // Para a orientação, aplicamos a mesma transformação aos vetores forward e up.
        Vector3 unityForward = transform.forward;
        Vector3 unityUp = transform.up;

        // Aplicando T(v) = (v.z, -v.x, v.y)
        Vector3 rosForward = new Vector3(unityForward.z, -unityForward.x, unityForward.y);
        Vector3 rosUp = new Vector3(unityUp.z, -unityUp.x, unityUp.y);

        // Reconstrói a rotação para ROS usando os vetores transformados
        Quaternion rosRot = Quaternion.LookRotation(rosForward, rosUp);

        // Cria a mensagem PoseStamped para o ROS
        PoseStampedMsg poseMsg = new PoseStampedMsg();
        poseMsg.header = new HeaderMsg();

        poseMsg.header.stamp = new TimeMsg((uint)Time.time, 0);
        poseMsg.header.frame_id = "base_link";

        // Define a posição convertida
        poseMsg.pose.position.x = rosPos.x;
        poseMsg.pose.position.y = rosPos.y;
        poseMsg.pose.position.z = rosPos.z;

        // Define a orientação convertida
        poseMsg.pose.orientation.x = rosRot.x;
        poseMsg.pose.orientation.y = rosRot.y;
        poseMsg.pose.orientation.z = rosRot.z;
        poseMsg.pose.orientation.w = rosRot.w;

        // Publica a mensagem no tópico ROS
        ros.Send(topicName, poseMsg);
    }
}
