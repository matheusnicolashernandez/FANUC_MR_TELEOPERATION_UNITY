using UnityEngine;
using System.Collections.Generic;


public class ChangeColorInvalidPose : MonoBehaviour
{
    [Header("References")]
    public Transform Link6;
    public Transform GizmoObject;
    public Renderer[] robotRenderers;

    [Header("Material")]
    public Material invalidPoseMaterial;

    [Header("Configuration")]
    public float toleranceEnter = 0.03f;  // distance at which the pose starts to become invalid
    public float toleranceExit = 0.025f;  // distance at which the pose returns to normal

    [Header("Smoothing")]
    public int normalSmoothFrames = 5;    // normal moving average
    public int grabSmoothFrames = 35;     // moving average during grab
    [Tooltip("Flag that should be enabled during grab or sudden movement for additional smoothing.")]
    public bool isGrabbed = false;

    // Internal state
    private Material[][] originalMaterialsBackup;
    private bool isInvalidPose = false;
    private Queue<float> distanceHistory = new Queue<float>();

    void Start()
    {
        if (robotRenderers == null || robotRenderers.Length == 0)
            robotRenderers = GetComponentsInChildren<Renderer>(true);

        // Backup of the original materials
        originalMaterialsBackup = new Material[robotRenderers.Length][];
        for (int i = 0; i < robotRenderers.Length; i++)
        {
            if (robotRenderers[i] != null)
                originalMaterialsBackup[i] = robotRenderers[i].sharedMaterials;
        }
    }

    void Update()
    {
        if (Link6 == null || GizmoObject == null) return;

        // Current distance
        float dist = Vector3.Distance(Link6.position, GizmoObject.position);

        // Adaptive smoothing
        int currentSmoothFrames = isGrabbed ? grabSmoothFrames : normalSmoothFrames;
        distanceHistory.Enqueue(dist);
        if (distanceHistory.Count > currentSmoothFrames) distanceHistory.Dequeue();

        float avgDist = 0f;
        foreach (var d in distanceHistory) avgDist += d;
        avgDist /= distanceHistory.Count;

        // Hysteresis: prevents flickering by using different thresholds
        bool shouldBeInvalid = false;
        if (!isInvalidPose)
            shouldBeInvalid = avgDist > toleranceEnter;
        else
            shouldBeInvalid = avgDist > toleranceExit;

        if (shouldBeInvalid != isInvalidPose)
        {
            SetInvalidPose(shouldBeInvalid);
        }
    }

    private void SetInvalidPose(bool invalid)
    {
        isInvalidPose = invalid;

        for (int i = 0; i < robotRenderers.Length; i++)
        {
            var rend = robotRenderers[i];
            if (rend == null) continue;

            if (invalid && invalidPoseMaterial != null)
            {
                Material[] arr = new Material[rend.sharedMaterials.Length];
                for (int k = 0; k < arr.Length; k++) arr[k] = invalidPoseMaterial;
                rend.sharedMaterials = arr;
            }
            else
            {
                if (originalMaterialsBackup[i] != null)
                    rend.sharedMaterials = originalMaterialsBackup[i];
            }
        }
    }

    void OnDrawGizmos()
    {
        if (Link6 == null || GizmoObject == null) return;

        Gizmos.color = isInvalidPose ? Color.red : Color.green;
        Gizmos.DrawLine(Link6.position, GizmoObject.position);

        // Tolerance sphere for visualization
        Gizmos.color = new Color(1f, 1f, 0f, 0.25f);
        Gizmos.DrawSphere(Link6.position, toleranceEnter);
    }
}