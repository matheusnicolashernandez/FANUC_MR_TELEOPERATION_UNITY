using UnityEngine;
using UnityEngine.Audio;

public class EQTestButton : MonoBehaviour
{
    public AudioMixer mixer;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            mixer.SetFloat("Gain_4000Hz", 10f);
            Debug.Log("Setado Gain_4000Hz para +10 dB via tecla espaço.");
        }
    }
}
