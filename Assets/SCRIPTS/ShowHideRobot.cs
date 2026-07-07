using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class ShowHideRobot : MonoBehaviour
{
    [Header("Button to Toggle Visibility")]
    public InputActionProperty toggleButton;

    [Header("Objects to Hide/Show")]
    public GameObject[] objetosParaAlternar;

    [Header("UI Text (TextMeshPro)")]
    public TextMeshProUGUI textoCanvas;
    public string textoVisivel = "Hide Robot";
    public string textoOculto = "Show Robot";
    public Color corVisivel = new Color(0f, 0.39f, 0f);
    public Color corOculto = new Color(0.8f, 0.8f, 0f);

    private bool visibilidadeAtiva = true;
    private bool ultimoEstadoDoBotao = false;

    void OnEnable()
    {
        if (toggleButton.action != null)
            toggleButton.action.Enable();
    }

    void OnDisable()
    {
        if (toggleButton.action != null)
            toggleButton.action.Disable();
    }

    void Update()
    {
        if (toggleButton.action == null) return;

        bool botaoPressionado = toggleButton.action.ReadValue<float>() > 0.5f;

        if (botaoPressionado && !ultimoEstadoDoBotao)
        {
            visibilidadeAtiva = !visibilidadeAtiva;
            AlternarVisibilidade();
            AtualizarTextoUI();
        }

        ultimoEstadoDoBotao = botaoPressionado;
    }

    private void AlternarVisibilidade()
    {
        foreach (GameObject obj in objetosParaAlternar)
        {
            if (obj != null)
            {
                Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
                foreach (Renderer r in renderers)
                {
                    r.enabled = visibilidadeAtiva;
                }
            }
        }

        Debug.Log("Visibility changed: " + (visibilidadeAtiva ? "Visible" : "Hidden"));
    }

    private void AtualizarTextoUI()
    {
        if (textoCanvas == null) return;

        textoCanvas.text = visibilidadeAtiva ? textoVisivel : textoOculto;
        textoCanvas.color = visibilidadeAtiva ? corVisivel : corOculto;
    }
}