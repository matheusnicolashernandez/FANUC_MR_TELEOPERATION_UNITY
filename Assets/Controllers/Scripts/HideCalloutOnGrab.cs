using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class HideCalloutOnGrab : MonoBehaviour
{
    [Header("Desativar se pegar com a MÃO ESQUERDA")]
    public List<GameObject> objectsToDisableLeft;

    [Header("Desativar se pegar com a MÃO DIREITA")]
    public List<GameObject> objectsToDisableRight;

    private XRGrabInteractable grab;

    void Awake()
    {
        grab = GetComponent<XRGrabInteractable>();
        if (grab == null)
            Debug.LogError("Este objeto precisa ter um XRGrabInteractable.");
    }

    void OnEnable()
    {
        grab.selectEntered.AddListener(OnGrab);
        grab.selectExited.AddListener(OnRelease);
    }

    void OnDisable()
    {
        grab.selectEntered.RemoveListener(OnGrab);
        grab.selectExited.RemoveListener(OnRelease);
    }

    void OnGrab(SelectEnterEventArgs args)
    {
        var interactor = args.interactorObject.transform;
        if (interactor == null) return;

        string interactorName = interactor.name.ToLower();

        if (interactorName.Contains("left"))
        {
            Debug.Log("Pegou com a MÃO ESQUERDA");
            foreach (GameObject obj in objectsToDisableLeft)
                if (obj != null) obj.SetActive(false);
        }
        else if (interactorName.Contains("right"))
        {
            Debug.Log("Pegou com a MÃO DIREITA");
            foreach (GameObject obj in objectsToDisableRight)
                if (obj != null) obj.SetActive(false);
        }
        else
        {
            Debug.LogWarning("Mão não identificada: " + interactor.name);
        }
    }

    void OnRelease(SelectExitEventArgs args)
    {
        var interactor = args.interactorObject.transform;
        if (interactor == null) return;

        string interactorName = interactor.name.ToLower();

        if (interactorName.Contains("left"))
        {
            foreach (GameObject obj in objectsToDisableLeft)
                if (obj != null) obj.SetActive(true);
        }
        else if (interactorName.Contains("right"))
        {
            foreach (GameObject obj in objectsToDisableRight)
                if (obj != null) obj.SetActive(true);
        }
    }
}