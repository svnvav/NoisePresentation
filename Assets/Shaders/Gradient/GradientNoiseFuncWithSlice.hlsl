#include "../NoiseCommon.hlsl"

uniform uint NoiseScale;
uniform uint Octaves = 1;
uniform uint Lacunarity = 2;
uniform float4 Config;
uniform float4 Factors;
uniform float4 Thresholds; 
uniform float ZSliceOffset;

StructuredBuffer<uint> Hashes;
static uint HashMask = 255;
StructuredBuffer<float3> Gradients;
static uint GradientsMask = 15;

void ConfigureProcedural()
{
    #if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
		float resolution = Config.x;
		float voxelSize = Config.y;

		float pz = floor(voxelSize * voxelSize * unity_InstanceID + 0.00001);
		float py = floor(voxelSize * (unity_InstanceID - resolution * resolution * pz) + 0.00001);
		float px = unity_InstanceID - resolution * py - resolution * resolution * pz;
		
		unity_ObjectToWorld = 0.0;
		unity_ObjectToWorld._m03_m13_m23_m33 = float4(
			voxelSize * px,
			voxelSize * py,
			voxelSize * pz,
			1.0
		);
		unity_ObjectToWorld._m00_m11_m22 = voxelSize;
    #endif
}

float GradientNoise(float3 value, uint period)
{
    float randomFactor = Factors.y;
    float easingFactor = Factors.w;
    
    float3 p = value * period;
    uint3 i0 = floor(p);
    
    float3 t0 = frac(p);
    float3 t1 = t0 - 1;
    
    i0 &= HashMask;
    uint3 i1 = (i0 + 1) % period;
    
    int h0 = Hashes[i0.x];
    int h1 = Hashes[i1.x];
    int h00 = Hashes[h0 + i0.y];
    int h10 = Hashes[h1 + i0.y];
    int h01 = Hashes[h0 + i1.y];
    int h11 = Hashes[h1 + i1.y];
    float3 g000 = (Gradients[Hashes[h00 + i0.z] & GradientsMask]);
    float3 g100 = (Gradients[Hashes[h10 + i0.z] & GradientsMask]);
    float3 g010 = (Gradients[Hashes[h01 + i0.z] & GradientsMask]);
    float3 g110 = (Gradients[Hashes[h11 + i0.z] & GradientsMask]);
    float3 g001 = (Gradients[Hashes[h00 + i1.z] & GradientsMask]);
    float3 g101 = (Gradients[Hashes[h10 + i1.z] & GradientsMask]);
    float3 g011 = (Gradients[Hashes[h01 + i1.z] & GradientsMask]);
    float3 g111 = (Gradients[Hashes[h11 + i1.z] & GradientsMask]);

    g000 = lerp(float3(1, 0, 0), g000, randomFactor);
    g100 = lerp(float3(1, 0, 0), g100, randomFactor);
    g010 = lerp(float3(1, 0, 0), g010, randomFactor);
    g110 = lerp(float3(1, 0, 0), g110, randomFactor);
    g001 = lerp(float3(1, 0, 0), g001, randomFactor);
    g101 = lerp(float3(1, 0, 0), g101, randomFactor);
    g011 = lerp(float3(1, 0, 0), g011, randomFactor);
    g111 = lerp(float3(1, 0, 0), g111, randomFactor);

    float v000 = dot( g000, float3(t0.x, t0.y, t0.z) );
    float v100 = dot( g100, float3(t1.x, t0.y, t0.z) );
    float v010 = dot( g010, float3(t0.x, t1.y, t0.z) );
    float v110 = dot( g110, float3(t1.x, t1.y, t0.z) );
    float v001 = dot( g001, float3(t0.x, t0.y, t1.z) );
    float v101 = dot( g101, float3(t1.x, t0.y, t1.z) );
    float v011 = dot( g011, float3(t0.x, t1.y, t1.z) );
    float v111 = dot( g111, float3(t1.x, t1.y, t1.z) );

    float tx = lerp(t0.x, QuinticCurve(t0.x), easingFactor);
    float ty = lerp(t0.y, QuinticCurve(t0.y), easingFactor);
    float tz = lerp(t0.z, QuinticCurve(t0.z), easingFactor);
     
    return lerp(
       lerp(lerp(v000, v100, tx), lerp(v010, v110, tx), ty),
       lerp(lerp(v001, v101, tx), lerp(v011, v111, tx), ty),
       tz);
}

float4 Noise(float3 positionWorldSpace)
{
    #if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
	    float voxelSize = Config.y;
		float multiplier = Config.z;
		float persistence = Config.w;
		
		float randomFactor = Factors.y;
		float axisColorFactor = Factors.x;

		float3 pos = positionWorldSpace;
		pos.x = 1 - pos.x;
	    
		int frequency = 1;
		float amplitude = 1;
		float range = 1;
	   
		float noiseValue;
	    
		for(uint o = 0; o < Octaves; o++)
		{
			noiseValue += GradientNoise(pos, frequency * NoiseScale) * amplitude;
			frequency *= Lacunarity;
			amplitude *= persistence;
			range += amplitude;
		}
		range -= amplitude;//compensate
		noiseValue /= range;
		noiseValue *= 0.5;
		noiseValue += 0.5;
		noiseValue *= step(Thresholds.w, noiseValue);
		noiseValue = lerp(0.5, noiseValue, randomFactor);
		noiseValue *=	step(Thresholds.x, 1 - positionWorldSpace.x) *
						step(Thresholds.y, 1 - positionWorldSpace.y) *
						step(Thresholds.z, 1 - positionWorldSpace.z);
		float3 color = lerp(1, positionWorldSpace, axisColorFactor);
		if(ZSliceOffset - voxelSize * 0.001 < positionWorldSpace.z && positionWorldSpace.z < ZSliceOffset + voxelSize * 1.001)
		{
			return float4(noiseValue, 0, noiseValue, 1);
		}
		else
		{
			return float4(
			color,
			noiseValue * multiplier
			);
		}
    #else
        return 1.0;
    #endif
}