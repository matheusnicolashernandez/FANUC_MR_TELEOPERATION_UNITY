using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Std;
using TMPro;
using UnityEngine.XR.Interaction.Toolkit;

public class AutoModeToggle : MonoBehaviour
{
    [Header("XR Input")]
    public InputActionProperty yButtonAction;
    public XRBaseController leftHandController;

    [Header("ROS")]
    public string autoModeService = "/set_auto_mode";

    [Header("Object to Enable/Disable")]
    public GameObject objetoParaOcultar;

    [Header("UI Text (TextMeshPro)")]
    public TextMeshProUGUI textoCanvas;
    public string textoAutoOn = "Switch to Plan & Execute";
    public string textoAutoOff = "Switch to Auto Mode";
    public Color corAutoOn = new Color(0f, 0.39f, 0f);
    public Color corAutoOff = new Color(0.8f, 0.8f, 0f);

    [Header("Enable Sounds (Automatic Mode ON)")]
    public AudioSource enableSound1;
    public AudioSource enableSound2;

    [Header("Disable Sounds (Automatic Mode OFF)")]
    public AudioSource disableSound1;
    public AudioSource disableSound2;

    [Header("Collision Sound")]
    public AudioSource collisionSound;
    public float collisionSoundCooldown = 2f;

    [Header("Haptic Feedback")]
    [Range(0f, 1f)] public float hapticIntensity = 0.2f;
    public float hapticDuration = 0.15f;

    [Header("Collision Detectors")]
    public ChangeColorOnCollision[] collisionDetectors;
    public ChangeColorOnCollision_garra[] garraDetectors;

    [Header("Planning/Execution Scripts")]
    public ButtonAPlan planScript;
    public ButtonBExecute executeScript;

    private ROSConnection ros;
    private bool currentState = false;
    private bool forcedOffDueToCollision = false;
    private float lastCollisionSoundTime = -Mathf.Infinity;

    // Reference to the script that controls the planning robot
    private PlannedPathReceiver pathReceiver;

    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterRosService<SetBoolRequest, SetBoolResponse>(autoModeService);

        if (yButtonAction != null && yButtonAction.action != null)
            yButtonAction.action.performed += OnYButtonPressed;
        else
            Debug.LogError("[AutoModeToggle] yButtonAction was not assigned in the Inspector!");

        // Attempts to find the PlannedPathReceiver script in the scene
        pathReceiver = FindObjectOfType<PlannedPathReceiver>();
        if (pathReceiver == null)
            Debug.LogWarning("[AutoModeToggle] No PlannedPathReceiver found in the scene.");

        SetAutoMode(false, playFeedback: false); // Starts disabled
    }

    void Update()
    {
        if (HasAnyCollision())
        {
            if (currentState)
            {
                Debug.Log("[AutoModeToggle] 🚨 Collision detected. Disabling automatic mode.");
                SetAutoMode(false, playFeedback: false);
                forcedOffDueToCollision = true;
            }

            if (collisionSound != null && Time.time - lastCollisionSoundTime >= collisionSoundCooldown)
            {
                collisionSound.Play();
                lastCollisionSoundTime = Time.time;
            }
        }
        else
        {
            if (!currentState && forcedOffDueToCollision)
            {
                Debug.Log("[AutoModeToggle] ✅ Collision resolved. Re-enabling automatic mode.");
                SetAutoMode(true, playFeedback: false);
                forcedOffDueToCollision = false;
            }
        }
    }

    private bool HasAnyCollision()
    {
        if (collisionDetectors != null)
        {
            foreach (var detector in collisionDetectors)
            {
                if (detector != null && detector.isColliding)
                    return true;
            }
        }

        if (garraDetectors != null)
        {
            foreach (var garra in garraDetectors)
            {
                if (garra != null && garra.IsColliding)
                    return true;
            }
        }

        return false;
    }

    private void OnYButtonPressed(InputAction.CallbackContext ctx)
    {
        SetAutoMode(!currentState, playFeedback: true);
        forcedOffDueToCollision = false;
    }

    private void SetAutoMode(bool newState, bool playFeedback = true)
    {
        currentState = newState;

        // Haptic feedback
        if (playFeedback && leftHandController != null)
            leftHandController.SendHapticImpulse(hapticIntensity, hapticDuration);

        // Sends ROS service request
        var request = new SetBoolRequest { data = currentState };
        ros.SendServiceMessage<SetBoolResponse>(
            autoModeService,
            request,
            response => Debug.Log($"[AutoModeToggle] Service response: {response.message}")
        );

        Debug.Log($"[AutoModeToggle] 🔁 Automatic mode is now {(currentState ? "ON" : "OFF")}");

        // Disables the optional visual object
        if (objetoParaOcultar != null)
            objetoParaOcultar.SetActive(!currentState);

        // Controls the behavior of the PlannedPathReceiver script
        if (pathReceiver != null)
        {
            pathReceiver.isAutoModeEnabled = currentState;
            pathReceiver.SetRenderersActive(false);
        }

        // Enables or disables the plan/execute scripts
        if (planScript != null) planScript.enabled = !currentState;
        if (executeScript != null) executeScript.enabled = !currentState;

        // Sounds
        if (playFeedback)
        {
            if (currentState)
            {
                enableSound1?.Play();
                enableSound2?.Play();
            }
            else
            {
                disableSound1?.Play();
                disableSound2?.Play();
            }
        }

        AtualizarTextoUI();
    }

    private void AtualizarTextoUI()
    {
        if (textoCanvas == null) return;

        textoCanvas.text = currentState ? textoAutoOn : textoAutoOff;
        textoCanvas.color = currentState ? corAutoOn : corAutoOff;
    }

    void OnDestroy()
    {
        if (yButtonAction != null && yButtonAction.action != null)
            yButtonAction.action.performed -= OnYButtonPressed;
    }
}