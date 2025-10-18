Shader "Moebius/UnlitTexture_Stencil"
{
    Properties{
        _BaseMap("Base (RGB)", 2D) = "white" {}
        _BaseColor("Color", Color) = (1,1,1,1)
        _MaskRef("Stencil Ref", Range(0,255)) = 1
    }
    SubShader{
        Tags{ "RenderPipeline"="UniversalPipeline" "Queue"="Geometry" "RenderType"="Opaque" }
        Pass{
            Name "ForwardUnlit"
            Tags{ "LightMode"="UniversalForward" }
            ZTest LEqual ZWrite On Cull Back
            // make player as MaskRef
            Stencil{ Ref [_MaskRef] Comp Always Pass Replace WriteMask 255 }
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            TEXTURE2D(_BaseMap); SAMPLER(sampler_BaseMap);
            CBUFFER_START(UnityPerMaterial)
                float4 _BaseColor; float4 _BaseMap_ST;
            CBUFFER_END
            struct A{ float4 positionOS:POSITION; float2 uv:TEXCOORD0; };
            struct V{ float4 pos:SV_POSITION; float2 uv:TEXCOORD0; };
            V vert(A i){ V o; o.pos=TransformObjectToHClip(i.positionOS.xyz); o.uv=TRANSFORM_TEX(i.uv,_BaseMap); return o; }
            half4 frag(V i):SV_Target{ return SAMPLE_TEXTURE2D(_BaseMap,sampler_BaseMap,i.uv) * _BaseColor; }
            ENDHLSL
        }
    }
    FallBack Off
}
