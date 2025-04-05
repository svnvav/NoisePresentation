StructuredBuffer<uint> Hashes;
float4 Config;
float4 Factors;
float4 Thresholds;
uint NoiseScale;
uint Octaves = 1;
uint Lacunarity = 2;
float ZSliceOffset;


static uint HashMask = 255;

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

inline float ValueNoise(float3 value, uint period)
{
	float continuityFactor = Factors.z;
	float easingFactor = Factors.w;
	
    float3 p = value * period;
    uint3 i0 = floor(p);
    float3 t = frac(p);

    i0 &= HashMask;

    uint3 i1 = i0 + 1;
    i1 = i1 % period;

    uint h0 = Hashes[i0.x];
    uint h1 = Hashes[i1.x];
    uint h00 = Hashes[h0 + i0.y];
    uint h10 = Hashes[h1 + i0.y];
    uint h01 = Hashes[h0 + i1.y];
    uint h11 = Hashes[h1 + i1.y];
    uint h000 = Hashes[h00 + i0.z];
    uint h100 = Hashes[h10 + i0.z];
    uint h010 = Hashes[h01 + i0.z];
    uint h110 = Hashes[h11 + i0.z];
    uint h001 = Hashes[h00 + i1.z];
    uint h101 = Hashes[h10 + i1.z];
    uint h011 = Hashes[h01 + i1.z];
    uint h111 = Hashes[h11 + i1.z];

    t.x = lerp(t.x, smoothstep(0., 1., t.x), easingFactor);
    t.y = lerp(t.y, smoothstep(0., 1., t.y), easingFactor);
    t.z = lerp(t.z, smoothstep(0., 1., t.z), easingFactor);

	float keyValue = h000;
	float actualValue = lerp(
        lerp(lerp(h000, h100, t.x), lerp(h010, h110, t.x), t.y),
        lerp(lerp(h001, h101, t.x), lerp(h011, h111, t.x), t.y),
        t.z);

	return lerp(keyValue, actualValue, continuityFactor);
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
		pos.z *= 1 - voxelSize;

		int frequency = 1;
		float amplitude = 1;
		float range = 1;
	   
		float noiseValue;
	    
		for(uint o = 0; o < Octaves; o++)
		{
			noiseValue += ValueNoise(pos, frequency * NoiseScale) * amplitude;
			frequency *= Lacunarity;
			amplitude *= persistence;
			range += amplitude;
		}
		range -= amplitude;//compensate
		noiseValue /= range * HashMask;
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
