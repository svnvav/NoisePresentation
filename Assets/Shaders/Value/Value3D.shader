Shader "Custom/Value3DNoise"
{
    Properties {}
    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalPipeline"
            "RenderType"="Transparent"
            "UniversalMaterialType" = "Unlit"
            "Queue"="Transparent"
        }
        Pass
        {
            Name "Pass"
            Tags
            {

            }

            // Render State
            Cull Back
            Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
            ZTest LEqual
            ZWrite Off

            HLSLPROGRAM

            #pragma target 4.5
            #pragma exclude_renderers gles gles3 glcore
            #pragma multi_compile_instancing
            #pragma multi_compile _ DOTS_INSTANCING_ON
            #pragma vertex vert
            #pragma fragment frag

            // Includes
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "ValueNoiseFunc.hlsl"

            // --------------------------------------------------
            // Structs and Packing

            struct Attributes
            {
                float3 positionOS : POSITION;
                #if UNITY_ANY_INSTANCING_ENABLED
                    uint instanceID : INSTANCEID_SEMANTIC;
                #endif
            };

            struct VertexOutput
            {
                float4 positionCS : SV_POSITION;
                float4 color : COLOR;
            };

            VertexOutput vert(Attributes input)
            {
                VertexOutput output;

                UNITY_SETUP_INSTANCE_ID(input);
                
                #pragma instancing_options assumeuniformscaling procedural:ConfigureProcedural
                #pragma editor_sync_compilation

                float3 positionWS = TransformObjectToWorld(input.positionOS);
                
                output.positionCS = TransformWorldToHClip(positionWS);
                output.color = Noise(positionWS);

                return output;
            }

            half4 frag(VertexOutput vertexData) : SV_TARGET
            {
                return half4(vertexData.color);
            }
            ENDHLSL
        }
    }


    FallBack "Hidden/Shader Graph/FallbackError"
}