using UnityEngine;
using UnityEngine.InputSystem;

public class ControllerHandAnimator_right : MonoBehaviour
{
    [Header("Inputs")]
    public InputActionProperty triggerAction;
    public InputActionProperty gripAction;
    public InputActionProperty thumbstickAction;
    public InputActionProperty button1Action;
    public InputActionProperty button2Action;
    public InputActionProperty button3Action;

    [Header("Animator")]
    public Animator controllerAnimator;

    void Update()
    {
        // Leitura dos valores
        float triggerValue = triggerAction.action.ReadValue<float>();
        float gripValue = gripAction.action.ReadValue<float>();
        Vector2 joy = thumbstickAction.action.ReadValue<Vector2>();

        float button1 = button1Action.action.ReadValue<float>();
        float button2 = button2Action.action.ReadValue<float>();
        float button3 = button3Action.action.ReadValue<float>();

        // Atualiza o Animator com os nomes certos
        controllerAnimator.SetFloat("Trigger", triggerValue);
        controllerAnimator.SetFloat("Grip", gripValue);
        controllerAnimator.SetFloat("Joy X", joy.x);
        controllerAnimator.SetFloat("Joy Y", joy.y);

        controllerAnimator.SetFloat("Button 1", button1);
        controllerAnimator.SetFloat("Button 2", button2);
        controllerAnimator.SetFloat("Button 3", button3);
    }
}
