using UnityEngine;
using UnityEngine.InputSystem;

public class ControllerAnimator : MonoBehaviour
{
    [Header("Thumbstick")]
    [SerializeField] private Transform thumbstickTransform;
    [SerializeField] private Vector2 stickRotationRange = new Vector2(30f, 30f);

    [Header("Trigger")]
    [SerializeField] private Transform triggerTransform;
    [SerializeField] private Vector2 triggerRotationRange = new Vector2(0f, -15f);

    [Header("Grip")]
    [SerializeField] private Transform gripTransform;
    [SerializeField] private Vector2 gripPositionRange = new Vector2(-0.0125f, -0.011f);

    [Header("Input Actions")]
    public InputActionProperty thumbstickAction;
    public InputActionProperty triggerAction;
    public InputActionProperty gripAction;

    private void OnEnable()
    {
        thumbstickAction.action?.Enable();
        triggerAction.action?.Enable();
        gripAction.action?.Enable();
    }

    private void OnDisable()
    {
        thumbstickAction.action?.Disable();
        triggerAction.action?.Disable();
        gripAction.action?.Disable();
    }

    private void Update()
    {
        if (thumbstickTransform != null && thumbstickAction.action != null)
        {
            Vector2 stickValue = thumbstickAction.action.ReadValue<Vector2>();
            thumbstickTransform.localRotation = Quaternion.Euler(
                -stickValue.y * stickRotationRange.x,
                0f,
                -stickValue.x * stickRotationRange.y
            );
        }

        if (triggerTransform != null && triggerAction.action != null)
        {
            float triggerValue = triggerAction.action.ReadValue<float>();
            triggerTransform.localRotation = Quaternion.Euler(
                Mathf.Lerp(triggerRotationRange.x, triggerRotationRange.y, triggerValue),
                0f,
                0f
            );
        }

        if (gripTransform != null && gripAction.action != null)
        {
            float gripValue = gripAction.action.ReadValue<float>();
            Vector3 currentPos = gripTransform.localPosition;
            gripTransform.localPosition = new Vector3(
                Mathf.Lerp(gripPositionRange.x, gripPositionRange.y, gripValue),
                currentPos.y,
                currentPos.z
            );
        }
    }
}
