using UnityEngine;

public class KnobFollower : MonoBehaviour
{
    public Transform knob; // Reference to the actual Knob
    public float followSpeed = 99999999f; // Smoothing speed
    private Vector3 initialKnobPosition;
    private Quaternion initialKnobRotation;
    private Vector3 initialPanelPosition;
    private Quaternion initialPanelRotation;

    void Start()
    {
        if (knob != null)
        {
            initialKnobPosition = knob.position; // Saves the initial Knob position
            initialKnobRotation = knob.rotation; // Saves the initial Knob rotation
            initialPanelPosition = transform.position; // Saves the initial panel position
            initialPanelRotation = transform.rotation; // Saves the initial panel rotation
        }
    }

    void Update()
    {
        if (knob != null)
        {
            // Calculates the rotation and position difference between the panel and the knob
            Vector3 deltaPosition = transform.position - initialPanelPosition;
            Quaternion deltaRotation = transform.rotation * Quaternion.Inverse(initialPanelRotation);

            // Applies smoothing to the position
            knob.position = Vector3.Lerp(knob.position, initialKnobPosition + deltaPosition, Time.deltaTime * followSpeed);

            // Applies smoothing to the rotation
            knob.rotation = Quaternion.Slerp(knob.rotation, initialKnobRotation * deltaRotation, Time.deltaTime * followSpeed);
        }
    }














}