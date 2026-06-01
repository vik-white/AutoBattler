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
            #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Fog.hlsl"

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

            float DepthToNearToFar(float rawDepth)
            {
                float viewZ = LinearEyeDepth(rawDepth, _ZBufferParams);
                return max(viewZ - _ProjectionParams.y, 0.0);
            }

            float SampleNearToFarDepth(float2 uv)
            {
                return DepthToNearToFar(SampleDepth(uv));
            }

            float SampleOutlineFogFactor(float2 uv, float2 texelSize)
            {
                float2 offset = texelSize * max(_OutlineScale, 1.0);
                float nearToFarZ = SampleNearToFarDepth(uv);
                nearToFarZ = min(nearToFarZ, SampleNearToFarDepth(uv + float2(offset.x, 0)));
                nearToFarZ = min(nearToFarZ, SampleNearToFarDepth(uv + float2(0, offset.y)));
                return ComputeFogFactorZ0ToFar(nearToFarZ);
            }

            float3 SampleRawNormals(float2 uv)
            {
                return SAMPLE_TEXTURE2D(_SceneViewSpaceNormals, sampler_SceneViewSpaceNormals, uv).xyz;
            }

            float3 NormalizeViewNormal(float3 rawNormal)
            {
                float3 n = rawNormal * rsqrt(max(dot(rawNormal, rawNormal), 1e-6));
                return TransformWorldToViewDir(n, false);
            }

            float CharacterMaskFromRaw(float3 rawNormal)
            {
                // Background pixels of the filtered normals texture are cleared to (0,0,0,0),
                // so any pixel with a non-trivial normal length belongs to a filtered (character) object.
                return step(0.01, dot(rawNormal, rawNormal));
            }

            float FragEdge(float2 uv, float2 texelSize)
            {
                float2 offset = texelSize * max(_OutlineScale, 1.0);

                float depthCenter = SampleDepth(uv);
                float depthRight = SampleDepth(uv + float2(offset.x, 0));
                float depthUp = SampleDepth(uv + float2(0, offset.y));

                float depthEdge = abs(depthCenter - depthRight) + abs(depthCenter - depthUp);
                depthEdge *= _RobertsCrossMultiplier;

                float3 rawNormalCenter = SampleRawNormals(uv);
                float3 rawNormalRight = SampleRawNormals(uv + float2(offset.x, 0));
                float3 rawNormalUp = SampleRawNormals(uv + float2(0, offset.y));

                float characterMask = max(
                    CharacterMaskFromRaw(rawNormalCenter),
                    max(CharacterMaskFromRaw(rawNormalRight), CharacterMaskFromRaw(rawNormalUp)));

                float3 normalCenter = NormalizeViewNormal(rawNormalCenter);
                float3 normalRight = NormalizeViewNormal(rawNormalRight);
                float3 normalUp = NormalizeViewNormal(rawNormalUp);

                float normalEdge = length(normalCenter - normalRight) + length(normalCenter - normalUp);
                float angleBoost = saturate((1.0 - abs(normalCenter.z)) - _SteepAngleThreshold) * _SteepAngleMultiplier;
                normalEdge *= 1.0 + angleBoost;

                float depthMask = step(_DepthThreshold, depthEdge);
                float normalMask = step(_NormalThreshold, normalEdge);

                return saturate(max(depthMask, normalMask)) * characterMask;
            }

            float4 Frag(Varyings input) : SV_Target
            {
                float4 source = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_BlitTexture, input.uv);
                float2 texelSize = 1.0 / _ScreenParams.xy;
                float edge = FragEdge(input.uv, texelSize);

                float fogFactor = SampleOutlineFogFactor(input.uv, texelSize);
                float3 outlineColor = MixFog(_OutlineColor.rgb, fogFactor);
                float3 color = lerp(source.rgb, outlineColor, edge * _OutlineColor.a);
                return float4(color, source.a);
            }
            ENDHLSL
        }
    }
}
