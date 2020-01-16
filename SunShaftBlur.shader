Shader "Hidden/Custom/SunShaftBlur"
{
    HLSLINCLUDE

        #include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"

        TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
        float4 _CenterAndParameter;
        float _Scale;
        #define NUM_SAMPLES 8
        float4 Frag(VaryingsDefault i) : SV_Target
        {
            float2 uv = i.texcoord;
            float3 BlurredValues = 0;
            // float2 scale = (1, _Center)

            float2 BlurVector = (_CenterAndParameter.xy - uv) * _Scale;
            for (int SampleIndex = 0; SampleIndex < NUM_SAMPLES; SampleIndex++)
            {
                float2 SampleUVs = uv + SampleIndex * BlurVector / NUM_SAMPLES;
                // Needed because sometimes the source texture is larger than the part we are reading from
                float2 ClampedUVs = clamp(SampleUVs, 0, 1);
                float3 SampleValue = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, ClampedUVs).rgb;
                BlurredValues += SampleValue;
            }
            float4 color;
            color.rgb = BlurredValues / (float)NUM_SAMPLES;
            color.a = 1;
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
                #pragma fragment Frag

            ENDHLSL
        }
    }
}
