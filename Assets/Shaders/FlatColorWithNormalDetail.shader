Shader "Moebius/FlatColorWithNormalDetail"
{
    Properties
    {
        _BaseColor("Base Color", Color) = (0.95, 0.95, 0.95, 1)
        _UseGradient("Use Gradient (0/1)", Float) = 0
        _TopColor("Gradient Top", Color) = (1,1,1,1)
        _BottomColor("Gradient Bottom", Color) = (0.8,0.8,0.8,1)
        _GradientHeight("Gradient Height", Float) = 2.0
        _GradientOffset("Gradient Offset (World Y)", Float) = 0.0

        _NormalDetailMap("Normal Detail (Tangent-Space)", 2D) = "bump" {}
        _NormalScale("Normal Scale", Range(0,2)) = 1.0
        _UVScale("UV Scale", Float) = 1.0

        _MainLightDir("Main Light Dir (world, normalized)", Vector) = (0,1,0,0)
        _ShadowStrength("Shadow Strength", Range(0,1)) = 0.5

        _SpecMarkEnable   ("Spec Mark Enable (0/1)", Float) = 1
        _SpecColor        ("Spec Color", Color) = (1,1,1,1)
        _SpecThreshold    ("Spec Threshold (N·L)", Range(0,1)) = 0.85
        _SpecMarkFeather  ("Spec Mark Feather", Range(0,0.5)) = 0.08
        _SpecIntensity    ("Spec Intensity", Range(0,1)) = 0.7

        _SpecShadingEnable("Spec Shading Enable (0/1)", Float) = 0
        _SpecWidth        ("Spec Band Width (N·H)", Range(0.001,0.5)) = 0.08
        _SpecBlend        ("Spec Blend", Range(0,1)) = 0.5

        _RimStrength      ("Rim Strength", Range(0,1)) = 0.0
        _RimPower         ("Rim Power", Range(0.5,8)) = 2.0

        _SpecOutlineEnable   ("Spec Highlight Outline (0/1)", Float) = 1
        _SpecOutlineColor    ("Spec Outline Color", Color) = (0,0,0,1)
        _SpecOutlineThickness("Spec Outline Thickness", Range(0,3)) = 1.0
        _SpecOutlineStrength ("Spec Outline Strength", Range(0,1)) = 1.0

        _AlbedoTex   ("Albedo (RGB)", 2D) = "white" {}
        _UseAlbedo   ("Use Albedo (0/1)", Float) = 0
        _AlbedoDesat ("Albedo Desaturation", Range(0,1)) = 0.7

        _DiffuseStrength ("Diffuse Strength (0=flat,1=Lambert)", Range(0,1)) = 0.6
        _DebugMode("Debug Mode (0=Off,1=Normals,2=SpecMark)", Float) = 0

        //  Albedo Uv world texture
        _AlbedoUVMode      ("Albedo UV Mode (0=MeshUV,1=WorldXZ,2=Triplanar)", Float) = 0
        _AlbedoWorldTiling ("Albedo World Tiling (per meter)", Float) = 1
        _AlbedoWorldRotDeg ("Albedo World Rotation (deg)", Float) = 0
        _AlbedoWorldOffset ("Albedo World Offset (XZ)", Vector) = (0,0,0,0)

        // normal with world albedo uv texture
        _NormalFollowAlbedo("Normal Map Follow Albedo UV (0/1)", Float) = 1
    }

    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Opaque" "Queue"="Geometry" }

        Pass
        {
            Name "UniversalForward"
            Tags { "LightMode"="UniversalForward" }

            Cull Back
            ZWrite On
            ZTest LEqual

            HLSLPROGRAM
            #pragma vertex   Vert
            #pragma fragment Frag
            #pragma target   3.5

            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
            #pragma multi_compile _ _SHADOWS_SOFT
            #pragma multi_compile_fragment _ _SHADOWS_SOFT_LOW _SHADOWS_SOFT_MEDIUM _SHADOWS_SOFT_HIGH

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseColor;
                float  _UseGradient;
                float4 _TopColor;
                float4 _BottomColor;
                float  _GradientHeight;
                float  _GradientOffset;

                float  _NormalScale;
                float  _UVScale;

                float4 _MainLightDir;
                float  _ShadowStrength;

                float  _SpecMarkEnable;
                float4 _SpecColor;
                float  _SpecThreshold;
                float  _SpecMarkFeather;
                float  _SpecIntensity;

                float  _SpecShadingEnable;
                float  _SpecWidth;
                float  _SpecBlend;

                float  _RimStrength;
                float  _RimPower;

                float  _SpecOutlineEnable;
                float4 _SpecOutlineColor;
                float  _SpecOutlineThickness;
                float  _SpecOutlineStrength;

                float  _UseAlbedo;
                float  _AlbedoDesat;
                float4 _AlbedoTex_ST;

                float  _DiffuseStrength;
                float  _DebugMode;

                float  _AlbedoUVMode;
                float  _AlbedoWorldTiling;
                float  _AlbedoWorldRotDeg;
                float4 _AlbedoWorldOffset;

                float  _NormalFollowAlbedo; 
            CBUFFER_END

            TEXTURE2D(_NormalDetailMap); SAMPLER(sampler_NormalDetailMap);
            TEXTURE2D(_AlbedoTex);       SAMPLER(sampler_AlbedoTex);

            struct Attributes {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
                float4 tangentOS  : TANGENT;
                float2 uv         : TEXCOORD0;
            };
            struct Varyings {
                float4 positionHCS : SV_POSITION;
                float3 positionWS  : TEXCOORD0;
                float2 uv          : TEXCOORD1;
                float3 normalWS    : TEXCOORD2;
                float3 tangentWS   : TEXCOORD3;
                float3 bitangentWS : TEXCOORD4;
                float4 shadowCoord : TEXCOORD5;
            };

            Varyings Vert(Attributes IN)
            {
                Varyings o;
                VertexPositionInputs v = GetVertexPositionInputs(IN.positionOS.xyz);
                o.positionHCS = v.positionCS;
                o.positionWS  = v.positionWS;
                o.uv          = IN.uv * _UVScale;

                float3 nWS = TransformObjectToWorldNormal(IN.normalOS);
                float3 tWS = normalize(TransformObjectToWorldDir(IN.tangentOS.xyz));
                float  tSign = IN.tangentOS.w * GetOddNegativeScale();
                float3 bWS = normalize(cross(nWS, tWS) * tSign);
                o.normalWS    = nWS;
                o.tangentWS   = tWS;
                o.bitangentWS = bWS;

                #ifdef _MAIN_LIGHT_SHADOWS_SCREEN
                    o.shadowCoord = ComputeScreenPos(o.positionHCS);
                #else
                    o.shadowCoord = TransformWorldToShadowCoord(o.positionWS);
                #endif
                return o;
            }

            float3 TangentToWorld(float3 nTS, float3 tWS, float3 bWS, float3 nWS)
            {
                float3x3 TBN = float3x3(tWS, bWS, nWS);
                return normalize(mul(nTS, TBN));
            }

            // Albedo UV 
            float2 GetAlbedoUV_Mesh(float2 uv) { return uv * _AlbedoTex_ST.xy + _AlbedoTex_ST.zw; }
            float2 GetAlbedoUV_WorldXZ(float3 positionWS)
            {
                float rad = radians(_AlbedoWorldRotDeg);
                float c = cos(rad), s = sin(rad);
                float2 xz = mul(float2x2(c,-s,s,c), positionWS.xz);
                return xz * _AlbedoWorldTiling + _AlbedoWorldOffset.xy;
            }
            float3 SampleAlbedo_Triplanar(float3 positionWS, float3 nWS)
            {
                float3 w = pow(abs(nWS), 4.0);
                w /= max(w.x + w.y + w.z, 1e-5);

                float2 uvX = GetAlbedoUV_WorldXZ(positionWS.zyx);
                float2 uvY = GetAlbedoUV_WorldXZ(positionWS.xzz);
                float2 uvZ = GetAlbedoUV_WorldXZ(positionWS.xyx);

                float3 sx = SAMPLE_TEXTURE2D(_AlbedoTex, sampler_AlbedoTex, uvX).rgb;
                float3 sy = SAMPLE_TEXTURE2D(_AlbedoTex, sampler_AlbedoTex, uvY).rgb;
                float3 sz = SAMPLE_TEXTURE2D(_AlbedoTex, sampler_AlbedoTex, uvZ).rgb;
                return sx*w.x + sy*w.y + sz*w.z;
            }

            // normal with Albedo 's xz texture
            float3 SampleNormal_WorldXZ(float3 positionWS, float3 nBase)
            {
                float2 uvA = GetAlbedoUV_WorldXZ(positionWS);
                float3 nTS = UnpackNormalScale(SAMPLE_TEXTURE2D(_NormalDetailMap, sampler_NormalDetailMap, uvA), _NormalScale);

                // 把贴图法线从“XZ 贴图坐标系”旋转到世界
                // move normal texture to world
                float rad = radians(_AlbedoWorldRotDeg);
                float c = cos(rad), s = sin(rad);
                float3 T = normalize(float3( c, 0,-s)); // +U direction
                float3 B = normalize(float3( s, 0, c)); // +V direction
                float3 N = float3(0,1,0);               // world Y

                float3 nWS = normalize(T * nTS.x + B * nTS.y + N * nTS.z);
                return nWS;
            }

            half4 Frag(Varyings IN) : SV_Target
            {
                // normal 
                float3 nWS0 = normalize(IN.normalWS);

                // detail normal
                float3 nDetailWS;
                if (_NormalFollowAlbedo > 0.5 && _AlbedoUVMode > 0.5) // WorldXZ or Triplanar
                {
                    // match World XZ；Triplanar 也
                    nDetailWS = SampleNormal_WorldXZ(IN.positionWS, nWS0);
                }
                else
                {
                    float3 nTS  = UnpackNormalScale(SAMPLE_TEXTURE2D(_NormalDetailMap, sampler_NormalDetailMap, IN.uv), _NormalScale);
                    nDetailWS   = TangentToWorld(nTS, IN.tangentWS, IN.bitangentWS, nWS0);
                }

                float3 n = normalize(lerp(nWS0, nDetailWS, saturate(_NormalScale)));

                float3 L = normalize(_MainLightDir.xyz);
                float  ndotl = saturate(dot(n, L));
                float3 V = normalize(_WorldSpaceCameraPos.xyz - IN.positionWS);

                if (_DebugMode > 0.5 && _DebugMode < 1.5)
                    return half4(normalize(mul((float3x3)UNITY_MATRIX_V, n))*0.5+0.5,1);

                // base color
                float3 col = _BaseColor.rgb;
                if (_UseGradient > 0.5)
                {
                    float h = max(_GradientHeight, 1e-5);
                    float t = saturate((IN.positionWS.y - _GradientOffset) / h);
                    col = lerp(_BottomColor.rgb, _TopColor.rgb, t);
                }

                // Albedo
                if (_UseAlbedo > 0.5)
                {
                    float3 texRGB;
                    if (_AlbedoUVMode < 0.5) {
                        texRGB = SAMPLE_TEXTURE2D(_AlbedoTex, sampler_AlbedoTex, GetAlbedoUV_Mesh(IN.uv)).rgb;
                    } else if (_AlbedoUVMode < 1.5) {
                        texRGB = SAMPLE_TEXTURE2D(_AlbedoTex, sampler_AlbedoTex, GetAlbedoUV_WorldXZ(IN.positionWS)).rgb;
                    } else {
                        texRGB = SampleAlbedo_Triplanar(IN.positionWS, n);
                    }
                    float  luma = dot(texRGB, float3(0.299,0.587,0.114));
                    float3 desat = lerp(luma.xxx, texRGB, 1.0 - _AlbedoDesat);
                    col *= desat;
                }

                // flat 
                col *= lerp(1.0, ndotl, _DiffuseStrength);

                // high light
                float markMask = 0.0;
                if (_SpecMarkEnable > 0.5)
                {
                    float a = _SpecThreshold - _SpecMarkFeather;
                    float b = _SpecThreshold + _SpecMarkFeather;
                    markMask = smoothstep(a, b, ndotl);
                    col = lerp(col, _SpecColor.rgb, markMask * _SpecIntensity);
                }

                // lightlight 
                if (_SpecShadingEnable > 0.5 && _SpecBlend > 0.0)
                {
                    float3 H = normalize(L + V);
                    float  nh = saturate(dot(n, H));
                    float  specBand = smoothstep(_SpecThreshold, _SpecThreshold + _SpecWidth, nh);
                    col = lerp(col, 1.0.xxx, specBand * _SpecBlend);
                }

                // outline
                if (_SpecOutlineEnable > 0.5 && _SpecOutlineStrength > 0.0 && _SpecOutlineThickness > 0.0)
                {
                    float w = max(1e-4, fwidth(ndotl)) * _SpecOutlineThickness;
                    float eL = smoothstep(_SpecThreshold - w, _SpecThreshold, ndotl);
                    float eR = 1.0 - smoothstep(_SpecThreshold, _SpecThreshold + w, ndotl);
                    float edgeMask = eL * eR;
                    edgeMask *= (_SpecMarkEnable > 0.5) ? 1.0 : 0.0;
                    col = lerp(col, _SpecOutlineColor.rgb, edgeMask * _SpecOutlineStrength);
                }

                // light
                if (_RimStrength > 0.001)
                {
                    float rim = pow(1.0 - saturate(dot(n, V)), _RimPower);
                    col = lerp(col, 1.0.xxx, rim * _RimStrength);
                }

                // shadow
                float4 sc = IN.shadowCoord;
                Light ml = GetMainLight(sc);
                float shadow = lerp(1.0, ml.shadowAttenuation, _ShadowStrength);
                col *= shadow;

                if (_DebugMode > 1.5)
                {
                    float markMaskDbg = (_SpecMarkEnable > 0.5) ? markMask : step(_SpecThreshold, ndotl);
                    return half4(lerp(_BaseColor.rgb, 1.0.xxx, markMaskDbg), 1);
                }

                return half4(col,1);
            }
            ENDHLSL
        }

        // all passes
        Pass
        {
            Name "ShadowCaster"
            Tags { "LightMode"="ShadowCaster" }
            ZWrite On
            ZTest LEqual
            ColorMask 0
            Cull Back
            HLSLPROGRAM
            #pragma vertex ShadowPassVertex
            #pragma fragment ShadowPassFragment
            #pragma target 3.5
            #pragma multi_compile_vertex _ _CASTING_PUNCTUAL_LIGHT_SHADOW
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            float3 _LightDirection; float3 _LightPosition;
            struct A { float4 positionOS:POSITION; float3 normalOS:NORMAL; };
            struct V { float4 positionCS:SV_POSITION; };
            float4 GetShadowPositionHClip(A i){
                float3 pWS = TransformObjectToWorld(i.positionOS.xyz);
                float3 nWS = TransformObjectToWorldNormal(i.normalOS);
                #if _CASTING_PUNCTUAL_LIGHT_SHADOW
                    float3 L = normalize(_LightPosition - pWS);
                #else
                    float3 L = _LightDirection;
                #endif
                float4 cs = TransformWorldToHClip(ApplyShadowBias(pWS, nWS, L));
                #if UNITY_REVERSED_Z
                    cs.z = min(cs.z, UNITY_NEAR_CLIP_VALUE);
                #else
                    cs.z = max(cs.z, UNITY_NEAR_CLIP_VALUE);
                #endif
                return cs;
            }
            V ShadowPassVertex(A i){ V o; o.positionCS = GetShadowPositionHClip(i); return o; }
            half4 ShadowPassFragment(V i):SV_TARGET { return 0; }
            ENDHLSL
        }

        Pass
        {
            Name "DepthOnly"
            Tags { "LightMode"="DepthOnly" }
            Cull Back
            ZWrite On
            ColorMask 0
            HLSLPROGRAM
            #pragma vertex   Vert
            #pragma fragment Frag
            #pragma target   3.5
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            struct A { float4 positionOS:POSITION; };
            struct V { float4 positionHCS:SV_POSITION; };
            V Vert(A i){ V o; o.positionHCS = TransformObjectToHClip(i.positionOS.xyz); return o; }
            void Frag() {}
            ENDHLSL
        }

        Pass
        {
            Name "DepthNormals"
            Tags { "LightMode"="DepthNormals" }
            Cull Back
            ZWrite On
            HLSLPROGRAM
            #pragma vertex   Vert
            #pragma fragment Frag
            #pragma target   3.5
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl"
            CBUFFER_START(UnityPerMaterial)
                float  _NormalScale;
                float  _UVScale;
                float4 _MainLightDir;
                float  _SpecThreshold;
                float  _SpecBlend;
                float  _AlbedoUVMode;
                float  _AlbedoWorldRotDeg;
                float  _AlbedoWorldTiling;
                float4 _AlbedoWorldOffset;
                float  _NormalFollowAlbedo;
                float4 _AlbedoTex_ST; 
            CBUFFER_END
            TEXTURE2D(_NormalDetailMap); SAMPLER(sampler_NormalDetailMap);
            struct A { float4 positionOS:POSITION; float3 normalOS:NORMAL; float4 tangentOS:TANGENT; float2 uv:TEXCOORD0; };
            struct V { float4 positionHCS:SV_POSITION; float3 normalWS:TEXCOORD0; float3 tangentWS:TEXCOORD1; float3 bitangentWS:TEXCOORD2; float2 uv:TEXCOORD3; float3 positionWS:TEXCOORD4; };
            V Vert(A IN){
                V OUT;
                VertexPositionInputs v = GetVertexPositionInputs(IN.positionOS.xyz);
                OUT.positionHCS = v.positionCS;
                OUT.positionWS  = v.positionWS;
                float3 nWS = TransformObjectToWorldNormal(IN.normalOS);
                float3 tWS = normalize(TransformObjectToWorldDir(IN.tangentOS.xyz));
                float  tSign = IN.tangentOS.w * GetOddNegativeScale();
                float3 bWS = normalize(cross(nWS, tWS) * tSign);
                OUT.normalWS=nWS; OUT.tangentWS=tWS; OUT.bitangentWS=bWS; OUT.uv = IN.uv * _UVScale;
                return OUT;
            }
            float2 GetAlbedoUV_WorldXZ(float3 positionWS){
                float rad = radians(_AlbedoWorldRotDeg);
                float c = cos(rad), s = sin(rad);
                float2 xz = mul(float2x2(c,-s,s,c), positionWS.xz);
                return xz * _AlbedoWorldTiling + _AlbedoWorldOffset.xy;
            }
            float3 TangentToWorld(float3 nTS,float3 tWS,float3 bWS,float3 nWS){ return normalize(tWS*nTS.x + bWS*nTS.y + nWS*nTS.z); }
            float4 Frag(V IN):SV_Target
            {
                float3 nWS0 = normalize(IN.normalWS);
                float3 nDetailWS;
                if (_NormalFollowAlbedo > 0.5 && _AlbedoUVMode > 0.5){
                    float2 uvA = GetAlbedoUV_WorldXZ(IN.positionWS);
                    float3 nTS = UnpackNormalScale(SAMPLE_TEXTURE2D(_NormalDetailMap, sampler_NormalDetailMap, uvA), _NormalScale);
                    float rad = radians(_AlbedoWorldRotDeg);
                    float c = cos(rad), s = sin(rad);
                    float3 T = normalize(float3( c,0,-s));
                    float3 B = normalize(float3( s,0, c));
                    float3 N = float3(0,1,0);
                    nDetailWS = normalize(T*nTS.x + B*nTS.y + N*nTS.z);
                } else {
                    float3 nTS = UnpackNormalScale(SAMPLE_TEXTURE2D(_NormalDetailMap, sampler_NormalDetailMap, IN.uv), _NormalScale);
                    nDetailWS  = TangentToWorld(nTS, IN.tangentWS, IN.bitangentWS, nWS0);
                }
                float3 nFinal  = normalize(lerp(nWS0, nDetailWS, saturate(_NormalScale)));
                float3 nVS = normalize(mul((float3x3)UNITY_MATRIX_V, nFinal));
                return float4(nVS*0.5f+0.5f,1.0);
            }
            ENDHLSL
        }
    }
    FallBack Off
}
