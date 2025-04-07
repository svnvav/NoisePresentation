#define UI0 1597334673U
#define UI1 3812015801U
#define UI3 uint3(UI0, UI1, 2798796415U)
#define UIF (1.0 / float(0xffffffffU))


uniform uint NoiseScale;
uniform uint Octaves = 1;
uniform uint Lacunarity = 2;
uniform float4 Config;
uniform float4 Factors;
uniform float4 Thresholds;
uniform float ZSliceOffset;

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

float3 hash33(uint3 q)
{
	q *= UI3;
	q = (q.x ^ q.y ^ q.z) * UI3;
	return float3(q) * UIF;
}

inline float DistanceToCellSq(float3 pos, int3 cell, uint3 period)
{
	int3 tiledCell = cell % period;
	float3 cellPosition = cell + lerp(float3(0.5, 0.5, 0.5), hash33(tiledCell), Factors.y);
    float3 toCell = cellPosition - pos;
    return dot(toCell, toCell);
}

float WorleyNoise(float3 pos, uint3 period)
{
    int3 baseCell = floor(pos);
	
    int3 cell = baseCell + int3(-1, -1, -1);
    float distSq = DistanceToCellSq(pos, cell, period);
	cell = baseCell + int3(-1, -1, 0);
	distSq = min(DistanceToCellSq(pos, cell, period), distSq);
	cell = baseCell + int3(-1, -1, 1);
	distSq = min(DistanceToCellSq(pos, cell, period), distSq);

    cell = baseCell + int3(-1, 0, -1);
    distSq = min(DistanceToCellSq(pos, cell, period), distSq);
    cell = baseCell + int3(-1, 0, 0);
    distSq = min(DistanceToCellSq(pos, cell, period), distSq);
    cell = baseCell + int3(-1, 0, 1);
    distSq = min(DistanceToCellSq(pos, cell, period), distSq);
	
	cell = baseCell + int3(-1, 1, -1);
	distSq = min(DistanceToCellSq(pos, cell, period), distSq);
    cell = baseCell + int3(-1, 1, 0);
    distSq = min(DistanceToCellSq(pos, cell, period), distSq);
	cell = baseCell + int3(-1, 1, 1);
	distSq = min(DistanceToCellSq(pos, cell, period), distSq);
//////////////////////////////////
    cell = baseCell + int3(0, -1, -1);
    distSq = min(DistanceToCellSq(pos, cell, period), distSq);
    cell = baseCell + int3(0, -1, 0);
    distSq = min(DistanceToCellSq(pos, cell, period), distSq);
    cell = baseCell + int3(0, -1, 1);
    distSq = min(DistanceToCellSq(pos, cell, period), distSq);

    cell = baseCell + int3(0, 0, -1);
    distSq = min(DistanceToCellSq(pos, cell, period), distSq);
    cell = baseCell;// + float3(0, 0, 0);
    distSq = min(DistanceToCellSq(pos, cell, period), distSq);
    cell = baseCell + int3(0, 0, 1);
    distSq = min(DistanceToCellSq(pos, cell, period), distSq);

    cell = baseCell + int3(0, 1, -1);
    distSq = min(DistanceToCellSq(pos, cell, period), distSq);
    cell = baseCell + int3(0, 1, 0);
    distSq = min(DistanceToCellSq(pos, cell, period), distSq);
    cell = baseCell + int3(0, 1, 1);
    distSq = min(DistanceToCellSq(pos, cell, period), distSq);
//////////////////////////////////
	cell = baseCell + int3(1, -1, -1);
	distSq = min(DistanceToCellSq(pos, cell, period), distSq);
	cell = baseCell + int3(1, -1, 0);
	distSq = min(DistanceToCellSq(pos, cell, period), distSq);
	cell = baseCell + int3(1, -1, 1);
	distSq = min(DistanceToCellSq(pos, cell, period), distSq);

    cell = baseCell + int3(1, 0, -1);
    distSq = min(DistanceToCellSq(pos, cell, period), distSq);
    cell = baseCell + int3(1, 0, 0);
    distSq = min(DistanceToCellSq(pos, cell, period), distSq);
    cell = baseCell + int3(1, 0, 1);
    distSq = min(DistanceToCellSq(pos, cell, period), distSq);

	cell = baseCell + int3(1, 1, -1);
	distSq = min(DistanceToCellSq(pos, cell, period), distSq);
	cell = baseCell + int3(1, 1, 0);
	distSq = min(DistanceToCellSq(pos, cell, period), distSq);
	cell = baseCell + int3(1, 1, 1);
	distSq = min(DistanceToCellSq(pos, cell, period), distSq);

    return sqrt(distSq);
}

float4 Noise(float3 positionWorldSpace)
{
    #if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
	    float voxelSize = Config.y;
		float multiplier = Config.z;
		float persistence = Config.w;
	
		float axisColorFactor = Factors.x;


		int frequency = 1;
		float amplitude = 1;
		float range = 1;

		float3 pos = positionWorldSpace;
		pos.x = 1 - pos.x;
		
		float3 cellPos = pos * NoiseScale;

		float noiseValue;

		for(uint o = 0; o < Octaves; o++)
		{
			range += amplitude;
			noiseValue += WorleyNoise(cellPos * frequency,frequency * NoiseScale) * amplitude;
			frequency *= Lacunarity;
			amplitude *= persistence;
		}

		noiseValue /= range;

		noiseValue *= step(Thresholds.w, noiseValue);
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

