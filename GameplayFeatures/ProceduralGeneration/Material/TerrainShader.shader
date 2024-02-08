Shader "UnityToolbox/TerrainShader"
{
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        CGPROGRAM
        #pragma target 3.0
        #pragma surface surf Standard fullforwardshadows

        #include "UnityCG.cginc"

        struct Input
        {
            float3 worldPos;
            float3 worldNormal;
        };

        const static int _MaxLayerCount = 8;
        const static float _Epsilon = 1E-4;

        int _LayerCount;
        UNITY_DECLARE_TEX2DARRAY(_BaseTextures);
        float _MinHeight;
        float _MaxHeight;
        float3 _BaseColors[_MaxLayerCount];
        float _BaseColorStrengths[_MaxLayerCount];
        float _BaseStartHeights[_MaxLayerCount];
        float _BaseTextureScales[_MaxLayerCount];
        float _BaseBlends[_MaxLayerCount];

        inline float inverseLerp(float a, float b, float value)
        {
            return saturate((value - a) / (b - a));
        }

        inline float3 triplanarTexture(float3 worldPos, float scale, float3 blendAxes, int textureIndex)
        {
            float3 scaledWorldPos = worldPos/scale;
            float3 xProjection = UNITY_SAMPLE_TEX2DARRAY(_BaseTextures, float3(scaledWorldPos.y, scaledWorldPos.z, textureIndex)) * blendAxes.x;
            float3 yProjection = UNITY_SAMPLE_TEX2DARRAY(_BaseTextures, float3(scaledWorldPos.z, scaledWorldPos.x, textureIndex)) * blendAxes.y;
            float3 zProjection = UNITY_SAMPLE_TEX2DARRAY(_BaseTextures, float3(scaledWorldPos.x, scaledWorldPos.y, textureIndex)) * blendAxes.z;
            return xProjection + yProjection + zProjection;
        }

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            float heightPercent = inverseLerp(_MinHeight,_MaxHeight, IN.worldPos.y);
            float3 blendAxes = abs(IN.worldNormal);
            blendAxes /= blendAxes.x + blendAxes.y + blendAxes.z;

            for (int i = 0; i < _LayerCount; i ++) {
                float drawStrength = inverseLerp(-_BaseBlends[i]/2 - _Epsilon, _BaseBlends[i]/2, heightPercent - _BaseStartHeights[i]);

                float3 baseColor = _BaseColors[i] * _BaseColorStrengths[i];
                float3 baseTexture = triplanarTexture(IN.worldPos, _BaseTextureScales[i], blendAxes, i) * (1-_BaseColorStrengths[i]);

                //o.Albedo = o.Albedo * (1-drawStrength) + (baseTexture*0.5f + baseColor) * drawStrength;
                o.Albedo = o.Albedo * (1-drawStrength) + (baseTexture*0.5f + baseColor) * drawStrength;
            }
            }

        ENDCG
    }
    FallBack "Diffuse"
}