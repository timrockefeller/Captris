
using UnityEngine;

[RequireComponent(typeof(Camera))]
[ExecuteInEditMode, AddComponentMenu("Image Effects/Tilt Shift")]
public class TiltShift : MonoBehaviour
{
    public bool Preview = false;

    [Range(-1f, 1f)]
    public float Offset = 0f;

    [Range(0f, 20f)]
    public float Area = 1f;

    [Range(0f, 20f)]
    public float Spread = 1f;

    [Range(8, 64)]
    public int Samples = 32;

    [Range(0f, 2f)]
    public float Radius = 1f;

    public bool UseDistortion = true;

    [Range(0f, 20f)]
    public float CubicDistortion = 5f;

    [Range(0.01f, 2f)]
    public float DistortionScale = 1f;

    [Range(0.01f, 2f)]
    public float Saturation = 1f;

    public Color bloodOutColor;

    [Range(0f, 1f)]
    public float bloodOutNum;
    public Shader Shader;

    protected Material m_Material;
    public Material Material
    {
        get
        {
            if (m_Material == null)
            {
                m_Material = new Material(Shader);
                m_Material.hideFlags = HideFlags.HideAndDontSave;
            }

            return m_Material;
        }
    }

    protected Vector4 m_GoldenRot = new Vector4();

    void Start()
    {

        // 看看你显卡行不行
        if (!Shader || !Shader.isSupported)
        {
            Debug.LogWarning("The shader is null or unsupported on this device");
            enabled = false;
        }

        // 黄金角度
        // (3 * -sqrt(5.0)) * PI  r
        float c = Mathf.Cos(2.39996323f);
        float s = Mathf.Sin(2.39996323f);
        m_GoldenRot.Set(c, s, -s, c);
    }

    void OnDisable()
    {
        if (m_Material)
            DestroyImmediate(m_Material);
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (UseDistortion)
            Material.EnableKeyword("USE_DISTORTION");
        else
            Material.DisableKeyword("USE_DISTORTION");

        Material.SetVector("_GoldenRot", m_GoldenRot);
        Material.SetVector("_Gradient", new Vector3(Offset, Area, Spread));
        Material.SetVector("_Distortion", new Vector2(CubicDistortion, DistortionScale));
        Material.SetVector("_Params", new Vector4(Samples, Radius, 1f / source.width, 1f / source.height));
        Material.SetFloat("_Saturation", Saturation);
        Material.SetColor("_BloodOutColor", bloodOutColor);
        Material.SetFloat("_BloodOutNum", bloodOutNum);
        Graphics.Blit(source, destination, Material, Preview ? 0 : 1);
    }
}
