Shader "Hidden/VHSNoiseGen"
{
    HLSLINCLUDE

        #include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"

        TEXTURE2D_SAMPLER2D(_HorizontalNoise, sampler_HorizontalNoise);
        TEXTURE2D_SAMPLER2D(_SpeckNoise, sampler_SpeckNoise);
        float _HorizontalNoisePos;
        float _HorizontalNoisePower;
        float4 _SpeckNoiseScaleOffset;
        float _Blend;

        float NoiseFrag(VaryingsDefault i) : SV_Target
        {
            float horizontalNoise = SAMPLE_TEXTURE2D(_HorizontalNoise, sampler_HorizontalNoise, float2(_HorizontalNoisePos, i.texcoord.y)).r;
            float speckNoise = SAMPLE_TEXTURE2D(_SpeckNoise, sampler_SpeckNoise, (i.texcoord - _SpeckNoiseScaleOffset.zw) * _SpeckNoiseScaleOffset.xy).r;
            return speckNoise > pow(saturate((1 - horizontalNoise) * (1 - horizontalNoise)), _HorizontalNoisePower);
        }

        TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
        float4 _MainTex_TexelSize;
        float2 _SmearOffsetAttenuation;
#define SMEAR_LENGTH 4
        float SmearFrag(VaryingsDefault i) : SV_Target
        {
            float color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord).r;
            [unroll]
            for (uint o = 1; o <= SMEAR_LENGTH; o++)
            {
                color += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord - float2(_MainTex_TexelSize.x * _SmearOffsetAttenuation.x * o, 0)).r* exp(-_SmearOffsetAttenuation.y * o);
            }
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
                #pragma fragment NoiseFrag
            ENDHLSL
        }
        Pass
        {
            HLSLPROGRAM
                #pragma vertex VertDefault
                #pragma fragment SmearFrag
            ENDHLSL
        }
    }
}
