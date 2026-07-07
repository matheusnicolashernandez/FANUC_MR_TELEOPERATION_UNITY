using UnityEngine;

[RequireComponent(typeof(ArticulationBody))]
public class RigidFollower : MonoBehaviour
{
    [SerializeField] private Transform target;

    [Header("Position Offset (Relative to Target Local Space)")]
    public Vector3 positionOffset = Vector3.zero;

    [Header("Rotation Offset (Degrees, Relative to Target Local Space)")]
    public Vector3 rotationOffsetEuler = Vector3.zero;

    [Header("Additional Options")]
    public bool disableGravity = true;
    public bool disableColliders = true;

    private ArticulationBody ab;
    private Quaternion rotationOffset;

    void Start()
    {
        ab = GetComponent<ArticulationBody>();
        ab.immovable = false;

        // Movement settings
        ab.jointType = ArticulationJointType.FixedJoint;
        ab.linearLockX = ArticulationDofLock.FreeMotion;
        ab.linearLockY = ArticulationDofLock.FreeMotion;
        ab.linearLockZ = ArticulationDofLock.FreeMotion;
        ab.twistLock = ArticulationDofLock.FreeMotion;
        ab.swingYLock = ArticulationDofLock.FreeMotion;
        ab.swingZLock = ArticulationDofLock.FreeMotion;

        // Disables gravity if necessary
        if (disableGravity)
            ab.useGravity = false;

        // Disables colliders if enabled
        if (disableColliders)
        {
            foreach (var col in GetComponentsInChildren<Collider>())
                col.enabled = false;
        }

        // Resets forces (drives are not used in this case, but maximum force is ensured if they are enabled)
        ArticulationDrive drive = ab.xDrive;
        drive.forceLimit = float.MaxValue;
        drive.stiffness = float.MaxValue;
        drive.damping = 0f;
        ab.xDrive = drive;

        rotationOffset = Quaternion.Euler(rotationOffsetEuler);
    }

    void FixedUpdate()
    {
        if (target == null) return;

        // Calculates the final position and rotation with offsets
        Vector3 finalPosition = target.TransformPoint(positionOffset);
        Quaternion finalRotation = target.rotation * rotationOffset;

        // Teleports instantly
        ab.TeleportRoot(finalPosition, finalRotation);

        // Resets any residual physical motion
        ab.velocity = Vector3.zero;
        ab.angularVelocity = Vector3.zero;
    }
}