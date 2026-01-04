Shader "Custom/HiddenCubeIdentity"
{
    Properties
    {
        _BaseColor ("Dot Color", Color) = (1,1,1,1)
        _Background ("Background Color", Color) = (0,0,0,1)
        _Density ("Density (cells per UV)", Range(1,200)) = 40
        _DotSize ("Dot Size (0..0.5)", Range(0.001, 0.49)) = 0.08
        _Speed ("Speed", Range(0,10)) = 1.5
        _Softness ("Edge Softness", Range(0.0001, 0.2)) = 0.02
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" "RenderPipeline"="UniversalRenderPipeline" }

        Pass
        {
            Name "Forward"
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv         : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv          : TEXCOORD0;
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseColor;
                float4 _Background;
                float  _Density;
                float  _DotSize;
                float  _Speed;
                float  _Softness;
            CBUFFER_END

            // Small hash helpers (stable pseudo-random per cell)
            float hash11(float n)
            {
                n = frac(n * 0.1031);
                n *= n + 33.33;
                n *= n + n;
                return frac(n);
            }

            float2 hash21(float2 p)
            {
                // returns 0..1
                float n = dot(p, float2(127.1, 311.7));
                return float2(hash11(n), hash11(n + 19.19));
            }

            Varyings vert (Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;
                return OUT;
            }

            half4 frag (Varyings IN) : SV_Target
            {
                // Scale UVs into a grid. Higher density = more cells = more dots.
                float2 uv = IN.uv * _Density;

                float2 cell = floor(uv);
                float2 f    = frac(uv);

                // Random base center per cell (0..1 in cell space)
                float2 rnd = hash21(cell);

                // Give each cell a random velocity direction and speed
                float2 velDir = normalize(hash21(cell + 17.0) * 2.0 - 1.0);
                float  velMag = lerp(0.2, 1.0, hash11(dot(cell, 3.13)));

                float t = _Time.y * _Speed;

                // Animated center (wrap inside cell)
                float2 center = frac(rnd + velDir * velMag * t);

                // Distance from pixel in cell to dot center
                float d = distance(f, center);

                // Dot mask: 1 inside dot, 0 outside, with softness
                float edge = max(_Softness, 1e-5);
                float dotMask = 1.0 - smoothstep(_DotSize, _DotSize + edge, d);

                float4 col = lerp(_Background, _BaseColor, dotMask);
                return col;
            }
            ENDHLSL
        }
    }
}
