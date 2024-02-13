Shader "UnityToolbox/TerrainShader"
{
    Properties
    {
        _LayerCount ("Layer Count", Int) = 0
        _BaseTextures ("Base Textures", 2DArray) = "" {}
        _BaseNormalMaps ("Base Normal Maps", 2DArray) = "" {}
        _BaseEmissionMaps ("Base Emission Maps", 2DArray) = "" {}
        _BaseMetallicMaps ("Base Metallic Maps", 2DArray) = "" {}
        _BaseOcclusionMaps ("Base Occlusion Maps", 2DArray) = "" {}
        _BaseRoughnessMaps ("Base Roughness Maps", 2DArray) = "" {}
        
        _MinHeight ("Min Height", Range(0, 1)) = 0
        _MaxHeight ("Max Height", Range(0, 1)) = 1

        _BaseColors ("Base Colors", 2D) = "white" {}
        _BaseStartHeights ("Base Start Heights", 2D) = "white" {}
        _BaseTextureScales ("Base Texture Scales", 2D) = "white" {}
        _BaseBlends ("Base Blends", 2D) = "white" {}
        _Smoothness ("Smoothness", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        CGPROGRAM
        #pragma target 3.0
        #pragma surface surf Standard vertex:vert fullforwardshadows addshadow

        #pragma shader_feature _NORMALMAP
        #pragma shader_feature _OCCLUSIONMAP
        #pragma shader_feature _METALLICMAP
        #pragma shader_feature _EMISSIONMAP
        #pragma shader_feature _ROUGHNESSMAP

        #include "UnityCG.cginc"

                struct Input
        {
            float3 localCoord;
            float3 localNormal;
        };

        void vert(inout appdata_full v, out Input data)
        {
            UNITY_INITIALIZE_OUTPUT(Input, data);
            data.localCoord = v.vertex.xyz;
            data.localNormal = v.normal.xyz;
        }

        const static int _MaxLayerCount = 8;
        const static float _Epsilon = 1E-4;

        int _LayerCount;
        UNITY_DECLARE_TEX2DARRAY(_BaseTextures);
        UNITY_DECLARE_TEX2DARRAY(_BaseNormalMaps);
        UNITY_DECLARE_TEX2DARRAY(_BaseEmissionMaps);
        UNITY_DECLARE_TEX2DARRAY(_BaseMetallicMaps);
        UNITY_DECLARE_TEX2DARRAY(_BaseOcclusionMaps);
        UNITY_DECLARE_TEX2DARRAY(_BaseRoughnessMaps);

        half _MinHeight;
        half _MaxHeight;
        sampler2D _BaseColors;
        sampler2D _BaseStartHeights;
        sampler2D _BaseTextureScales;
        sampler2D _BaseBlends;
        sampler2D _Smoothness;

        inline half inverseLerp(half a, half b, half value)
        {
            return saturate((value - a) / (b - a));
        }

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            half heightPercent = inverseLerp(_MinHeight,_MaxHeight, IN.localCoord.y);
            float3 blendAxes = normalize(abs(IN.localNormal));
            blendAxes /= dot(blendAxes, (float3)1);

            for (int i = 0; i < _LayerCount; i ++) {

                //Perform texture lookup for array values
                float xPos = 0.1f + (float)1/_LayerCount * i;

                half4 baseColor = tex2D(_BaseColors, float4(xPos ,0,0,0));
                half baseStartHeight = tex2D(_BaseStartHeights, float4(xPos,0,0,0)).x;
                half baseTextureScale = tex2D(_BaseTextureScales, float4(xPos,0,0,0)).x*255;
                half baseBlend = tex2D(_BaseBlends, float4(xPos,0,0,0)).x;
                half baseSmoothness = tex2D(_Smoothness, float4(xPos,0,0,0)).x;

                half drawStrength = inverseLerp(-baseBlend/2 - _Epsilon, baseBlend/2, heightPercent - baseStartHeight);

                half3 baseColorScaled = baseColor.rgb * baseColor.a;

                //Triplanar scalings
                float2 scaledWorldPosX = IN.localCoord.yz / baseTextureScale;
                float2 scaledWorldPosY = IN.localCoord.zx / baseTextureScale;
                float2 scaledWorldPosZ = IN.localCoord.xy / baseTextureScale;

                //Texturing
                half4 xTexture = UNITY_SAMPLE_TEX2DARRAY(_BaseTextures, float3(scaledWorldPosX, i)) * blendAxes.x;
                half4 yTexture = UNITY_SAMPLE_TEX2DARRAY(_BaseTextures, float3(scaledWorldPosY, i)) * blendAxes.y;
                half4 zTexture = UNITY_SAMPLE_TEX2DARRAY(_BaseTextures, float3(scaledWorldPosZ, i)) * blendAxes.z;
                half4 projectedTexture = xTexture + yTexture + zTexture;
                o.Albedo = o.Albedo * (1-drawStrength) + projectedTexture.rgb * drawStrength * baseColorScaled;
                
        #ifdef _EMISSIONMAP
                half4 xEmission = UNITY_SAMPLE_TEX2DARRAY(_BaseEmissionMaps, float3(scaledWorldPosX, i)) * blendAxes.x;
                half4 yEmission = UNITY_SAMPLE_TEX2DARRAY(_BaseEmissionMaps, float3(scaledWorldPosY, i)) * blendAxes.y;
                half4 zEmission = UNITY_SAMPLE_TEX2DARRAY(_BaseEmissionMaps, float3(scaledWorldPosZ, i)) * blendAxes.z;
                half4 projectedEmission = xEmission + yEmission + zEmission;
                o.Emission = o.Emission * (1-drawStrength) + projectedEmission * drawStrength;
        #endif

        #ifdef _NORMALMAP
                half4 xNormal = UNITY_SAMPLE_TEX2DARRAY(_BaseNormalMaps, float3(scaledWorldPosX, i)) * blendAxes.x;
                half4 yNormal = UNITY_SAMPLE_TEX2DARRAY(_BaseNormalMaps, float3(scaledWorldPosY, i)) * blendAxes.y;
                half4 zNormal = UNITY_SAMPLE_TEX2DARRAY(_BaseNormalMaps, float3(scaledWorldPosZ, i)) * blendAxes.z;
                half3 projectedNormal = UnpackScaleNormal(xNormal + yNormal + zNormal, baseTextureScale);
                o.Normal = o.Normal * (1-drawStrength) + projectedNormal * drawStrength;
        #endif
        
        #ifdef _OCCLUSIONMAP
                half4 xOcclusion = UNITY_SAMPLE_TEX2DARRAY(_BaseOcclusionMaps, float3(scaledWorldPosX, i)) * blendAxes.x;
                half4 yOcclusion = UNITY_SAMPLE_TEX2DARRAY(_BaseOcclusionMaps, float3(scaledWorldPosY, i)) * blendAxes.y;
                half4 zOcclusion = UNITY_SAMPLE_TEX2DARRAY(_BaseOcclusionMaps, float3(scaledWorldPosZ, i)) * blendAxes.z;
                half4 projectedOcclusion = lerp((half4)1, xOcclusion + yOcclusion + zOcclusion, 1);
                o.Occlusion = o.Occlusion * (1-drawStrength) + projectedOcclusion * drawStrength;
        #endif

        #ifdef _METALLICMAP
                half4 xMetallic = UNITY_SAMPLE_TEX2DARRAY(_BaseMetallicMaps, float3(scaledWorldPosX, i)) * blendAxes.x;
                half4 yMetallic = UNITY_SAMPLE_TEX2DARRAY(_BaseMetallicMaps, float3(scaledWorldPosY, i)) * blendAxes.y;
                half4 zMetallic = UNITY_SAMPLE_TEX2DARRAY(_BaseMetallicMaps, float3(scaledWorldPosZ, i)) * blendAxes.z;
                half4 projectedMetallic = xMetallic + yMetallic + zMetallic;
                half1 roughness = projectedMetallic.a;
                o.Metallic = o.Metallic * (1-drawStrength) + projectedMetallic.r * drawStrength;

        #ifdef _ROUGHNESSMAP
                half4 xRoughness = UNITY_SAMPLE_TEX2DARRAY(_BaseRoughnessMaps, float3(scaledWorldPosX, i)) * blendAxes.x;
                half4 yRoughness = UNITY_SAMPLE_TEX2DARRAY(_BaseRoughnessMaps, float3(scaledWorldPosY, i)) * blendAxes.y;
                half4 zRoughness = UNITY_SAMPLE_TEX2DARRAY(_BaseRoughnessMaps, float3(scaledWorldPosZ, i)) * blendAxes.z;
                roughness = 1-(xRoughness + yRoughness + zRoughness).r;
        #endif
                o.Smoothness = o.Smoothness * (1-drawStrength) + roughness * drawStrength * baseSmoothness;
        #endif

            }
        }

        ENDCG
    }
    FallBack "Diffuse"
}