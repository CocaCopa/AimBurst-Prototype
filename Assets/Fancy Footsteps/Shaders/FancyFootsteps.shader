Shader "VaxKun/FancyFootsteps_URP"
{
    Properties
    {
        _Cutoff("Mask Clip Value", Float) = 0.5
        _NoiseScale("NoiseScale", Float) = 1.0
    }

    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="TransparentCutout" "Queue"="AlphaTest" }
        LOD 100
        Cull Off

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            // URP keywords (basic)
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _SHADOWS_SOFT

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            float _Cutoff;
            float _NoiseScale;

            struct Attributes
            {
                float4 positionOS : POSITION;
                float4 color      : COLOR;
                float3 normalOS   : NORMAL;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float4 color       : COLOR;
                float3 positionWS  : TEXCOORD0;
                float3 normalWS    : TEXCOORD1;
            };

            // --- Simplex noise from your shader, adapted to HLSL ---
            float3 mod2D289(float3 x) { return x - floor(x * (1.0 / 289.0)) * 289.0; }
            float2 mod2D289(float2 x) { return x - floor(x * (1.0 / 289.0)) * 289.0; }
            float3 permute(float3 x) { return mod2D289(((x * 34.0) + 1.0) * x); }

            float snoise(float2 v)
            {
                const float4 C = float4(0.211324865405187, 0.366025403784439, -0.577350269189626, 0.024390243902439);
                float2 i = floor(v + dot(v, C.yy));
                float2 x0 = v - i + dot(i, C.xx);
                float2 i1 = (x0.x > x0.y) ? float2(1.0, 0.0) : float2(0.0, 1.0);
                float4 x12 = x0.xyxy + C.xxzz;
                x12.xy -= i1;
                i = mod2D289(i);
                float3 p = permute(permute(i.y + float3(0.0, i1.y, 1.0)) + i.x + float3(0.0, i1.x, 1.0));
                float3 m = max(0.5 - float3(dot(x0, x0), dot(x12.xy, x12.xy), dot(x12.zw, x12.zw)), 0.0);
                m = m * m;
                m = m * m;
                float3 x = 2.0 * frac(p * C.www) - 1.0;
                float3 h = abs(x) - 0.5;
                float3 ox = floor(x + 0.5);
                float3 a0 = x - ox;
                m *= 1.79284291400159 - 0.85373472095314 * (a0 * a0 + h * h);
                float3 g;
                g.x = a0.x * x0.x + h.x * x0.y;
                g.yz = a0.yz * x12.xz + h.yz * x12.yw;
                return 130.0 * dot(m, g);
            }

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                VertexPositionInputs posInputs = GetVertexPositionInputs(IN.positionOS.xyz);
                OUT.positionHCS = posInputs.positionCS;
                OUT.positionWS = posInputs.positionWS;
                OUT.color = IN.color;

                VertexNormalInputs nrmInputs = GetVertexNormalInputs(IN.normalOS);
                OUT.normalWS = nrmInputs.normalWS;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                // Base albedo from vertex color
                half3 albedo = IN.color.rgb;

                // Noise uses OBJECT XY in your original (world->object then xy).
                // Here we can use WORLD XY (more stable across scaling), or mimic object space:
                // float2 p = TransformWorldToObject(IN.positionWS).xy * _NoiseScale;
                float2 p = IN.positionWS.xy * _NoiseScale;

                float n = snoise(p);
                n = n * 0.5 + 0.5;

                // mask = step(noise, vertexAlpha)
                float mask = step(n, IN.color.a);

                clip(mask - _Cutoff);

                // Basic URP lighting (Lambert-ish via URP functions)
                half3 normalWS = normalize(IN.normalWS);
                Light mainLight = GetMainLight();
                half NdotL = saturate(dot(normalWS, mainLight.direction));
                half3 lit = albedo * (mainLight.color.rgb * NdotL + 0.2); // small ambient term

                return half4(lit, 1);
            }
            ENDHLSL
        }
    }
}
