using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRGrabInteractable))]
public class GizmoWorkspaceSnap : MonoBehaviour
{
    [Header("References")]
    public RobotBase robotBase;        // Script that defines isOutOfWorkspace
    public Transform link6;            // Transform of the robot's link6

    [Header("Snap Configuration")]
    [Tooltip("Time in seconds to move to link6 (lerp).")]
    public float snapDuration = 0.2f;
    [Tooltip("Time in seconds that the gizmo remains at link6 before returning.")]
    public float returnDelay = 0.5f;

    private XRGrabInteractable grabInteractable;

    // Flags and poses
    private bool isGrabbed = false;
    private Vector3 lastInsidePosition;
    private Quaternion lastInsideRotation;

    private void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        grabInteractable.selectEntered.AddListener(OnGrab);
        grabInteractable.selectExited.AddListener(OnRelease);
    }

    private void OnDestroy()
    {
        grabInteractable.selectEntered.RemoveListener(OnGrab);
        grabInteractable.selectExited.RemoveListener(OnRelease);
    }

    private void Update()
    {
        // While grabbed and still INSIDE the workspace,
        // we keep updating the last valid pose.
        if (isGrabbed && robotBase != null && !robotBase.isOutOfWorkspace)
        {
            lastInsidePosition = transform.position;
            lastInsideRotation = transform.rotation;
        }
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        isGrabbed = true;
        // Initializes lastInside with the pose at the moment it was grabbed,
        // but it will be updated by Update() until it moves outside.
        lastInsidePosition = transform.position;
        lastInsideRotation = transform.rotation;
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        isGrabbed = false;

        // Only performs the snap if outside the workspace
        if (robotBase != null && robotBase.isOutOfWorkspace && link6 != null)
        {
            StopAllCoroutines();
            StartCoroutine(SnapAndReturn(lastInsidePosition, lastInsideRotation));
        }
    }

    private IEnumerator SnapAndReturn(Vector3 returnPos, Quaternion returnRot)
    {
        // 1) Move to link6
        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;
        float t = 0f;
        while (t < snapDuration)
        {
            t += Time.deltaTime;
            float n = Mathf.Clamp01(t / snapDuration);
            transform.position = Vector3.Lerp(startPos, link6.position, n);
            transform.rotation = Quaternion.Slerp(startRot, link6.rotation, n);
            yield return null;
        }
        transform.position = link6.position;
        transform.rotation = link6.rotation;

        // 2) Wait
        yield return new WaitForSeconds(returnDelay);

        // 3) Return to the last valid pose
        t = 0f;
        while (t < snapDuration)
        {
            t += Time.deltaTime;
            float n = Mathf.Clamp01(t / snapDuration);
            transform.position = Vector3.Lerp(link6.position, returnPos, n);
            transform.rotation = Quaternion.Slerp(link6.rotation, returnRot, n);
            yield return null;
        }
        transform.position = returnPos;
        transform.rotation = returnRot;
    }
}