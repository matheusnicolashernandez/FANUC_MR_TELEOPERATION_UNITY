using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.UI;

public class ToggleObjectsOnPrimaryButton : MonoBehaviour
{
    [Header("Input (Primary Button - Left Hand)")]
    public InputActionProperty primaryButtonAction;
    public XRBaseController leftHandController;

    [Header("Objetos para mostrar/ocultar com fade")]
    public List<GameObject> objectsToToggle;

    [Header("Configurações de feedback")]
    public float fadeDuration = 0.5f;
    public float scaleDuration = 0.25f;
    public AudioSource enableAudio;
    public AudioSource disableAudio;

    [Header("Haptic")]
    public float hapticIntensity = 0.2f;
    public float hapticDuration = 0.15f;

    private bool lastButtonState = false;
    private bool objectsVisible = true;
    private Coroutine fadeCoroutine = null;

    void OnEnable()
    {
        if (primaryButtonAction.action != null)
            primaryButtonAction.action.Enable();
    }

    void OnDisable()
    {
        if (primaryButtonAction.action != null)
            primaryButtonAction.action.Disable();
    }

    void Update()
    {
        if (primaryButtonAction.action == null) return;

        bool isPressed = primaryButtonAction.action.ReadValue<float>() > 0.5f;

        if (isPressed && !lastButtonState)
        {
            if (leftHandController != null)
                leftHandController.SendHapticImpulse(hapticIntensity, hapticDuration);

            if (objectsVisible)
            {
                if (disableAudio != null) disableAudio.Play();
                fadeCoroutine = StartCoroutine(FadeOutAll());
            }
            else
            {
                if (enableAudio != null) enableAudio.Play();
                if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
                ShowAllInstantly();
            }

            objectsVisible = !objectsVisible;
        }

        lastButtonState = isPressed;
    }

    IEnumerator FadeOutAll()
    {
        List<CanvasGroup> uiList = new List<CanvasGroup>();
        List<Graphic> graphics = new List<Graphic>();
        List<Renderer> meshRenderers = new List<Renderer>();
        List<Color> meshBaseColors = new List<Color>();
        List<Transform> scales = new List<Transform>();

        // coletar componentes
        foreach (var obj in objectsToToggle)
        {
            if (obj == null) continue;

            scales.Add(obj.transform);

            var cg = obj.GetComponent<CanvasGroup>();
            if (cg != null)
            {
                uiList.Add(cg);
                continue;
            }

            foreach (var g in obj.GetComponentsInChildren<Graphic>(true))
                graphics.Add(g);

            foreach (var rend in obj.GetComponentsInChildren<Renderer>(true))
            {
                if (rend == null) continue;
                meshRenderers.Add(rend);

                // pega uma cor base razoável (se shader tem _Color)
                Color baseColor = Color.white;
                if (rend.sharedMaterial != null && rend.sharedMaterial.HasProperty("_Color"))
                    baseColor = rend.sharedMaterial.color;
                else if (rend.material != null && rend.material.HasProperty("_Color"))
                    baseColor = rend.material.color;

                meshBaseColors.Add(baseColor);
            }
        }

        // scalings start
        Vector3[] startScales = new Vector3[scales.Count];
        for (int i = 0; i < scales.Count; i++)
            startScales[i] = scales[i].localScale;

        float t = 0f;
        // usar MaterialPropertyBlock para alterar alpha por renderer (não altera materiais compartilhados)
        while (t < fadeDuration)
        {
            float alpha = Mathf.Lerp(1f, 0f, t / fadeDuration);
            float scaleFactor = Mathf.SmoothStep(1f, 0.7f, Mathf.Min(t / scaleDuration, 1f));

            foreach (var cg in uiList)
                cg.alpha = alpha;

            foreach (var g in graphics)
            {
                if (g == null) continue;
                Color c = g.color;
                c.a = alpha;
                g.color = c;
            }

            for (int i = 0; i < meshRenderers.Count; i++)
            {
                var rend = meshRenderers[i];
                if (rend == null) continue;

                MaterialPropertyBlock mpb = new MaterialPropertyBlock();
                rend.GetPropertyBlock(mpb);
                Color baseC = meshBaseColors[i];
                Color c = new Color(baseC.r, baseC.g, baseC.b, alpha);
                mpb.SetColor("_Color", c);
                rend.SetPropertyBlock(mpb);
            }

            for (int i = 0; i < scales.Count; i++)
            {
                if (scales[i] != null)
                    scales[i].localScale = startScales[i] * scaleFactor;
            }

            t += Time.deltaTime;
            yield return null;
        }

        // final do fade: set alpha 0 e desligar renderers/graphics
        foreach (var cg in uiList)
        {
            if (cg == null) continue;
            cg.alpha = 0f;
            cg.gameObject.SetActive(false);
        }

        foreach (var g in graphics)
        {
            if (g == null) continue;
            Color c = g.color;
            c.a = 0f;
            g.color = c;
            g.gameObject.SetActive(false);
        }

        for (int i = 0; i < meshRenderers.Count; i++)
        {
            var rend = meshRenderers[i];
            if (rend == null) continue;
            // garantir alpha 0
            MaterialPropertyBlock mpb = new MaterialPropertyBlock();
            mpb.SetColor("_Color", new Color(meshBaseColors[i].r, meshBaseColors[i].g, meshBaseColors[i].b, 0f));
            rend.SetPropertyBlock(mpb);

            // desliga o renderer para que nada mais (restauração de materiais, etc.) faça ele reaparecer
            rend.enabled = false;
        }

        // manter GameObject ativo (opcional) — evita efeitos colaterais de SetActive
        foreach (var obj in objectsToToggle)
        {
            if (obj == null) continue;
            obj.transform.localScale = Vector3.one;
        }

        fadeCoroutine = null;
    }

    void ShowAllInstantly()
    {
        // re-ativar objetos e limpar propertyblocks
        foreach (var obj in objectsToToggle)
        {
            if (obj == null) continue;
            // reativa GameObject (no caso de ter sido desativado por outra coisa)
            obj.SetActive(true);
            obj.transform.localScale = Vector3.one;

            var cg = obj.GetComponent<CanvasGroup>();
            if (cg != null)
            {
                cg.alpha = 1f;
                cg.gameObject.SetActive(true);
            }

            foreach (var g in obj.GetComponentsInChildren<Graphic>(true))
            {
                if (g == null) continue;
                g.gameObject.SetActive(true);
                Color c = g.color;
                c.a = 1f;
                g.color = c;
            }

            foreach (var rend in obj.GetComponentsInChildren<Renderer>(true))
            {
                if (rend == null) continue;
                // limpar property block (restaura visual para materiais originais)
                rend.SetPropertyBlock(null);
                // reativar renderer
                rend.enabled = true;
            }
        }
    }
}
