using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Sensor;

public class CamScript : MonoBehaviour
{
    ROSConnection m_Ros;
    Texture2D texRos;
    public GameObject displayPlane;
    Material planeMaterial;
    string webcamiagetopic = "/image_raw";

    void Start()
    {
        m_Ros = ROSConnection.GetOrCreateInstance();
        m_Ros.Subscribe<ImageMsg>(webcamiagetopic, ReceiveImage);
        planeMaterial = displayPlane.GetComponent<Renderer>().material;
    }

    public void ReceiveImage(ImageMsg imageMessage)
    {
        if (texRos == null)
        {
            texRos = new Texture2D((int)imageMessage.width, (int)imageMessage.height, TextureFormat.RGB24, false);
            planeMaterial.mainTexture = texRos;
        }

        texRos.LoadRawTextureData(imageMessage.data);
        texRos.Apply();
    }
}
