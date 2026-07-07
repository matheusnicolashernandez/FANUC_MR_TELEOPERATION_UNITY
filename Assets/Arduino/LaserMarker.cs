using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Std;

public class LaserMarker : MonoBehaviour
{
    [Header("ROS")]
    [SerializeField] string topicName = "tof_distance";
    [Tooltip("Factor used to convert centimeters into scale units (optional)")]
    [SerializeField] float scaleFactor = 0.01f;

    [Header("Smoothing")]
    [Tooltip("Minimum difference in raw units required to trigger an update (prevents small fluctuations)")]
    [SerializeField] float noiseThreshold = 0.01f;

    [Header("Distance Limiter")]
    [Tooltip("Maximum distance in cm before limiting the scale")]
    [SerializeField] float maxDistanceCm = 100f;

    [Header("Growth Reference")]
    [Tooltip("Empty Transform positioned at the bottom face of the cylinder (growth origin)")]
    [SerializeField] Transform faceOrigin;

    [Header("Material Change by Value Range")]
    [Tooltip("Default beam material")]
    [SerializeField] Material defaultMaterial;
    [Tooltip("Alternate material when within the specified range")]
    [SerializeField] Material alternateMaterial;
    [Tooltip("Lower limit in cm for changing the material")]
    [SerializeField] float minThresholdCm = 2.5f;
    [Tooltip("Upper limit in cm for changing the material")]
    [SerializeField] float maxThresholdCm = 3.5f;

    ROSConnection ros;
    MeshFilter meshFilter;
    MeshRenderer meshRenderer;
    float meshHeight;
    float previousScaleY;

    void Start()
    {
        if (faceOrigin == null)
        {
            Debug.LogError("You must assign a faceOrigin!");
            enabled = false;
            return;
        }

        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
        meshHeight = meshFilter.mesh.bounds.size.y;
        previousScaleY = transform.localScale.y;

        ros = ROSConnection.GetOrCreateInstance();
        ros.Subscribe<Float32Msg>(topicName, UpdateScaleFromROS);
        Debug.Log($"[ROS] Subscribed to '{topicName}'");
    }

    void UpdateScaleFromROS(Float32Msg msg)
    {
        float rawCm = msg.data;

        // Apply the distance limiter
        float limitedCm = Mathf.Min(rawCm, maxDistanceCm);

        float targetScaleY = Mathf.Max(0.01f, limitedCm * scaleFactor);

        if (Mathf.Abs(targetScaleY - previousScaleY) < noiseThreshold)
        {
            targetScaleY = previousScaleY;
        }

        float newScaleY = targetScaleY;

        Vector3 newScale = transform.localScale;
        newScale.y = newScaleY;
        transform.localScale = newScale;

        float halfHeight = (meshHeight * newScaleY) * 0.5f;
        Vector3 worldUp = faceOrigin.TransformDirection(Vector3.up);
        transform.position = faceOrigin.position + worldUp * halfHeight;

        previousScaleY = newScaleY;

        // Change the material when the value is within the specified range
        if (rawCm >= minThresholdCm && rawCm <= maxThresholdCm)
        {
            meshRenderer.material = alternateMaterial;
        }
        else
        {
            meshRenderer.material = defaultMaterial;
        }

        Debug.Log($"[ROS] {rawCm:F1} cm (limited: {limitedCm:F1}) → Scale Y={newScaleY:F3}, HalfHeight={halfHeight:F3}");
    }
}