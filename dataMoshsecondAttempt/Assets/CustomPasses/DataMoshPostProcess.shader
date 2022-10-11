Shader "Hidden/Shader/DataMoshPostProcess"
{
    Properties
    {
        // This property is necessary to make the CommandBuffer.Blit bind the source texture to _MainTex
        _MainTex("Main Texture", 2DArray) = "grey" {}

    }

    HLSLINCLUDE

    #pragma target 4.5
    #pragma only_renderers d3d11 playstation xboxone xboxseries vulkan metal switch

    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
    #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"
    #include "Packages/com.unity.render-pipelines.high-definition/Runtime/PostProcessing/Shaders/FXAA.hlsl"
    #include "Packages/com.unity.render-pipelines.high-definition/Runtime/PostProcessing/Shaders/RTUpscale.hlsl"

    struct Attributes
    {
        uint vertexID : SV_VertexID;
        UNITY_VERTEX_INPUT_INSTANCE_ID
    };

    struct Varyings
    {
        float4 positionCS : SV_POSITION;
        float2 texcoord   : TEXCOORD0;
        UNITY_VERTEX_OUTPUT_STEREO
    };

    Varyings Vert(Attributes input)
    {
        Varyings output;
        UNITY_SETUP_INSTANCE_ID(input);
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
        output.positionCS = GetFullScreenTriangleVertexPosition(input.vertexID);
        output.texcoord = GetFullScreenTriangleTexCoord(input.vertexID);
        return output;
    }

    // List of properties to control your post process effect
    float _Intensity;
    float _RandomizeBlocksWithTime;
    TEXTURE2D_X(_MainTex);
    TEXTURE2D(_CameraMotionVectorTexture);
    SAMPLER(sampler_CameraMotionVectorTexture);
    TEXTURE2D(_BufferTexture);
    SAMPLER(sampler_BufferTexture);

    float nrand(float x, float y)
    {
        return frac(sin(dot(float2(x, y), float2(12.9898, 78.233))) * 43758.5453);
    }



    float4 CustomPostProcess(Varyings input) : SV_Target
    {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

    float2 uvRounded = round(input.texcoord * (_ScreenParams.xy / 32)) / (_ScreenParams.xy / 32); // blocks uv for motion vector. used below
    //take _ScreenParams into account so blocks aren't stretched
    float randomNumber = nrand(_Time.x, uvRounded.x + uvRounded.y * _ScreenParams.x);

       float3 motion = SAMPLE_TEXTURE2D(_CameraMotionVectorTexture, s_linear_clamp_sampler, uvRounded).xyz;
        float3 sourceColor = SAMPLE_TEXTURE2D_X(_MainTex, s_linear_clamp_sampler, input.texcoord).xyz;

       float2 motionVectorUVs = float2(input.texcoord.x - motion.r, input.texcoord.y - motion.g);

        
        //inverse uv + appropriate displacement for motion vectors texture
        //also takes into account inverse UV values depending on graphics platform
        float3 bufferColor = SAMPLE_TEXTURE2D(_BufferTexture, s_linear_clamp_sampler, motionVectorUVs).xyz; ///recycling
      //   float3 color = lerp(sourceColor, bufferColor, _Intensity);
         float3 color = lerp(sourceColor, bufferColor, lerp(round(1 - (randomNumber) / 1.4), 1, _Intensity));


       //  float3 color = lerp(moshedColor, randomizedColor, _RandomizeBlocksWithTime)
        return float4(color, 1);



    }

    ENDHLSL

    SubShader
    {
        Tags{ "RenderPipeline" = "HDRenderPipeline" }
        Pass
        {
            Name "DataMoshPostProcess"

            ZWrite Off
            ZTest Always
            Blend Off
            Cull Off

            HLSLPROGRAM
                #pragma fragment CustomPostProcess
                #pragma vertex Vert
            ENDHLSL
        }
    }
    Fallback Off
}
