#include "../NoiseCommon.hlsl"

uniform uint NoiseScale;
uniform uint Octaves = 1;
uniform uint Lacunarity = 2;
uniform float4 Config;

static uint HashMask = 255;
static const uint HashArray[] = {
    151, 160, 137, 91, 90, 15, 131, 13, 201, 95, 96, 53, 194, 233, 7, 225,
    140, 36, 103, 30, 69, 142, 8, 99, 37, 240, 21, 10, 23, 190, 6, 148,
    247, 120, 234, 75, 0, 26, 197, 62, 94, 252, 219, 203, 117, 35, 11, 32,
    57, 177, 33, 88, 237, 149, 56, 87, 174, 20, 125, 136, 171, 168, 68, 175,
    74, 165, 71, 134, 139, 48, 27, 166, 77, 146, 158, 231, 83, 111, 229, 122,
    60, 211, 133, 230, 220, 105, 92, 41, 55, 46, 245, 40, 244, 102, 143, 54,
    65, 25, 63, 161, 1, 216, 80, 73, 209, 76, 132, 187, 208, 89, 18, 169,
    200, 196, 135, 130, 116, 188, 159, 86, 164, 100, 109, 198, 173, 186, 3, 64,
    52, 217, 226, 250, 124, 123, 5, 202, 38, 147, 118, 126, 255, 82, 85, 212,
    207, 206, 59, 227, 47, 16, 58, 17, 182, 189, 28, 42, 223, 183, 170, 213,
    119, 248, 152, 2, 44, 154, 163, 70, 221, 153, 101, 155, 167, 43, 172, 9,
    129, 22, 39, 253, 19, 98, 108, 110, 79, 113, 224, 232, 178, 185, 112, 104,
    218, 246, 97, 228, 251, 34, 242, 193, 238, 210, 144, 12, 191, 179, 162, 241,
    81, 51, 145, 235, 249, 14, 239, 107, 49, 192, 214, 31, 181, 199, 106, 157,
    184, 84, 204, 176, 115, 121, 50, 45, 127, 4, 150, 254, 138, 236, 205, 93,
    222, 114, 67, 29, 24, 72, 243, 141, 128, 195, 78, 66, 215, 61, 156, 180,

    151, 160, 137, 91, 90, 15, 131, 13, 201, 95, 96, 53, 194, 233, 7, 225,
    140, 36, 103, 30, 69, 142, 8, 99, 37, 240, 21, 10, 23, 190, 6, 148,
    247, 120, 234, 75, 0, 26, 197, 62, 94, 252, 219, 203, 117, 35, 11, 32,
    57, 177, 33, 88, 237, 149, 56, 87, 174, 20, 125, 136, 171, 168, 68, 175,
    74, 165, 71, 134, 139, 48, 27, 166, 77, 146, 158, 231, 83, 111, 229, 122,
    60, 211, 133, 230, 220, 105, 92, 41, 55, 46, 245, 40, 244, 102, 143, 54,
    65, 25, 63, 161, 1, 216, 80, 73, 209, 76, 132, 187, 208, 89, 18, 169,
    200, 196, 135, 130, 116, 188, 159, 86, 164, 100, 109, 198, 173, 186, 3, 64,
    52, 217, 226, 250, 124, 123, 5, 202, 38, 147, 118, 126, 255, 82, 85, 212,
    207, 206, 59, 227, 47, 16, 58, 17, 182, 189, 28, 42, 223, 183, 170, 213,
    119, 248, 152, 2, 44, 154, 163, 70, 221, 153, 101, 155, 167, 43, 172, 9,
    129, 22, 39, 253, 19, 98, 108, 110, 79, 113, 224, 232, 178, 185, 112, 104,
    218, 246, 97, 228, 251, 34, 242, 193, 238, 210, 144, 12, 191, 179, 162, 241,
    81, 51, 145, 235, 249, 14, 239, 107, 49, 192, 214, 31, 181, 199, 106, 157,
    184, 84, 204, 176, 115, 121, 50, 45, 127, 4, 150, 254, 138, 236, 205, 93,
    222, 114, 67, 29, 24, 72, 243, 141, 128, 195, 78, 66, 215, 61, 156, 180
};

static uint GradientsMask = 15;
static const float3 GradientArray[] = {
    float3(1, 1, 0),
    float3(-1, 1, 0),
    float3(1, -1, 0),
    float3(-1, -1, 0),

    float3(1, 0, 1),
    float3(-1, 0, 1),
    float3(1, 0, -1),
    float3(-1, 0, -1),

    float3(0, 1, 1),
    float3(0, -1, 1),
    float3(0, 1, -1),
    float3(0, -1, -1),

    float3(1, 1, 0),
    float3(-1, 1, 0),
    float3(0, -1, 1),
    float3(0, -1, -1)
};

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
    float3 p = value * period;
    uint3 i0 = floor(p);

    float3 t0 = frac(p);
    float3 t1 = t0 - 1;

    i0 &= HashMask;
    uint3 i1 = (i0 + 1) % period;

    int h0 = HashArray[i0.x];
    int h1 = HashArray[i1.x];
    int h00 = HashArray[h0 + i0.y];
    int h10 = HashArray[h1 + i0.y];
    int h01 = HashArray[h0 + i1.y];
    int h11 = HashArray[h1 + i1.y];
    float3 g000 = GradientArray[HashArray[h00 + i0.z] & GradientsMask];
    float3 g100 = GradientArray[HashArray[h10 + i0.z] & GradientsMask];
    float3 g010 = GradientArray[HashArray[h01 + i0.z] & GradientsMask];
    float3 g110 = GradientArray[HashArray[h11 + i0.z] & GradientsMask];
    float3 g001 = GradientArray[HashArray[h00 + i1.z] & GradientsMask];
    float3 g101 = GradientArray[HashArray[h10 + i1.z] & GradientsMask];
    float3 g011 = GradientArray[HashArray[h01 + i1.z] & GradientsMask];
    float3 g111 = GradientArray[HashArray[h11 + i1.z] & GradientsMask];

    float v000 = dot(g000, float3(t0.x, t0.y, t0.z));
    float v100 = dot(g100, float3(t1.x, t0.y, t0.z));
    float v010 = dot(g010, float3(t0.x, t1.y, t0.z));
    float v110 = dot(g110, float3(t1.x, t1.y, t0.z));
    float v001 = dot(g001, float3(t0.x, t0.y, t1.z));
    float v101 = dot(g101, float3(t1.x, t0.y, t1.z));
    float v011 = dot(g011, float3(t0.x, t1.y, t1.z));
    float v111 = dot(g111, float3(t1.x, t1.y, t1.z));

    float tx = QuinticCurve(t0.x);
    float ty = QuinticCurve(t0.y);
    float tz = QuinticCurve(t0.z);

    return lerp(
        lerp(lerp(v000, v100, tx), lerp(v010, v110, tx), ty),
        lerp(lerp(v001, v101, tx), lerp(v011, v111, tx), ty),
        tz);
}

float4 Noise(float3 positionWorldSpace)
{
    float alphaMultiplier = Config.z;
    float persistence = Config.w;

    float3 pos = positionWorldSpace;
    pos.x = 1 - pos.x;

    int frequency = 1;
    float amplitude = 1;
    float range = 1;

    float noiseValue;

    for (uint o = 0; o < Octaves; o++)
    {
        noiseValue += GradientNoise(pos, frequency * NoiseScale) * amplitude;
        frequency *= Lacunarity;
        amplitude *= persistence;
        range += amplitude;
    }
    range -= amplitude; //compensate
    noiseValue /= range;
    noiseValue *= 0.5;
    noiseValue += 0.5;

    float3 color = float3(1, 1, 1);

    return float4(color, noiseValue * alphaMultiplier);
}
