using UnityEngine;

[RequireComponent(typeof(ArticulationBody))]
public class Follow_Gripper : MonoBehaviour
{
    [SerializeField] private Transform target;

    [Header("Position Offset (Target Local Space)")]
    public Vector3 positionOffset = Vector3.zero;

    [Header("Rotation Offset (Degrees, Target Local Space)")]
    public Vector3 rotationOffsetEuler = Vector3.zero;

    [Header("Additional Options")]
    public bool disableGravity = true;
    public bool disableColliders = true;

    private ArticulationBody ab;

    void Start()
    {
        ab = GetComponent<ArticulationBody>();
        ab.immovable = false;

        ab.jointType = ArticulationJointType.FixedJoint;
        ab.linearLockX = ArticulationDofLock.FreeMotion;
        ab.linearLockY = ArticulationDofLock.FreeMotion;
        ab.linearLockZ = ArticulationDofLock.FreeMotion;
        ab.twistLock = ArticulationDofLock.FreeMotion;
        ab.swingYLock = ArticulationDofLock.FreeMotion;
        ab.swingZLock = ArticulationDofLock.FreeMotion;

        if (disableGravity)
            ab.useGravity = false;

        if (disableColliders)
        {
            foreach (var col in GetComponentsInChildren<Collider>())
                col.enabled = false;
        }

        var drive = ab.xDrive;
        drive.forceLimit = float.MaxValue;
        drive.stiffness = float.MaxValue;
        drive.damping = 0f;
        ab.xDrive = drive;
    }

    void FixedUpdate()
    {
        if (target == null) return;

        // Position offset in the target's local space
        Vector3 desiredPosition = target.TransformPoint(positionOffset);

        // Rotation offset — applied in the target's LOCAL space
        Quaternion localRotationOffset = Quaternion.Euler(rotationOffsetEuler);
        Quaternion desiredRotation = target.rotation * localRotationOffset;

        // Direct application (no delay, no smoothing)
        ab.TeleportRoot(desiredPosition, desiredRotation);
        ab.velocity = Vector3.zero;
        ab.angularVelocity = Vector3.zero;
    }
}