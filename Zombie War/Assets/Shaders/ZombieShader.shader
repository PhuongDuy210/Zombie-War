Shader "CustomRenderTexture/ZombieShader"
{
    Properties
    {
        _BaseMap ("Base Map", 2D) = "white" {}
        _BaseColor ("Base Color", Color) = (1,1,1,1)

        // Flash
        _FlashColor ("Flash Color", Color) = (1,0,0,1)
        _FlashIntensity ("Flash Intensity", Range(0,1)) = 0

        // Dissolve
        _NoiseTex ("Noise Texture", 2D) = "white" {}
        _DissolveAmount ("Dissolve Amount", Range(0,1)) = 0
        _EdgeColor ("Edge Color", Color) = (1,0.5,0,1)
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="AlphaTest" }
        LOD 200

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);
            float4 _BaseColor;

            TEXTURE2D(_NoiseTex);
            SAMPLER(sampler_NoiseTex);

            float4 _FlashColor;
            float _FlashIntensity;
            float _DissolveAmount;
            float4 _EdgeColor;

            Varyings vert (Attributes IN)
            {
                Varyings OUT;
                OUT.positionCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;
                return OUT;
            }

            half4 frag (Varyings IN) : SV_Target
            {
                // Base texture
                half4 baseTex = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv);
                half4 baseCol = baseTex * _BaseColor;
            
                // Flash blend
                baseCol.rgb = lerp(baseCol.rgb, _FlashColor.rgb, _FlashIntensity);
            
                // Dissolve mask
                float noise = SAMPLE_TEXTURE2D(_NoiseTex, sampler_NoiseTex, IN.uv).r;
            
                // Discard pixels below dissolve threshold
                if (noise < _DissolveAmount)
                    discard;
            
                // Edge glow: only a narrow band
                float edgeBand = step(_DissolveAmount, noise) - step(_DissolveAmount + 0.02, noise);
            
                // Blend edge color only where edgeBand == 1
                baseCol.rgb = lerp(baseCol.rgb, _EdgeColor.rgb, edgeBand);
            
                return baseCol;
            }
            ENDHLSL
        }
    }
}
