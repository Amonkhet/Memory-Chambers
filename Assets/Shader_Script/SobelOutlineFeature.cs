using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class SobelOutlineFeature : ScriptableRendererFeature
{
    [System.Serializable]
    public class Settings
    {
        public RenderPassEvent injectionPoint = RenderPassEvent.AfterRenderingTransparents;
        public Shader sobelShader;
        public Color edgeColor = new Color(0.07f, 0.07f, 0.07f, 1f);
        [Range(0f, 1f)] public float edgeThreshold = 0.10f;
        [Range(0.001f, 0.5f)] public float edgeSoftness = 0.05f;
        [Range(0f, 5f)] public float depthScale = 3.0f;
        [Range(0f, 5f)] public float normalScale = 2.0f;
        [Range(1, 3)] public int thickness = 2;
        [Range(0f, 1f)] public float overlay = 1f;
    }

    class SobelPass : ScriptableRenderPass
    {
        private readonly Settings _settings;
        private Material _mat;
        private RTHandle _temp;
        const string kTag = "Moebius Sobel Outline";

        public SobelPass(Settings settings)
        {
            _settings = settings;
            profilingSampler = new ProfilingSampler(kTag);
        }

        public bool Init(Material mat)
        {
            _mat = mat;
            return _mat != null;
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            ConfigureInput(ScriptableRenderPassInput.Depth | ScriptableRenderPassInput.Normal);

            var desc = renderingData.cameraData.cameraTargetDescriptor;
            desc.depthBufferBits = 0;
            RenderingUtils.ReAllocateIfNeeded(ref _temp, desc, FilterMode.Bilinear, TextureWrapMode.Clamp, name: "_SobelOutlineRT");
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (_mat == null) return;
            var cmd = CommandBufferPool.Get(kTag);
            using (new ProfilingScope(cmd, profilingSampler))
            {
                _mat.SetColor("_EdgeColor", _settings.edgeColor);
                _mat.SetFloat("_EdgeThreshold", _settings.edgeThreshold);
                _mat.SetFloat("_EdgeSoftness", Mathf.Max(0.0001f, _settings.edgeSoftness));
                _mat.SetFloat("_DepthScale", _settings.depthScale);
                _mat.SetFloat("_NormalScale", _settings.normalScale);
                _mat.SetFloat("_Overlay", _settings.overlay);
                _mat.SetInt("_Thickness", Mathf.Clamp(_settings.thickness, 1, 3));

                var src = renderingData.cameraData.renderer.cameraColorTargetHandle;

                //          measure texture size
                int w = src.rt.width;
                int h = src.rt.height;
                _mat.SetVector("_TexelSize", new Vector4(1f / Mathf.Max(1,w), 1f / Mathf.Max(1,h), w, h));

                // Blit: src → temp → src
                Blitter.BlitCameraTexture(cmd, src, _temp, _mat, 0);
                Blitter.BlitCameraTexture(cmd, _temp, src);
            }
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void OnCameraCleanup(CommandBuffer cmd) { }
    }

    public Settings settings = new Settings();
    private SobelPass _pass;
    private Material _mat;

    public override void Create()
    {
    
        if (settings.sobelShader == null)
            settings.sobelShader = Shader.Find("Moebius/SobelOutline_Fullscreen");

        if (settings.sobelShader != null && _mat == null)
            _mat = CoreUtils.CreateEngineMaterial(settings.sobelShader);

        _pass = new SobelPass(settings) { renderPassEvent = settings.injectionPoint };
        if (_mat != null) _pass.Init(_mat);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (_mat == null)
        {

            if (settings.sobelShader == null) 
                settings.sobelShader = Shader.Find("Moebius/SobelOutline_Fullscreen");
            if (settings.sobelShader != null) 
                _mat = CoreUtils.CreateEngineMaterial(settings.sobelShader);
            if (_pass != null && _mat != null) 
                _pass.Init(_mat);
        }
        if (_pass != null && _mat != null)
            renderer.EnqueuePass(_pass);
    }

    protected override void Dispose(bool disposing)
    {
        CoreUtils.Destroy(_mat);
    }
}