using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class VRControllerTransparency : MonoBehaviour
{
    public GameObject controllerModel;
    private XRDirectInteractor interactor;
    private Renderer[] renderers;

    // Stores clones to avoid modifying shared materials
    private Material[][] originalMaterials;
    private Material[][] transparentMaterials;

    [Range(0f, 1f)]
    public float transparencyAmount = 0.3f;

    void Start()
    {
        interactor = GetComponent<XRDirectInteractor>();
        renderers = controllerModel.GetComponentsInChildren<Renderer>();

        if (renderers.Length == 0)
        {
            Debug.LogError("No Renderer found!");
            return;
        }

        originalMaterials = new Material[renderers.Length][];
        transparentMaterials = new Material[renderers.Length][];

        for (int i = 0; i < renderers.Length; i++)
        {
            var mats = renderers[i].materials; // per-renderer instance
            originalMaterials[i] = new Material[mats.Length];
            transparentMaterials[i] = new Material[mats.Length];

            for (int j = 0; j < mats.Length; j++)
            {
                Material original = mats[j];

                // Safe clone of the original to store as the "original"
                Material originalClone = new Material(original);
                originalMaterials[i][j] = originalClone;

                // Creates a transparent copy to use while grabbing
                Material copy = new Material(original);
                SetMaterialTransparent(copy, transparencyAmount);
                transparentMaterials[i][j] = copy;
            }
        }

        interactor.selectEntered.AddListener(OnGrab);
        interactor.selectExited.AddListener(OnRelease);
    }

    void SetMaterialTransparent(Material mat, float alpha)
    {
        if (mat.HasProperty("_Color"))
        {
            Color c = mat.color;
            c.a = alpha;
            mat.color = c;
        }

        // Attempts to configure transparent mode (Standard shader)
        if (mat.HasProperty("_Mode"))
        {
            mat.SetFloat("_Mode", 3);
            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            mat.SetInt("_ZWrite", 0);
            mat.DisableKeyword("_ALPHATEST_ON");
            mat.EnableKeyword("_ALPHABLEND_ON");
            mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            mat.renderQueue = 3000;
        }
    }

    void OnGrab(SelectEnterEventArgs args)
    {
        for (int i = 0; i < renderers.Length; i++)
        {
            // Applies transparent materials
            renderers[i].materials = transparentMaterials[i];
        }
    }

    void OnRelease(SelectExitEventArgs args)
    {
        for (int i = 0; i < renderers.Length; i++)
        {
            // Restores the original clones (not shared references)
            renderers[i].materials = originalMaterials[i];
        }
    }
}