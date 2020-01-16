using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
 
[Serializable]
[PostProcess(typeof(SunShaftRenderer), PostProcessEvent.BeforeStack, "Custom/SunShaft")]
public sealed class SunShaft : PostProcessEffectSettings
{
    [Range(0f, 10f), Tooltip("SunShaft intensity.")]
    public FloatParameter intensity = new FloatParameter { value = 1f };
    // [Range(0f, 4f), Tooltip("SunShaft threshold.")]
    // public FloatParameter threshold = new FloatParameter { value = 0f };
    [ColorUsage(false, true, 0f, 8f, 0.125f, 3f), Tooltip("Color")]
    public ColorParameter color = new ColorParameter { value = Color.white };
    public override bool IsEnabledAndSupported(PostProcessRenderContext context)
    {
        if( enabled.value  && intensity.value > 0f)
        {
             Light l = RenderSettings.sun;
            Vector3 lf = l.transform.forward;
            Camera camera = context.camera;
            var ct = camera.transform;
            if (Vector3.Dot(lf, ct.forward) < 0)
            {
                return true;
                // var vp = camera.WorldToViewportPoint(ct.position - lf * 10);
                // // Debug.Log(vp);
                // if (Mathf.Abs(vp.x - 0.5f) < 1 && Mathf.Abs(vp.y - 0.5f) < 1)
                // {
                //     return true;
                // }
            }
        }
        return false;
    }
}
 
internal sealed class SunShaftRenderer : PostProcessEffectRenderer<SunShaft>
{
    int sunShaftID;
    int sunShaftID2;
    int sunShaftCombine;
    public override void Init()
    {
        sunShaftID = Shader.PropertyToID("SunShaft");
        sunShaftID2 = Shader.PropertyToID("SunShaft2");
        sunShaftCombine = Shader.PropertyToID("_SunShaftCombine");
    }
    public override void Render(PostProcessRenderContext context)
    {
        Light l = RenderSettings.sun;
        Vector3 lf = l.transform.forward;
        Camera camera = context.camera;
        var ct = camera.transform;
        var vp = camera.WorldToViewportPoint(ct.position - lf * 10);
        var cmd = context.command;
        cmd.BeginSample("SunShaft");
        var sheet = context.propertySheets.Get(Shader.Find("Hidden/Custom/SunShaftDownSample"));

        sheet.properties.SetColor("_Color", settings.color);
        context.GetScreenSpaceTemporaryRT(cmd, sunShaftID, 0, context.sourceFormat, RenderTextureReadWrite.Default, FilterMode.Bilinear, context.screenWidth / 2, context.screenHeight / 2);
        context.GetScreenSpaceTemporaryRT(cmd, sunShaftID2, 0, context.sourceFormat, RenderTextureReadWrite.Default, FilterMode.Bilinear, context.screenWidth / 2, context.screenHeight / 2);
        context.GetScreenSpaceTemporaryRT(cmd, sunShaftCombine, 0, context.sourceFormat, RenderTextureReadWrite.Default, FilterMode.Bilinear, context.screenWidth / 2, context.screenHeight / 2);
        cmd.BlitFullscreenTriangle(context.source, sunShaftID, sheet, 0);
        sheet = context.propertySheets.Get(Shader.Find("Hidden/Custom/SunShaftBlur"));
        Vector4 Center = new Vector4(vp.x, vp.y, 1 / (float)context.width,  1 / (float)context.height);
        sheet.properties.SetVector("_CenterAndParameter", Center);
        sheet.properties.SetFloat("_Scale", 0.1f);
        cmd.BlitFullscreenTriangle(sunShaftID, sunShaftID2, sheet, 0);
        sheet.properties.SetFloat("_Scale", 0.3f);
        cmd.BlitFullscreenTriangle(sunShaftID2, sunShaftID, sheet, 0);
        sheet.properties.SetFloat("_Scale", 0.9f);
        cmd.BlitFullscreenTriangle(sunShaftID, sunShaftCombine, sheet, 0);
        sheet = context.propertySheets.Get(Shader.Find("Hidden/Custom/SunShaftCombine"));
        sheet.properties.SetFloat("_Intensity", settings.intensity);
        // sheet.properties.SetFloat("_Intensity", 1);
        cmd.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
        cmd.ReleaseTemporaryRT(sunShaftID);
        cmd.ReleaseTemporaryRT(sunShaftID2);
        cmd.ReleaseTemporaryRT(sunShaftCombine);
    }


}