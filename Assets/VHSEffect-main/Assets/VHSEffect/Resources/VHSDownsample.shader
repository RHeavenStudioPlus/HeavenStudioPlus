Shader "Hidden/VHSDownsample"
{
    HLSLINCLUDE

    #include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"

    TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
    float4 _MainTex_TexelSize;
    float _Blend;
    TEXTURE2D(_Noise);
    float _NoiseOpacity;


    float4 FragDown(VaryingsDefault i) : SV_Target
    {
        float2 offset = _MainTex_TexelSize.xy;
        offset.x *= 2;
        float4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord + float2(offset.x, offset.y))
                     + SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord + float2(-offset.x, offset.y))
                     + SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord + float2(offset.x, -offset.y))
                     + SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord + float2(-offset.x, -offset.y));
        color *= 0.25;
        color.rgb += SAMPLE_TEXTURE2D(_Noise, sampler_MainTex, i.texcoord).r * _NoiseOpacity;
        return color;
    }

    float _UpsampleBlend;
    float4 FragUp(VaryingsDefault i) : SV_Target
    {
        float4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord);
        color.a = _UpsampleBlend;
        return color;
    }

        ENDHLSL

        SubShader
    {
        Cull Off ZWrite Off ZTest Always

         Pass
        {
            HLSLPROGRAM
                #pragma vertex VertDefault
                #pragma fragment FragDown
            ENDHLSL
        }

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            HLSLPROGRAM
                #pragma vertex VertDefault
                #pragma fragment FragUp
            ENDHLSL
        }
    }
}
