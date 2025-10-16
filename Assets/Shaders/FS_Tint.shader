Shader "Moebius/Debug/FS_Tint"
{
    Properties
    {
        // all colour change
        _Tint   ("Tint Color", Color) = (1,0,1,1)
        _Amount ("Tint Amount", Range(0,1)) = 0.6

        // saturations
        _Saturation ("Global Saturation (0..1)", Range(0,1)) = 0.6

        // noise texture
        _UseNoiseTex    ("Use Noise Texture (0/1)", Float) = 1
        _NoiseTex       ("Noise Texture (RGB)", 2D) = "white" {}
        _NoiseTiling    ("Noise Tiling (x,y)", Vector) = (200,200,0,0)
        _NoiseOffset    ("Noise Offset (x,y)", Vector) = (0,0,0,0)
        _NoiseSpeed     ("Noise Scroll (x,y)", Vector) = (0,0,0,0)
        _NoiseIntensity ("Noise Intensity", Range(0,1)) = 0.25
        _Monochrome     ("Monochrome Noise (0/1)", Float) = 1
    }

    SubShader
    {
        Tags{ "RenderPipeline"="UniversalPipeline" }
        ZWrite Off Cull Off ZTest Always

        Pass
        {
            Name "FS_Tint_NoiseDesat"
            HLSLPROGRAM
            #pragma target 3.5
            #pragma vertex   Vert
            #pragma fragment Frag

           
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            
            float4 _Tint;
            float  _Amount;

            float  _Saturation;

            float  _UseNoiseTex;
            float4 _NoiseTiling; // xy
            float4 _NoiseOffset; // xy
            float4 _NoiseSpeed;  // xy
            float  _NoiseIntensity;
            float  _Monochrome;

        
            TEXTURE2D(_NoiseTex);
            SAMPLER(sampler_NoiseTex);

            struct VSOut { float4 pos:SV_Position; float2 uv:TEXCOORD0; };

            VSOut Vert(uint id: SV_VertexID)
            {
                VSOut o;
                o.pos = GetFullScreenTriangleVertexPosition(id);
                o.uv  = GetFullScreenTriangleTexCoord(id);
                return o;
            }

            float3 ApplySaturation(float3 col, float sat)
            {
                // Rec.601
                float luma = dot(col, float3(0.299, 0.587, 0.114));
                return lerp(luma.xxx, col, saturate(sat));
            }

            float3 SampleNoiseTex(float2 uv, float t)
            {
                float2 nUV = uv * _NoiseTiling.xy + _NoiseOffset.xy + _NoiseSpeed.xy * t;
                float3 n = SAMPLE_TEXTURE2D(_NoiseTex, sampler_NoiseTex, nUV).rgb;

                if (_Monochrome > 0.5)
                {
                    float g = dot(n, float3(0.3333, 0.3333, 0.3333));
                    n = g.xxx;
                }

                return n * 2.0 - 1.0;
            }

            float4 Frag(VSOut i) : SV_Target
            {
      
                float3 src = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, i.uv).rgb;

                //  apply color change
                float3 col = lerp(src, _Tint.rgb, saturate(_Amount));

                // apply saturation
                col = ApplySaturation(col, _Saturation);

                // noise texture
                if (_UseNoiseTex > 0.5)
                {
                    float t = _Time.y;
                    float3 n = SampleNoiseTex(i.uv, t);
                 
                    float3 grain = 1.0 + n * (_NoiseIntensity * 0.25);
                    col *= grain;
                }

             
                col = saturate(col);
                return float4(col, 1);
            }
            ENDHLSL
        }
    }
    FallBack Off
}