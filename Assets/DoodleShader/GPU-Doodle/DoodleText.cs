using UnityEngine;

public class DoodleText : MonoBehaviour
{
    public Material mat;              // - Target material

    public Vector4 DoodleMaxOffset;   // - How far the UV can be distorted
    public float DoodleFrameTime;     // - How long does a frame last
    public int DoodleFrameCount;      // - How many frames per animation
    public Vector4 DoodleNoiseScale;  // - How noisy should the effect be

    public bool DoodleOn;        // - Toggle doodle effect

    void Update()
    {
        SetAll();
    }

    public void SetAll()
    {
        mat.SetVector("_DoodleMaxOffset", DoodleMaxOffset);
        mat.SetFloat("_DoodleFrameTime", DoodleFrameTime);
        mat.SetInt("_DoodleFrameCount", DoodleFrameCount);
        mat.SetVector("_DoodleNoiseScale", DoodleNoiseScale);

        if (DoodleOn)
            Shader.EnableKeyword("DOODLE_ON");
        else
            Shader.DisableKeyword("DOODLE_ON");
    }
}
