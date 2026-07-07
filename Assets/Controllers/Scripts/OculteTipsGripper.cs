using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class OculteTipsGripper : MonoBehaviour
{
    [Header("Objetos para ocultar com fade")]
    public List<GameObject> objectsToHide;

    [Header("Configurações de fade")]
    public float fadeDuration = 0.5f;
    public float scaleDuration = 0.25f;

    [Header("Som (opcional)")]
    public AudioSource disableAudio;

    private bool jaDesapareceu = false;

    void Start()
    {
        var interactable = GetComponent<XRGrabInteractable>();
        if (interactable != null)
            interactable.selectEntered.AddListener(OnGrab);
    }

    void OnGrab(SelectEnterEventArgs args)
    {
        if (jaDesapareceu) return;

        if (disableAudio != null)
            disableAudio.Play();

        StartCoroutine(FadeOutAll());
        jaDesapareceu = true;
    }

    IEnumerator FadeOutAll()
    {
        List<CanvasGroup> uiList = new List<CanvasGroup>();
        List<Material> rendererMats = new List<Material>();
        List<Transform> scales = new List<Transform>();

        foreach (var obj in objectsToHide)
        {
            if (obj == null) continue;

            scales.Add(obj.transform);

            var cg = obj.GetComponent<CanvasGroup>();
            if (cg != null)
            {
                uiList.Add(cg);
                continue;
            }

            foreach (var rend in obj.GetComponentsInChildren<Renderer>())
                foreach (var mat in rend.materials)
                {
                    SetupMaterialWithFade(mat);
                    rendererMats.Add(mat);
                }
        }

        float t = 0f;
        Vector3[] startScales = new Vector3[scales.Count];
        for (int i = 0; i < scales.Count; i++)
            startScales[i] = scales[i].localScale;

        while (t < fadeDuration)
        {
            float alpha = Mathf.Lerp(1f, 0f, t / fadeDuration);
            float scaleFactor = Mathf.SmoothStep(1f, 0.7f, t / scaleDuration);

            foreach (var cg in uiList)
                cg.alpha = alpha;

            foreach (var mat in rendererMats)
            {
                if (mat.HasProperty("_Color"))
                {
                    Color c = mat.color;
                    c.a = alpha;
                    mat.color = c;
                }
            }

            for (int i = 0; i < scales.Count; i++)
                if (scales[i] != null)
                    scales[i].localScale = startScales[i] * scaleFactor;

            t += Time.deltaTime;
            yield return null;
        }

        foreach (var cg in uiList)
        {
            cg.alpha = 0f;
            cg.gameObject.SetActive(false);
        }

        foreach (var obj in objectsToHide)
        {
            if (obj == null) continue;
            if (obj.GetComponent<CanvasGroup>() == null)
                obj.SetActive(false);
            obj.transform.localScale = Vector3.one;
        }
    }

    void SetupMaterialWithFade(Material mat)
    {
        if (!mat.HasProperty("_Mode")) return;
        mat.SetFloat("_Mode", 2);
        mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        mat.SetInt("_ZWrite", 0);
        mat.DisableKeyword("_ALPHATEST_ON");
        mat.EnableKeyword("_ALPHABLEND_ON");
        mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        mat.renderQueue = 3000;
    }
}
