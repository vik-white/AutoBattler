Shader "Hidden/Outlines"
{
    Properties
    {
        _BlitTexture ("Source", 2D) = "white" {}
        _OutlineColor ("Outline Color", Color) = (0,0,0,1)
        _OutlineScale ("Outline Scale", Float) = 1
        _DepthThreshold ("Depth Threshold", Float) = 1.5
        _RobertsCrossMultiplier ("Depth Multiplier", Float) = 100
        _NormalThreshold ("Normal Threshold", Float) = 0.4
        _SteepAngleThreshold ("Steep Angle Threshold", Float) = 0.2
        _SteepAngleMultiplier ("Steep Angle Multiplier", Float) = 25
    }

    SubShader
    {
        Tags { "RenderPipeline" = "UniversalPipeline" }
        ZWrite Off
        ZTest Always
        Cull Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            Name "Outlines"

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"

            TEXTURE2D_X(_BlitTexture);
            SAMPLER(sampler_BlitTexture);
            TEXTURE2D(_SceneViewSpaceNormals);
            SAMPLER(sampler_SceneViewSpaceNormals);

            float4 _OutlineColor;
            float _OutlineScale;
            float _DepthThreshold;
            float _RobertsCrossMultiplier;
            float _NormalThreshold;
            float _SteepAngleThreshold;
            float _SteepAngleMultiplier;

            struct Attributes
            {
                uint vertexID : SV_VertexID;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            Varyings Vert(Attributes input)
            {
                Varyings output;
                output.positionCS = GetFullScreenTriangleVertexPosition(input.vertexID);
                output.uv = GetFullScreenTriangleTexCoord(input.vertexID);
                return output;
            }

            float SampleDepth(float2 uv)
            {
                return SampleSceneDepth(uv);
            }

            float3 SampleNormals(float2 uv)
            {
                return SAMPLE_TEXTURE2D(_SceneViewSpaceNormals, sampler_SceneViewSpaceNormals, uv).xyz * 2.0 - 1.0;
            }

            float FragEdge(float2 uv, float2 texelSize)
            {
                float2 offset = texelSize * max(_OutlineScale, 1.0);

                float depthCenter = SampleDepth(uv);
                float depthRight = SampleDepth(uv + float2(offset.x, 0));
                float depthUp = SampleDepth(uv + float2(0, offset.y));

                float depthEdge = abs(depthCenter - depthRight) + abs(depthCenter - depthUp);
                depthEdge *= _RobertsCrossMultiplier;

                float3 normalCenter = SampleNormals(uv);
                float3 normalRight = SampleNormals(uv + float2(offset.x, 0));
                float3 normalUp = SampleNormals(uv + float2(0, offset.y));

                float normalEdge = length(normalCenter - normalRight) + length(normalCenter - normalUp);
                float angleBoost = saturate((1.0 - abs(normalCenter.z)) - _SteepAngleThreshold) * _SteepAngleMultiplier;
                normalEdge *= 1.0 + angleBoost;

                float depthMask = step(_DepthThreshold, depthEdge);
                float normalMask = step(_NormalThreshold, normalEdge);

                return saturate(max(depthMask, normalMask));
            }

            float4 Frag(Varyings input) : SV_Target
            {
                float4 source = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_BlitTexture, input.uv);
                float2 texelSize = 1.0 / _ScreenParams.xy;
                float edge = FragEdge(input.uv, texelSize);

                float3 color = lerp(source.rgb, _OutlineColor.rgb, edge * _OutlineColor.a);
                return float4(color, source.a);
            }
            ENDHLSL
        }
    }
}
