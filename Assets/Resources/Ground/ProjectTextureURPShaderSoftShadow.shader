Shader "Custom/URPColoredShadowWithTransparency_ProjectedTexture_WithAdditionalLights"
{
    Properties
    {
        _Color ("Base Color", Color) = (1, 0, 0, 1)
        _ShadowColor ("Shadow Color", Color) = (0, 0, 0, 1)
        _ShadowOpacity ("Shadow Opacity", Range(0,1)) = 0.5
        _MainTex ("Color Texture", 2D) = "white" {}
        _SSUVScale ("UV Scale", Range(0,10)) = 1
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "Queue" = "Geometry"
            "RenderType" = "Opaque"
            "IgnoreProjector" = "True"
        }

        LOD 100
        ZWrite On
        ZTest LEqual
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            Name "UniversalForward"
            Tags { "LightMode" = "UniversalForward" }

            ZWrite On
            ZTest LEqual
            Blend SrcAlpha OneMinusSrcAlpha

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #pragma multi_compile _ _FORWARD_PLUS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile_fragment _ _SHADOWS_SOFT
            #pragma multi_compile _ SHADOWS_SHADOWMASK
            #pragma multi_compile_fragment _ _SCREEN_SPACE_OCCLUSION
            #pragma multi_compile _ LIGHTMAP_ON
            #pragma multi_compile _ LIGHTMAP_SHADOW_MIXING

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _SSUVScale;
            float4 _Color;
            float4 _ShadowColor;
            float _ShadowOpacity;

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float4 tangentOS : TANGENT;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 normalWS : TEXCOORD0;
                float3 positionWS : TEXCOORD1;
                float4 shadowCoord : TEXCOORD2;
            };

            float2 GetScreenUV(float2 clipPos, float uvScaleFactor)
            {
                float4 objectPositionCS = TransformObjectToHClip(float3(0, 0, 0));
                float2 screenUV = float2(clipPos.x / _ScreenParams.x, clipPos.y / _ScreenParams.y);
                float screenRatio = _ScreenParams.y / _ScreenParams.x;

                screenUV -= 0.5;
                screenUV.x -= objectPositionCS.x / (2 * objectPositionCS.w);
                screenUV.y += objectPositionCS.y / (2 * objectPositionCS.w);
                screenUV.y *= screenRatio;

                screenUV *= (1.0 / uvScaleFactor) * objectPositionCS.w;
                screenUV = screenUV * _MainTex_ST.xy + _MainTex_ST.zw;

                return screenUV;
            }

            float3 GetAdditionalLighting(float3 positionWS, float3 normalWS, float4 positionCS, float4 shadowCoord)
            {
                float3 lighting = 0;

                InputData inputData = (InputData)0;
                inputData.positionWS = positionWS;
                inputData.normalWS = normalWS;
                inputData.viewDirectionWS = GetWorldSpaceNormalizeViewDir(positionWS);
                inputData.normalizedScreenSpaceUV = GetNormalizedScreenSpaceUV(positionCS);
                inputData.shadowCoord = shadowCoord;

                #if USE_FORWARD_PLUS
                UNITY_LOOP for (uint lightIndex = 0; lightIndex < min(URP_FP_DIRECTIONAL_LIGHTS_COUNT, MAX_VISIBLE_LIGHTS); lightIndex++)
                {
                    Light light = GetAdditionalLight(lightIndex, positionWS, half4(1, 1, 1, 1));
                    float NdotL = saturate(dot(normalWS, light.direction));
                    lighting += light.color * NdotL * light.distanceAttenuation * light.shadowAttenuation;
                }
                #endif

                #if defined(_ADDITIONAL_LIGHTS)
                uint lightCount = GetAdditionalLightsCount();
                LIGHT_LOOP_BEGIN(lightCount)
                    Light light = GetAdditionalLight(lightIndex, positionWS, half4(1, 1, 1, 1));
                    float NdotL = saturate(dot(normalWS, light.direction));
                    lighting += light.color * NdotL * light.distanceAttenuation * light.shadowAttenuation;
                LIGHT_LOOP_END
                #endif

                return lighting;
            }

            Varyings vert(Attributes IN)
            {
                Varyings OUT;

                VertexPositionInputs vertexInputs = GetVertexPositionInputs(IN.positionOS.xyz);
                VertexNormalInputs normalInputs = GetVertexNormalInputs(IN.normalOS);

                OUT.positionCS = vertexInputs.positionCS;
                OUT.positionWS = vertexInputs.positionWS;
                OUT.normalWS = normalize(normalInputs.normalWS);

                #if defined(_MAIN_LIGHT_SHADOWS)
                    OUT.shadowCoord = GetShadowCoord(vertexInputs);
                #else
                    OUT.shadowCoord = float4(0, 0, 0, 0);
                #endif

                return OUT;
            }

            float4 frag(Varyings IN) : SV_Target
            {
                float3 normal = normalize(IN.normalWS);

                float shadow = 1.0;
                #if defined(_MAIN_LIGHT_SHADOWS)
                    shadow = MainLightRealtimeShadow(IN.shadowCoord);
                #endif

                Light mainLight = GetMainLight(IN.shadowCoord);
                float NdotL = saturate(dot(normal, mainLight.direction));
                float3 lighting = mainLight.color * NdotL;
                lighting += GetAdditionalLighting(IN.positionWS, normal, IN.positionCS, IN.shadowCoord);

                float shadowStrength = _ShadowOpacity * (1.0 - shadow);
                float3 colorWithShadow = lerp(lighting, lighting * _ShadowColor.rgb, shadowStrength);

                float4 finalColor = _Color;
                finalColor.rgb *= colorWithShadow;
                finalColor.a = _Color.a;

                float2 screenUV = GetScreenUV(IN.positionCS.xy, _SSUVScale);
                float4 projectedTex = tex2D(_MainTex, screenUV);

                finalColor.rgb *= projectedTex.rgb;
                finalColor.a *= projectedTex.a;

                return finalColor;
            }
            ENDHLSL
        }
    }
}
