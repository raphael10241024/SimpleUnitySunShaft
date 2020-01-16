Shader "Hidden/Custom/SunShaftCombine"
{
    HLSLINCLUDE

        #include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"

        TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
        TEXTURE2D_SAMPLER2D(_SunShaftCombine, sampler_SunShaftCombine);
        float _Intensity;
        // float4 _MinMaxUV;

        float4 Frag(VaryingsDefault i) : SV_Target
        {
            float3 SceneColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord).rgb;
            float3 BlurColor = SAMPLE_TEXTURE2D(_SunShaftCombine, sampler_SunShaftCombine, i.texcoord).rgb * _Intensity;
            float4 color;
            color.rgb = SceneColor;
            color.a = 1;
            color.rgb = SceneColor + BlurColor;
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
