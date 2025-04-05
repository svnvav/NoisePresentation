#define Skewers float2(0.1666667, 0.3333333)

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

float FallOff(float3 t)
{
	float f = 0.5 - t.x * t.x - t.y * t.y - t.z * t.z;
	if(f > 0.)
	{
		return f * f * f;
	}
	return 0;
}

float SimplexValuePart(float3 p, int3 i)
{
	float unskew = Factors.w * dot(i, Skewers.xxx);
	float3 unskewed = p - i + unskew;
	float h = Hashes[Hashes[Hashes[i.x & HashMask] + i.y & HashMask] + i.z & HashMask];
	return FallOff(unskewed) * h;
}

float Noise(float3 position, uint scale)
{
	float3 pos = position * scale;
	float3 skewed = pos + Factors.w * dot(pos, Skewers.yyy);
	
	int3 i = floor(skewed);
    
	float sample = SimplexValuePart(pos, i);
	sample += SimplexValuePart(pos, i + 1);
    
	float x = skewed.x - i.x;
	float y = skewed.y - i.y;
	float z = skewed.z - i.z;
	if (x >= y) {
		if (x >= z) {
			sample += SimplexValuePart(pos, int3(i.x + 1, i.y, i.z));
			if (y >= z) {
				sample += SimplexValuePart(pos, int3(i.x + 1, i.y + 1, i.z));
			}
			else {
				sample += SimplexValuePart(pos, int3(i.x + 1, i.y, i.z + 1));
			}
		}
		else {
			sample += SimplexValuePart(pos, int3(i.x, i.y, i.z + 1));
			sample += SimplexValuePart(pos, int3(i.x + 1, i.y, i.z + 1));
		}
	}
	else {
		if (y >= z) {
			sample += SimplexValuePart(pos, int3(i.x, i.y + 1, i.z));
			if (x >= z) {
				sample += SimplexValuePart(pos, int3(i.x + 1, i.y + 1, i.z));
			}
			else {
				sample += SimplexValuePart(pos, int3(i.x, i.y + 1, i.z + 1));
			}
		}
		else {
			sample += SimplexValuePart(pos, int3(i.x, i.y, i.z + 1));
			sample += SimplexValuePart(pos, int3(i.x, i.y + 1, i.z + 1));
		}
	}

	float keyColor0 = float(Hashes[Hashes[Hashes[i.x & HashMask] + i.y & HashMask] + i.z & HashMask]) / HashMask;
	
	return lerp(keyColor0, sample * 8 * 2 / HashMask - 1, Factors.z);
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

		uint frequency = 1;
		float amplitude = 1;
		float range = 1;
	   
		float noiseValue;
	    
		for(uint o = 0; o < Octaves; o++)
		{
			noiseValue += Noise(pos, frequency * NoiseScale) * amplitude;
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
