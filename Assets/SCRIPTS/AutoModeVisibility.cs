using UnityEngine;

public class AutoModeVisibility : MonoBehaviour
{
    [Header("Referência do PlannedPathReceiver principal")]
    public PlannedPathReceiver plannedPathReceiver;

    private Renderer[] renderers;

    void Start()
    {
        renderers = GetComponentsInChildren<Renderer>();

        if (plannedPathReceiver == null)
        {
            Debug.LogError("[AutoModeVisibility] Referência ao PlannedPathReceiver não atribuída!");
        }
    }

    void Update()
    {
        if (plannedPathReceiver != null)
        {
            // Sincroniza visibilidade da garra com a visibilidade do robô principal
            SetGarraVisibility(plannedPathReceiver.IsRobotVisible);
        }
    }

    private void SetGarraVisibility(bool visible)
    {
        foreach (Renderer rend in renderers)
        {
            if (rend.enabled != visible)
                rend.enabled = visible;
        }
    }
}
