using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using System;

[Serializable, VolumeComponentMenu("Post-processing/Custom/DataMoshPostProcessvolume")]
public sealed class DataMoshPostProcessvolume : CustomPostProcessVolumeComponent, IPostProcessComponent
{
    [Tooltip("Controls the intensity of the effect.")]
    public ClampedFloatParameter intensity = new ClampedFloatParameter(0f, 0f, 1f);
    public ClampedFloatParameter randomizeBlocksWithTime = new ClampedFloatParameter(0f, 0f, 1f);

    
    public ClampedIntParameter BlockSize = new ClampedIntParameter(5, 2, 20);
    public BoolParameter ShowInSceneView = new BoolParameter(false);



    Material m_Material;

    public PostFxParameter _MotionVectorTex = new PostFxParameter(null);
    public PostFxParameter _BufferTex = new PostFxParameter(null);

    public override bool visibleInSceneView => ShowInSceneView.value;

    public bool IsActive() => m_Material != null && intensity.value > 0f;

    // Do not forget to add this post process in the Custom Post Process Orders list (Project Settings > Graphics > HDRP Settings).
    public override CustomPostProcessInjectionPoint injectionPoint => CustomPostProcessInjectionPoint.AfterPostProcess;
    public static Vector2 renderDimensions;
    const string kShaderName = "Hidden/Shader/DataMoshPostProcess";

    public override void Setup()
    {
        if (Shader.Find(kShaderName) != null)
            m_Material = new Material(Shader.Find(kShaderName));
        else
            Debug.LogError($"Unable to find shader '{kShaderName}'. Post Process Volume DataMoshPostProcessvolume is unable to load.");
    }

    public override void Render(CommandBuffer cmd, HDCamera camera, RTHandle source, RTHandle destination)
    {
        if (m_Material == null)
            return;
        m_Material.SetTexture("_CameraMotionVectorTexture", (RenderTexture)_MotionVectorTex);
        m_Material.SetTexture("_BufferTexture", (RenderTexture)_BufferTex);
        m_Material.SetFloat("_Intensity", intensity.value);
        m_Material.SetFloat("_RandomizeBlocksWithTime", randomizeBlocksWithTime.value);

        
        cmd.Blit(source, destination, m_Material, 0);

        cmd.Blit(m_Material.mainTexture, (RenderTexture)_BufferTex);

         renderDimensions =  source.GetScaledSize();
    }

    public override void Cleanup()
    {
        CoreUtils.Destroy(m_Material);
    }
}
