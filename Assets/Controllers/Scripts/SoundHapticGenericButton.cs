using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class SoundHapticGenericButton : MonoBehaviour
{
    [Header("Input (Escolha o botão desejado)")]
    public InputActionProperty buttonAction;
    public XRBaseController controller;

    [Header("Feedback Sonoro")]
    public AudioSource firstPressAudio;     // Som ao apertar pela primeira vez
    public AudioSource secondPressAudio;    // Som ao apertar novamente (toggle)

    [Header("Feedback Háptico")]
    public float hapticIntensity = 0.2f;
    public float hapticDuration = 0.15f;

    private bool lastButtonState = false;
    private bool toggleState = false;

    void OnEnable()
    {
        if (buttonAction.action != null)
            buttonAction.action.Enable();
    }

    void OnDisable()
    {
        if (buttonAction.action != null)
            buttonAction.action.Disable();
    }

    void Update()
    {
        if (buttonAction.action == null) return;

        bool isPressed = buttonAction.action.ReadValue<float>() > 0.5f;

        if (isPressed && !lastButtonState)
        {
            // Botão foi pressionado (toggle)
            toggleState = !toggleState;

            if (controller != null)
                controller.SendHapticImpulse(hapticIntensity, hapticDuration);

            if (toggleState)
            {
                if (firstPressAudio != null)
                    firstPressAudio.Play();
            }
            else
            {
                if (secondPressAudio != null)
                    secondPressAudio.Play();
            }
        }

        lastButtonState = isPressed;
    }
}
