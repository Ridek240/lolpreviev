using UnityEngine;
using UnityEngine.UI;

public class NDIStatsMonitor : MonoBehaviour
{
    public RenderTexture ndiTexture;
    public Text uiText; // UI Text do wyœwietlania danych
    public int refreshRate = 1; // Co ile sekund odœwie¿aæ dane

    private float timer;
    private int bytesPerPixel = 4; // Domyœlnie 4 bajty (np. dla BGRA32)

    void Start()
    {
        if (ndiTexture == null)
        {
            Debug.LogError("Brak przypisanego RenderTexture!");
            enabled = false;
            return;
        }

        DetectFormat();
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= refreshRate)
        {
            timer = 0f;
            DisplayStats();
        }
    }

    void DetectFormat()
    {
        switch (ndiTexture.graphicsFormat)
        {
            case UnityEngine.Experimental.Rendering.GraphicsFormat.R8G8B8A8_UNorm:
            case UnityEngine.Experimental.Rendering.GraphicsFormat.B8G8R8A8_UNorm:
                bytesPerPixel = 4;
                break;
            case UnityEngine.Experimental.Rendering.GraphicsFormat.R16G16B16A16_SFloat:
                bytesPerPixel = 8;
                break;
            case UnityEngine.Experimental.Rendering.GraphicsFormat.R32G32B32A32_UInt:
            case UnityEngine.Experimental.Rendering.GraphicsFormat.R32G32B32A32_SFloat:
                bytesPerPixel = 16;
                break;
            default:
                Debug.LogWarning($"Nierozpoznany format grafiki: {ndiTexture.graphicsFormat}, zak³adam 4 bajty na piksel.");
                bytesPerPixel = 4;
                break;
        }
    }

    void DisplayStats()
    {
        int width = ndiTexture.width;
        int height = ndiTexture.height;
        int frameSizeBytes = width * height * bytesPerPixel;
        int targetFPS = (int)(1f / Time.deltaTime);

        int bitrateBps = frameSizeBytes * targetFPS * 8;
        float bitrateMbps = bitrateBps / 1_000_000f;

        string stats = $"NDI Stats:\n" +
                       $"- Resolution: {width}x{height}\n" +
                       $"- Format: {ndiTexture.graphicsFormat} ({bytesPerPixel} B/pixel)\n" +
                       $"- FPS: {targetFPS}\n" +
                       $"- Bitrate: {bitrateMbps:F2} Mbps";

        Debug.Log(stats);

        if (uiText != null)
        {
            uiText.text = stats;
        }
    }
}
