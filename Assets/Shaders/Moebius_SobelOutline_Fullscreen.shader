Shader "Moebius/SobelOutline_Fullscreen"
{
    Properties
    {
        _EdgeColor     ("Edge Color", Color)                = (0.07, 0.07, 0.07, 1)
        _EdgeThreshold ("Edge Threshold", Range(0,1))       = 0.10
        _EdgeSoftness  ("Edge Softness",  Range(0.001,0.5)) = 0.05
        _DepthScale    ("Depth Edge Scale", Range(0,5))     = 3.0
        _NormalScale   ("Normal Edge Scale", Range(0,5))    = 2.0
        _Thickness     ("Thickness (1..3)", Range(1,3))     = 2
        _Overlay       ("Overlay (1=On,0=LinesOnly)", Range(0,1)) = 1

        // read player value avoid ouline overlay player
        _MaskRef    ("Mask Stencil Ref", Range(0,255))  = 1
        _MaskRead   ("Mask ReadMask",     Range(0,255))  = 255
    }

    SubShader
    {
        Tags{ "RenderPipeline"="UniversalPipeline" }
        Cull Off ZWrite Off ZTest Always

        //Pass 0：orinal version
        Pass
        {
            Name "SobelOutline_Fullscreen"

            HLSLPROGRAM
            #pragma vertex   Vert
            #pragma fragment Frag
            #pragma target   3.5
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            TEXTURE2D_X(_CameraDepthTexture);
            TEXTURE2D_X(_CameraNormalsTexture);

            CBUFFER_START(UnityPerMaterial)
                float4 _EdgeColor; float _EdgeThreshold; float _EdgeSoftness;
                float _DepthScale; float _NormalScale; float _Thickness; float _Overlay;
                float  _MaskRef;   float _MaskRead;
            CBUFFER_END

            struct VSQ { float4 pos:SV_POSITION; float2 uv:TEXCOORD0; };
            VSQ Vert(uint id:SV_VertexID){ VSQ o; o.pos=GetFullScreenTriangleVertexPosition(id); o.uv=GetFullScreenTriangleTexCoord(id); return o; }

            float SampleLinearDepth(float2 uv){ float raw=SAMPLE_TEXTURE2D_X(_CameraDepthTexture,sampler_LinearClamp,uv).r; return Linear01Depth(raw,_ZBufferParams); }
            float3 DecodeVSNormal(float2 uv){ float3 enc=SAMPLE_TEXTURE2D_X(_CameraNormalsTexture,sampler_LinearClamp,uv).xyz; return normalize(enc*2-1); }

            float SobelFromScalars(float s00,float s10,float s20,float s01,float s21,float s02,float s12,float s22){
                float gx=(s20+2*s21+s22)-(s00+2*s01+s02);
                float gy=(s02+2*s12+s22)-(s00+2*s10+s20);
                return sqrt(gx*gx+gy*gy);
            }

            float SobelDepth(float2 uv,float2 texel){
                float s00=SampleLinearDepth(uv+texel*float2(-1,-1));
                float s10=SampleLinearDepth(uv+texel*float2( 0,-1));
                float s20=SampleLinearDepth(uv+texel*float2( 1,-1));
                float s01=SampleLinearDepth(uv+texel*float2(-1, 0));
                float s21=SampleLinearDepth(uv+texel*float2( 1, 0));
                float s02=SampleLinearDepth(uv+texel*float2(-1, 1));
                float s12=SampleLinearDepth(uv+texel*float2( 0, 1));
                float s22=SampleLinearDepth(uv+texel*float2( 1, 1));
                return SobelFromScalars(s00,s10,s20,s01,s21,s02,s12,s22);
            }

            float SobelNormal(float2 uv,float2 texel){
                float3 n00=DecodeVSNormal(uv+texel*float2(-1,-1));
                float3 n10=DecodeVSNormal(uv+texel*float2( 0,-1));
                float3 n20=DecodeVSNormal(uv+texel*float2( 1,-1));
                float3 n01=DecodeVSNormal(uv+texel*float2(-1, 0));
                float3 n21=DecodeVSNormal(uv+texel*float2( 1, 0));
                float3 n02=DecodeVSNormal(uv+texel*float2(-1, 1));
                float3 n12=DecodeVSNormal(uv+texel*float2( 0, 1));
                float3 n22=DecodeVSNormal(uv+texel*float2( 1, 1));
               
                float s00=(n00.x*0.5+0.5 + n00.y*0.5+0.5 + n00.z*0.5+0.5)/3.0;
                float s10=(n10.x*0.5+0.5 + n10.y*0.5+0.5 + n10.z*0.5+0.5)/3.0;
                float s20=(n20.x*0.5+0.5 + n20.y*0.5+0.5 + n20.z*0.5+0.5)/3.0;
                float s01=(n01.x*0.5+0.5 + n01.y*0.5+0.5 + n01.z*0.5+0.5)/3.0;
                float s21=(n21.x*0.5+0.5 + n21.y*0.5+0.5 + n21.z*0.5+0.5)/3.0;
                float s02=(n02.x*0.5+0.5 + n02.y*0.5+0.5 + n02.z*0.5+0.5)/3.0;
                float s12=(n12.x*0.5+0.5 + n12.y*0.5+0.5 + n12.z*0.5+0.5)/3.0;
                float s22=(n22.x*0.5+0.5 + n22.y*0.5+0.5 + n22.z*0.5+0.5)/3.0;
                return SobelFromScalars(s00,s10,s20,s01,s21,s02,s12,s22);
            }

            half4 Frag(VSQ i):SV_Target
            {
                float2 uv=i.uv; float2 texel=1.0/_ScreenParams.xy;
                float eDepth=_DepthScale*SobelDepth(uv,texel);
                float eNorm =_NormalScale*SobelNormal(uv,texel);

                if(_Thickness>1.5){ float2 t=texel*1.5; eDepth=max(eDepth,_DepthScale*SobelDepth(uv,t)); eNorm=max(eNorm,_NormalScale*SobelNormal(uv,t)); }
                if(_Thickness>2.5){ float2 t=texel*2.0; eDepth=max(eDepth,_DepthScale*SobelDepth(uv,t)); eNorm=max(eNorm,_NormalScale*SobelNormal(uv,t)); }

                float edgeRaw=eDepth+eNorm;
                float e=saturate((edgeRaw-_EdgeThreshold)/max(_EdgeSoftness,1e-5));
                float3 src=SAMPLE_TEXTURE2D_X(_BlitTexture,sampler_LinearClamp,uv).rgb;
                float3 edgeCol=_EdgeColor.rgb;
                return (_Overlay>0.5) ? float4(lerp(src,edgeCol,e),1) : float4(lerp(1.0.xxx,edgeCol,e),1);
            }
            ENDHLSL
        }

        // Pass 1：avoid overlay to player
        Pass
        {
            Name "SobelOutline_ExcludeStencil"

            // only draw outline on mask
            Stencil
            {
                Ref      [_MaskRef]
                ReadMask [_MaskRead]
                Comp NotEqual
            }

            HLSLPROGRAM
            #pragma vertex   Vert
            #pragma fragment Frag
            #pragma target   3.5
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            TEXTURE2D_X(_CameraDepthTexture);
            TEXTURE2D_X(_CameraNormalsTexture);

            CBUFFER_START(UnityPerMaterial)
                float4 _EdgeColor; float _EdgeThreshold; float _EdgeSoftness;
                float _DepthScale; float _NormalScale; float _Thickness; float _Overlay;
                float  _MaskRef;   float _MaskRead;
            CBUFFER_END

            struct VSQ { float4 pos:SV_POSITION; float2 uv:TEXCOORD0; };
            VSQ Vert(uint id:SV_VertexID){ VSQ o; o.pos=GetFullScreenTriangleVertexPosition(id); o.uv=GetFullScreenTriangleTexCoord(id); return o; }

            float SampleLinearDepth(float2 uv){ float raw=SAMPLE_TEXTURE2D_X(_CameraDepthTexture,sampler_LinearClamp,uv).r; return Linear01Depth(raw,_ZBufferParams); }
            float3 DecodeVSNormal(float2 uv){ float3 enc=SAMPLE_TEXTURE2D_X(_CameraNormalsTexture,sampler_LinearClamp,uv).xyz; return normalize(enc*2-1); }
            float SobelFromScalars(float s00,float s10,float s20,float s01,float s21,float s02,float s12,float s22){ float gx=(s20+2*s21+s22)-(s00+2*s01+s02); float gy=(s02+2*s12+s22)-(s00+2*s10+s20); return sqrt(gx*gx+gy*gy); }
            float SobelDepth(float2 uv,float2 texel){ float s00=SampleLinearDepth(uv+texel*float2(-1,-1)); float s10=SampleLinearDepth(uv+texel*float2(0,-1)); float s20=SampleLinearDepth(uv+texel*float2(1,-1)); float s01=SampleLinearDepth(uv+texel*float2(-1,0)); float s21=SampleLinearDepth(uv+texel*float2(1,0)); float s02=SampleLinearDepth(uv+texel*float2(-1,1)); float s12=SampleLinearDepth(uv+texel*float2(0,1)); float s22=SampleLinearDepth(uv+texel*float2(1,1)); return SobelFromScalars(s00,s10,s20,s01,s21,s02,s12,s22); }
            float SobelNormal(float2 uv,float2 texel){ float3 n00=DecodeVSNormal(uv+texel*float2(-1,-1)); float3 n10=DecodeVSNormal(uv+texel*float2(0,-1)); float3 n20=DecodeVSNormal(uv+texel*float2(1,-1)); float3 n01=DecodeVSNormal(uv+texel*float2(-1,0)); float3 n21=DecodeVSNormal(uv+texel*float2(1,0)); float3 n02=DecodeVSNormal(uv+texel*float2(-1,1)); float3 n12=DecodeVSNormal(uv+texel*float2(0,1)); float3 n22=DecodeVSNormal(uv+texel*float2(1,1)); float s00=(n00.x*0.5+0.5+n00.y*0.5+0.5+n00.z*0.5+0.5)/3.0; float s10=(n10.x*0.5+0.5+n10.y*0.5+0.5+n10.z*0.5+0.5)/3.0; float s20=(n20.x*0.5+0.5+n20.y*0.5+0.5+n20.z*0.5+0.5)/3.0; float s01=(n01.x*0.5+0.5+n01.y*0.5+0.5+n01.z*0.5+0.5)/3.0; float s21=(n21.x*0.5+0.5+n21.y*0.5+0.5+n21.z*0.5+0.5)/3.0; float s02=(n02.x*0.5+0.5+n02.y*0.5+0.5+n02.z*0.5+0.5)/3.0; float s12=(n12.x*0.5+0.5+n12.y*0.5+0.5+n12.z*0.5+0.5)/3.0; float s22=(n22.x*0.5+0.5+n22.y*0.5+0.5+n22.z*0.5+0.5)/3.0; return SobelFromScalars(s00,s10,s20,s01,s21,s02,s12,s22); }

            half4 Frag(VSQ i):SV_Target
            {
                float2 uv=i.uv; float2 texel=1.0/_ScreenParams.xy;
                float e = _DepthScale*SobelDepth(uv,texel) + _NormalScale*SobelNormal(uv,texel);
                if(_Thickness>1.5){ float2 t=texel*1.5; e=max(e,_DepthScale*SobelDepth(uv,t)+_NormalScale*SobelNormal(uv,t)); }
                if(_Thickness>2.5){ float2 t=texel*2.0; e=max(e,_DepthScale*SobelDepth(uv,t)+_NormalScale*SobelNormal(uv,t)); }
                float m=saturate((e-_EdgeThreshold)/max(_EdgeSoftness,1e-5));
                float3 src=SAMPLE_TEXTURE2D_X(_BlitTexture,sampler_LinearClamp,uv).rgb;
                float3 edgeCol=_EdgeColor.rgb;
                return (_Overlay>0.5)? float4(lerp(src,edgeCol,m),1) : float4(lerp(1.0.xxx,edgeCol,m),1);
            }
            ENDHLSL
        }
    }
    Fallback Off
}

// best version no overlay issue