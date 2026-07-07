using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class StreamReceiver : MonoBehaviour
{
    public string streamUrl = "http://10.22.16.126:8080/?action=snapshot";
    public Renderer targetRenderer;
    public float refreshRate = 0.1f; // 100ms (~10 FPS)

    private Texture2D currentTexture;

    IEnumerator Start()
    {
        // Test texture to verify whether the object is visible
        Texture2D testTexture = new Texture2D(128, 128);
        Color[] pixels = new Color[128 * 128];
        for (int i = 0; i < pixels.Length; i++)
            pixels[i] = Color.red;
        testTexture.SetPixels(pixels);
        testTexture.Apply();
        targetRenderer.material.mainTexture = testTexture;

        yield return new WaitForSeconds(1f);

        // Streaming loop
        while (true)
        {
            using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(streamUrl))
            {
                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    Texture2D newTexture = DownloadHandlerTexture.GetContent(request);

                    if (newTexture != null)
                    {
                        // Destroys the previous texture to prevent memory leaks
                        if (currentTexture != null)
                            Destroy(currentTexture);

                        currentTexture = newTexture;
                        targetRenderer.material.mainTexture = currentTexture;
                    }
                }
                else
                {
                    Debug.LogWarning("Error loading image: " + request.error);
                }
            }

            yield return new WaitForSeconds(refreshRate);
        }
    }

    void OnDestroy()
    {
        // Releases the current texture when destroying the object
        if (currentTexture != null)
        {
            Destroy(currentTexture);
        }
    }
}