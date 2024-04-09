Shader "Abiogenesis3d/PixelArtEdgeHighlights"
{
    SubShader
    {
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            Name "PixelArtEdgeHighlights_Pass"

            HLSLPROGRAM
            #pragma shader_feature UNITY_PIPELINE_URP
            #pragma shader_feature UNITY_WEBGL

            #pragma vertex Vert
            #pragma fragment Frag

        #if UNITY_PIPELINE_URP
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareNormalsTexture.hlsl"
            TEXTURE2D(_CameraOpaqueTexture);
            SAMPLER(sampler_CameraOpaqueTexture);
            TEXTURE2D(_CameraDepthTexture);
            SAMPLER(sampler_CameraDepthTexture);
        #else
            #include "UnityCG.cginc"
            UNITY_DECLARE_TEX2D(_MainTex);
            UNITY_DECLARE_TEX2D(_CameraDepthTexture);
            UNITY_DECLARE_TEX2D(_CameraDepthNormalsTexture);
        #endif

            float _ConvexHighlight;
            float _OutlineShadow;
            float _ConcaveShadow;
            float _DepthSensitivity;
            int _DebugEffect;
            float4 _Test1;

            struct Attributes
            {
                UNITY_VERTEX_INPUT_INSTANCE_ID
                float4 positionHCS : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 texcoord : TEXCOORD0;
            };

            Varyings VertURP(Attributes input)
            {
                UNITY_SETUP_INSTANCE_ID(input);

                Varyings output;
                output.positionCS = float4(input.positionHCS.xyz, 1.0);

            #if UNITY_UV_STARTS_AT_TOP
                output.positionCS.y *= -1;
            #endif

                output.texcoord = input.texcoord;
                return output;
            }

            float2 TransformTriangleVertexToUV(float2 vertex)
            {
                return (vertex + 1.0) * 0.5;
            }

        #if UNITY_PIPELINE_URP
        #else
            Varyings VertBuiltin(Attributes input)
            {
                UNITY_SETUP_INSTANCE_ID(input);

                Varyings output;

                // NOTE: this one is for the deprecated PostProcess
                // output.positionCS = float4(input.positionHCS.xy, 0.0, 1.0);
                // output.texcoord = TransformTriangleVertexToUV(input.positionHCS.xy);
                // #if UNITY_UV_STARTS_AT_TOP
                //     output.texcoord = output.texcoord * float2(1.0, -1.0) + float2(0.0, 1.0);
                // #endif

                output.positionCS = UnityObjectToClipPos(input.positionHCS);
                output.texcoord = input.texcoord;

                return output;
            }
        #endif

            Varyings Vert(Attributes input)
            {
            #if UNITY_PIPELINE_URP
                return VertURP(input);
            #else
                return VertBuiltin(input);
            #endif
            }

            float2 GetPixelSize()
            {
                return _ScreenParams.zw - 1;
                // return 1 / _ScreenParams.xy;
            }

            float smooth_max(float max, float val)
            {
                return saturate(val / max);
            }

            float clamp01(float v)
            {
                return clamp(v, 0.0, 1.0);
            }

            float3 Lighten(float3 color, float amount)
            {
                return color * (1.0 + amount);
            }

            float GetNormalDiff(float3 center, float3 side)
            {
                float normalDot = dot(center - side, float3(1.0, 1.0, 1.0));
                normalDot = smooth_max(0.00000001, normalDot);
                normalDot = (1.0 - dot(center, side)) * normalDot;
                return normalDot;
            }

            struct DepthNormal
            {
                float depth;
                float3 normal;
            };

            struct Neighbours
            {
                DepthNormal center;
                DepthNormal left;
                DepthNormal right;
                DepthNormal top;
                DepthNormal bottom;

                DepthNormal leftTop;
                DepthNormal rightTop;
                DepthNormal leftBottom;
                DepthNormal rightBottom;
            };

            float Sobel(Neighbours p, float2 uv)
            {
                float3 hor = float3(0, 0, 0);
                hor += p.leftBottom.depth  *  1.0;
                hor += p.rightBottom.depth * -1.0;
                hor += p.left.depth        *  1.0;
                hor += p.right.depth       * -1.0;
                hor += p.leftTop.depth     *  1.0;
                hor += p.rightTop.depth    * -1.0;

                float3 ver = float3(0, 0, 0);
                ver += p.leftBottom.depth  *  1.0;
                ver += p.bottom.depth      *  1.0;
                ver += p.rightBottom.depth *  1.0;
                ver += p.leftTop.depth     * -1.0;
                ver += p.top.depth         * -1.0;
                ver += p.rightTop.depth    * -1.0;

                return sqrt(hor * hor + ver * ver).r;
            }

            DepthNormal GetDepthNormal(float2 uv)
            {
                DepthNormal depthNormal;
            #if UNITY_PIPELINE_URP
                depthNormal.depth = _CameraDepthTexture.SampleLevel(sampler_CameraDepthTexture, uv, 0).r;
                depthNormal.normal = _CameraNormalsTexture.SampleLevel(sampler_CameraNormalsTexture, uv, 0).rgb;
                depthNormal.normal = TransformWorldToViewDir(depthNormal.normal, true);
            #else
                float depth = _CameraDepthTexture.SampleLevel(sampler_CameraDepthTexture, uv, 0).r;
                float4 depthNormalRaw = _CameraDepthNormalsTexture.SampleLevel(sampler_CameraDepthNormalsTexture, uv, 0);
                float decodedDepth;
                float3 decodedNormal;
                DecodeDepthNormal(depthNormalRaw, decodedDepth, decodedNormal);

                // depthNormal.depth = 1 - decodedDepth; // NOTE: decoded depth has noise when moving for some reason
                // depthNormal.depth = depth; // orthographic
                bool isOrthographic = (unity_OrthoParams.w == 1.0);
                depthNormal.depth = isOrthographic ? depth : 1 - decodedDepth;

                depthNormal.normal = decodedNormal;
            #endif

            #ifdef UNITY_WEBGL
                depthNormal.depth = 1 - depthNormal.depth;
            #endif
                return depthNormal;
            }

            Neighbours GetDepthNormalsNeighbours(float2 uv, float2 pixelSize)
            {
                Neighbours neighbours;

                neighbours.center = GetDepthNormal(uv);
                neighbours.left   = GetDepthNormal(uv + float2(-pixelSize.x, 0.0));
                neighbours.right  = GetDepthNormal(uv + float2( pixelSize.x, 0.0));
                neighbours.top    = GetDepthNormal(uv + float2(0.0,  pixelSize.y));
                neighbours.bottom = GetDepthNormal(uv + float2(0.0, -pixelSize.y));

                neighbours.leftTop     = GetDepthNormal(uv + float2(-pixelSize.x,  pixelSize.y));
                neighbours.rightTop    = GetDepthNormal(uv + float2( pixelSize.x,  pixelSize.y));
                neighbours.leftBottom  = GetDepthNormal(uv + float2(-pixelSize.x, -pixelSize.y));
                neighbours.rightBottom = GetDepthNormal(uv + float2( pixelSize.x, -pixelSize.y));

                return neighbours;
            }

            float4 GetMainColor(float2 texcoord)
            {
            #if (UNITY_PIPELINE_URP)
                return _CameraOpaqueTexture.SampleLevel(sampler_CameraOpaqueTexture, texcoord, 0);
            #else
                return _MainTex.SampleLevel(sampler_MainTex, texcoord, 0);
            #endif
            }

            float4 Frag (Varyings i) : SV_Target
            {
                float2 pixelSize = GetPixelSize();
                float4 originalColor = GetMainColor(i.texcoord);

                Neighbours p = GetDepthNormalsNeighbours(i.texcoord, pixelSize);

                float sobel = Sobel(p, i.texcoord);
                sobel = smoothstep(0, 0.5, sobel);
                sobel *= 10000;

                float depthDiff = 0.0;
                depthDiff += p.center.depth - p.left.depth;
                depthDiff += p.center.depth - p.right.depth;
                depthDiff += p.center.depth - p.top.depth;
                depthDiff += p.center.depth - p.bottom.depth;
                depthDiff = smoothstep(0, _DepthSensitivity, depthDiff);

                float normalDiff = 0;
                normalDiff += GetNormalDiff(p.center.normal, p.left.normal);
                normalDiff += GetNormalDiff(p.center.normal, p.right.normal);
                normalDiff += GetNormalDiff(p.center.normal, p.top.normal);
                normalDiff += GetNormalDiff(p.center.normal, p.bottom.normal);
                normalDiff = smoothstep(0, 1, normalDiff);

                float concaveNormalDiff = 0;
                concaveNormalDiff += GetNormalDiff(p.left.normal, p.center.normal);
                concaveNormalDiff += GetNormalDiff(p.right.normal, p.center.normal);
                concaveNormalDiff += GetNormalDiff(p.top.normal, p.center.normal);
                concaveNormalDiff += GetNormalDiff(p.bottom.normal, p.center.normal);

                float outline = depthDiff;
                float convex = 0;
                float concave = 0;

                if (depthDiff > 0)
                convex = smooth_max(0.3, normalDiff - outline) * 2;
                else concave = smooth_max(1, concaveNormalDiff - sobel);

                float3 col = originalColor.rgb;
                col = Lighten(col, convex * _ConvexHighlight);
                col = Lighten(col, - outline * _OutlineShadow);
                col = Lighten(col, - concave * _ConcaveShadow * _OutlineShadow);

                int k = 1;
                if (_DebugEffect == 0) {} // 0 is no debug
                else if (_DebugEffect == k++) col = float3(concave, convex, outline);
                else if (_DebugEffect == k++) col = float3(1, 1, 1) * p.center.depth;
                else if (_DebugEffect == k++) col = float3(p.center.normal);
                else if (_DebugEffect == k++) col = float3(1, 1, 1) * sobel;
                else if (_DebugEffect == k++) col = float3(1, 1, 1) * depthDiff;
                else if (_DebugEffect == k++) col = float3(1, 1, 1) * normalDiff;
                else if (_DebugEffect == k++) col = float3(1, 1, 1) * concaveNormalDiff;

                return float4(col, originalColor.a);
            }

            ENDHLSL
        }
    }
}
