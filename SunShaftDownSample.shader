Shader "Hidden/Custom/SunShaftDownSample"
{
    HLSLINCLUDE

        #include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"

        TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
        float4 _Color;

        float4 Frag(VaryingsDefault i) : SV_Target
        {
            float3 SceneColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord).rgb;
            float2 NormalizedCoordinates = i.texcoord;
            float EdgeMask = 1.0f - NormalizedCoordinates.x * (1.0f - NormalizedCoordinates.x) * NormalizedCoordinates.y * (1.0f - NormalizedCoordinates.y) * 8.0f;
            EdgeMask = EdgeMask * EdgeMask * EdgeMask * EdgeMask;
            float Luminance = max(dot(SceneColor, half3(.3f, .59f, .11f)), 6.10352e-5);
            float AdjustedLuminance = clamp(Luminance, 0.0f, 4);
            // float3 BloomColor = SceneColor / Luminance * AdjustedLuminance * 2.0f;
            float4 color;
            // return color;
            color.rgb = SceneColor *  (1 - EdgeMask) * AdjustedLuminance * _Color.rgb;
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
