using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraToRenderTexture : MonoBehaviour
{
    public RenderTexture targetTexture;

    void Start()
    {
        var cam = GetComponent<Camera>();
        cam.targetTexture = targetTexture;
    }
}
