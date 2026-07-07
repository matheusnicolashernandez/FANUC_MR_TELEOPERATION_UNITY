

using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    WebCamTexture webcam;
    public GameObject panel;

    // Start is called before the first frame update
    void Start()
    {
        webcam = new WebCamTexture(); // Corrigido com ponto e vírgula
        panel.GetComponent<Renderer>().material.mainTexture = webcam;
        webcam.Play();
    }
}
