using UnityEngine;
using UnityEngine.Rendering;

[System.Serializable]
public class PostFxParameter : VolumeParameter<RenderTexture>
{
    public PostFxParameter(RenderTexture value, bool overrideState = false)
        : base(value, overrideState)
    {
    }
}

[System.Serializable]
public class PostFxShader : VolumeParameter<Shader>
{
    public PostFxShader(Shader value, bool overrideState = false)
        : base(value, overrideState)
    {
    }
}
