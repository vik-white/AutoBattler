Shader "Hidden/ViewSpaceNormals"
{
    Properties
    {
        [HideInInspector]_DeformedMeshIndex("Deformed Mesh Buffer Index Offset", Float) = 0
        [HideInInspector]_DeformationParamsForMotionVectors("Deformation Parameters", Vector) = (0,0,0,0)
    }

    SubShader
    {
        Tags { "RenderPipeline" = "UniversalPipeline" }

        Pass
        {
            Name "ViewSpaceNormals"
            Tags { "LightMode" = "UniversalForward" }

            ZWrite On
            ZTest LEqual
            Cull Back

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #pragma target 4.5
            #pragma multi_compile_instancing
            #pragma multi_compile _ DOTS_INSTANCING_ON
            #pragma multi_compile _ DOTS_DEFORMED

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            CBUFFER_START(UnityPerMaterial)
                float _DeformedMeshIndex;
                float4 _DeformationParamsForMotionVectors;
            CBUFFER_END

            #ifdef UNITY_DOTS_INSTANCING_ENABLED
            UNITY_DOTS_INSTANCING_START(UserPropertyMetadata)
                UNITY_DOTS_INSTANCED_PROP(float, _DeformedMeshIndex)
                UNITY_DOTS_INSTANCED_PROP(float4, _DeformationParamsForMotionVectors)
            UNITY_DOTS_INSTANCING_END(UserPropertyMetadata)
            #endif

            #include "Packages/com.rukhanka.animation/Rukhanka.Runtime/Deformation/Resources/ComputeDeformedVertex.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float4 tangentOS : TANGENT;
                uint vertexID : SV_VertexID;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 normalVS : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            Varyings Vert(Attributes input)
            {
                Varyings output;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);

                float3 positionOS = input.positionOS.xyz;
                float3 normalOS = input.normalOS;
                float3 tangentOS = input.tangentOS.xyz;

                ComputeDeformedVertex_float(
                    input.vertexID,
                    input.positionOS.xyz,
                    input.normalOS.xyz,
                    input.tangentOS.xyz,
                    positionOS,
                    normalOS,
                    tangentOS);

                VertexPositionInputs positionInputs = GetVertexPositionInputs(positionOS);
                VertexNormalInputs normalInputs = GetVertexNormalInputs(normalOS, float4(tangentOS, input.tangentOS.w));

                output.positionCS = positionInputs.positionCS;
                output.normalVS = TransformWorldToViewDir(normalInputs.normalWS, true);
                return output;
            }

            float4 Frag(Varyings input) : SV_Target
            {
                float3 normal = normalize(input.normalVS);
                return float4(normal * 0.5 + 0.5, 1.0);
            }
            ENDHLSL
        }
    }
}
