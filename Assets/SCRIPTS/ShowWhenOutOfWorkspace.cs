using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(Renderer))]
public class WorkspaceFeedback : MonoBehaviour
{
    [Header("References")]
    public RobotBase robotBase;
    public XRBaseController leftHandController;
    public XRBaseController rightHandController;

    [Header("Dissolve")]
    [Range(0f, 1f)] public float maxDissolveAmount = 1f; // completely invisible
    [Range(0f, 1f)] public float minDissolveAmount = 0f; // fully visible
    public float appearSpeed = 2f;
    public float disappearSpeed = 1f;

    [Header("Sounds When Leaving the Workspace")]
    public AudioSource warningSound1;
    public AudioSource warningSound2;

    [Header("Delay Between Audio Clips")]
    public float soundCooldown = 2f;

    [Header("Haptic Feedback")]
    [Range(0f, 1f)] public float hapticIntensity = 0.3f;
    public float hapticDuration = 0.2f;

    private Renderer rend;
    private Material fadeMaterial;
    private float dissolveAmount = 1f;  // starts invisible
    private float lastSoundTime = -Mathf.Infinity;
    private bool playedFeedback = false;
    private bool useFirstSound = true;

    void Start()
    {
        rend = GetComponent<Renderer>();
        fadeMaterial = rend.material;

        // Starts invisible (dissolved)
        dissolveAmount = maxDissolveAmount;
        if (fadeMaterial.HasProperty("_DissolveAmount"))
            fadeMaterial.SetFloat("_DissolveAmount", dissolveAmount);

        rend.enabled = true;
    }

    void Update()
    {
        if (robotBase == null || fadeMaterial == null)
            return;

        bool isOut = robotBase.isOutOfWorkspace;
        float targetDissolve = isOut ? minDissolveAmount : maxDissolveAmount;
        float speed = isOut ? appearSpeed : disappearSpeed;

        dissolveAmount = Mathf.MoveTowards(dissolveAmount, targetDissolve, Time.deltaTime * speed);

        if (fadeMaterial.HasProperty("_DissolveAmount"))
            fadeMaterial.SetFloat("_DissolveAmount", dissolveAmount);

        if (isOut && !playedFeedback)
        {
            if (Time.time - lastSoundTime >= soundCooldown)
            {
                PlaySound();
                lastSoundTime = Time.time;
            }

            PlayHaptics();
            playedFeedback = true;
        }
        else if (!isOut)
        {
            playedFeedback = false;
        }
    }

    private void PlaySound()
    {
        if (useFirstSound && warningSound1 != null)
            warningSound1.Play();
        else if (!useFirstSound && warningSound2 != null)
            warningSound2.Play();

        useFirstSound = !useFirstSound;
    }

    private void PlayHaptics()
    {
        leftHandController?.SendHapticImpulse(hapticIntensity, hapticDuration);
        rightHandController?.SendHapticImpulse(hapticIntensity, hapticDuration);
    }
}