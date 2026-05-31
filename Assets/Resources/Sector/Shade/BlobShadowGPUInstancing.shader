Shader "BlobShadowGPUInstancing"
{
    Properties 
    {
		_BlobColor ("Color", Color) = (0, 0, 0, 0)
		_BlobIntensity ("Intensity", Float) = 0
		_Shape ("Shape", Vector) = (1, 2, 0, 0)
		_Vector2 ("Vector2", Vector) = (1, 0.1, 0, 0)
	}
	SubShader 
	{
		Tags 
		{
			"RenderPipeline" = "UniversalPipeline"
			"RenderType" = "Transparent"
			"Queue" = "Transparent"
		}

		Pass 
		{
			Name "Unlit"
			
			ZWrite Off
			ZTest LEqual
			Cull Back
			Blend SrcAlpha OneMinusSrcAlpha
			
			HLSLPROGRAM

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

			UNITY_INSTANCING_BUFFER_START(Props)
				UNITY_DEFINE_INSTANCED_PROP(half4, _BlobColor)
				UNITY_DEFINE_INSTANCED_PROP(half, _BlobIntensity)
				UNITY_DEFINE_INSTANCED_PROP(half2, _Shape)
				UNITY_DEFINE_INSTANCED_PROP(half2, _Vector2)
			UNITY_INSTANCING_BUFFER_END(Props)

			TEXTURE2D_FLOAT(_CameraDepthTexture);
			SAMPLER(sampler_CameraDepthTexture);
			
			#pragma vertex Vertex
			#pragma fragment Fragment
			
			#pragma multi_compile_fog
			
			#pragma multi_compile_instancing
			#pragma instancing_options nolodfade nolightprobe nolightmap
			

			float GetVertexDepth(float4 positionNDC, half2 _Vector2)
			{
				return saturate((positionNDC.a - _Vector2.x) * _Vector2.y);
			}

			half InverseLerp(half A, half B, float T)
			{
			    return (T - A) / (B - A);
			}
			
			struct Attributes
			{
				float4 positionOS : POSITION;
				float2 uv : TEXCOORD0;
				half4 color : COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct Varyings
			{
				float4 positionCS : SV_POSITION;
				float4 screenPos : TEXCOORD0;
				half VertexDepth : TEXCOORD1;
				half FogFactor : TEXCOORD2;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			
			Varyings Vertex(Attributes IN)
			{
				Varyings OUT;

				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_TRANSFER_INSTANCE_ID(IN, OUT);

				VertexPositionInputs positionInputs = GetVertexPositionInputs(IN.positionOS.xyz);
				OUT.positionCS = positionInputs.positionCS;
				OUT.screenPos = positionInputs.positionNDC;
				OUT.FogFactor = ComputeFogFactor(positionInputs.positionCS.z);
				
				OUT.VertexDepth = GetVertexDepth(positionInputs.positionNDC, UNITY_ACCESS_INSTANCED_PROP(Props, _Vector2));
				
				return OUT;
			}
			
			half4 Fragment(Varyings IN) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(IN);
				
				float2 screenSpaceUV = IN.screenPos.xy / IN.screenPos.w;
				float depth = SAMPLE_TEXTURE2D(_CameraDepthTexture, sampler_CameraDepthTexture, screenSpaceUV).r;
				
				float3 worldSpacePosition = ComputeWorldSpacePosition(screenSpaceUV, depth, UNITY_MATRIX_I_VP);
				float3 objectSpacePosition = TransformWorldToObject(worldSpacePosition);
				
				half3 color = UNITY_ACCESS_INSTANCED_PROP(Props, _BlobColor).xyz;
				color = MixFog(color, IN.FogFactor);

				half2 shape = UNITY_ACCESS_INSTANCED_PROP(Props, _Shape);
				half blobIntensity = UNITY_ACCESS_INSTANCED_PROP(Props, _BlobIntensity);
				
				half alpha = saturate(InverseLerp(shape.x, shape.y, dot(objectSpacePosition, objectSpacePosition)));
				
				alpha *= blobIntensity;
				alpha *= IN.VertexDepth;
				alpha = saturate(alpha);
				
				return half4(color, alpha);
			}
			ENDHLSL
		}
	}
}
