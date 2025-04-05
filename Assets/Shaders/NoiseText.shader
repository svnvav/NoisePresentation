Shader "Unlit/NoiseText"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _AlphaThreshold ("Alpha Threshold", Range (0, 1)) = 1
        _BlendOpacity ("Blend Opacity", Range (0, 1)) = 1
        _NoiseScale ("Noise Scale", Range (1, 128)) = 8
        _TimeScale ("Time Scale", Range(0, 16)) = 1
    }
    SubShader
    {
        Tags { 
            "RenderType"="Transparent" 
            "Queue"="Transparent"
            }
        Cull Back
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _AlphaThreshold;
            float _BlendOpacity;
            float _NoiseScale;
            float _TimeScale;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            float QuinticCurve(float x)
            {
                return x * x * x * (x * (x * 6. - 15.) + 10.);
            }

            //get a scalar random value from a 3d value
            float rand3dTo1d(float3 value, float3 dotDir = float3(12.9898, 78.233, 37.719)){
	            //make value smaller to avoid artefacts
	            float3 smallValue = cos(value);
	            //get scalar value from 3d vector
	            float random = dot(smallValue, dotDir);
	            //make value more random by making it bigger and then taking the factional part
	            random = frac(sin(random) * 143758.5453);
	            return random;
            }
            
            float3 rand3dTo3d(float3 value){
	            return float3(
		            rand3dTo1d(value, float3(12.989, 78.233, 37.719)),
		            rand3dTo1d(value, float3(39.346, 11.135, 83.155)),
		            rand3dTo1d(value, float3(73.156, 52.235, 09.151))
	            );
            }
            
            float GradientNoise(float3 value, uint period)
            {
                float3 p = value * period;
                uint3 i0 = floor(p);
                
                float3 t0 = frac(p);
                float3 t1 = t0 - 1;
                
                float3 g000 = rand3dTo1d((i0 + uint3(0, 0, 0)) % period) * 2 - 1;
                float3 g100 = rand3dTo1d((i0 + uint3(1, 0, 0)) % period) * 2 - 1;
                float3 g010 = rand3dTo1d((i0 + uint3(0, 1, 0)) % period) * 2 - 1;
                float3 g110 = rand3dTo1d((i0 + uint3(1, 1, 0)) % period) * 2 - 1;
                float3 g001 = rand3dTo1d((i0 + uint3(0, 0, 1)) % period) * 2 - 1;
                float3 g101 = rand3dTo1d((i0 + uint3(1, 0, 1)) % period) * 2 - 1;
                float3 g011 = rand3dTo1d((i0 + uint3(0, 1, 1)) % period) * 2 - 1;
                float3 g111 = rand3dTo1d((i0 + uint3(1, 1, 1)) % period) * 2 - 1;

                float v000 = dot( g000, float3(t0.x, t0.y, t0.z) );
                float v100 = dot( g100, float3(t1.x, t0.y, t0.z) );
                float v010 = dot( g010, float3(t0.x, t1.y, t0.z) );
                float v110 = dot( g110, float3(t1.x, t1.y, t0.z) );
                float v001 = dot( g001, float3(t0.x, t0.y, t1.z) );
                float v101 = dot( g101, float3(t1.x, t0.y, t1.z) );
                float v011 = dot( g011, float3(t0.x, t1.y, t1.z) );
                float v111 = dot( g111, float3(t1.x, t1.y, t1.z) );

                float tx = QuinticCurve(t0.x);
                float ty = QuinticCurve(t0.y);
                float tz = QuinticCurve(t0.z);
                 
                return lerp(
                   lerp(lerp(v000, v100, tx), lerp(v010, v110, tx), ty),
                   lerp(lerp(v001, v101, tx), lerp(v011, v111, tx), ty),
                   tz);
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                

                uint scale = floor(_NoiseScale);
                float3 noiseUv = float3(i.uv, (_Time.x * _TimeScale) % 16);
                float noise = GradientNoise(noiseUv, scale);
            	noise *= 0.5;
            	noise += 0.5;

                fixed4 result = col * (1 - _BlendOpacity) + noise.xxxx * _BlendOpacity;

                result.rgba = step(_AlphaThreshold, result.r);
                
                return result;
            }
            ENDCG
        }
    }
}
