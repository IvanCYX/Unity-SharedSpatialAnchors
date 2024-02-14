Shader "Unlit/TessTest" {
    Properties {
        [Header(Tessellation)]
        _InTessLevel ("Inner Tesselation Level", Range(1,100)) = 1
        _OutTessLevel ("Outer Tesselation Level", Range(1,100)) = 1

        [Header(Lighting)]
        _Ks ("Specular Reflection Constant", Range(0,1)) = 0.5
        _Kd ("Diffuse Reflection Constant", Range(0,1)) = 0.5
        _Ka ("Ambient Reflection Constant", Range(0,1)) = 0.5
        _A ("Shininess Constant", Range(0,1)) = 0.5
        _Ia ("Ambient Lighting", Range(0,1)) = 0.1
        _Is ("Specular Intensity", Range(0,1)) = 0.5
        _Id ("Diffuse Intensity", Range(0,1)) = 0.5
    }

    SubShader {
        Tags { 
            "RenderType" = "Opaque" 
        }

        Pass {
            CGPROGRAM

            #pragma target 5.0

            #pragma vertex Vertex
            #pragma hull Hull
            #pragma domain Domain
            #pragma fragment Fragment

            #include "UnityCG.cginc"

            #define BARYCENTRIC_INTERPOLATE(fieldName) \
                patch[0].fieldName * barycentricCoordinates.x + \
                patch[1].fieldName * barycentricCoordinates.y + \
                patch[2].fieldName * barycentricCoordinates.z

            struct MeshData {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
            };

            struct TessellationControlPoint {
                float3 positionOS : INTERNALTESSPOS;
                float3 normalOS : NORMAL;
            };

            struct TessellationFactors {
                float edge[3] : SV_TessFactor;
                float inside : SV_INSIDETESSFACTOR;
            };

            struct Interpolators {
                float4 positionCS : SV_POSITION;
                float3 positionOS : TEXCOORD0;
                float3 normalOS : TEXCOORD1;
            };

            float _InTessLevel;
            float _OutTessLevel;

            float _Ks; float _Kd; float _Ka; float _A;
            float _Ia; float _Is; float _Id;

            TessellationControlPoint Vertex(MeshData input) {
                TessellationControlPoint output;
                output.positionOS = input.positionOS;
                output.normalOS = input.normalOS;
                return output;
            }

            [domain("tri")] [outputcontrolpoints(3)] [outputtopology("triangle_cw")] [patchconstantfunc("PatchConstantFunction")] [partitioning("integer")]
            TessellationControlPoint Hull(InputPatch<TessellationControlPoint,3> patch, uint id: SV_OUTPUTCONTROLPOINTID) {
                return patch[id];
            }

            TessellationFactors PatchConstantFunction(InputPatch<TessellationControlPoint,3> patch) {
                TessellationFactors factor;
                factor.edge[0] = _OutTessLevel; factor.edge[1] = _OutTessLevel; factor.edge[2] = _OutTessLevel; 
                factor.inside = _InTessLevel;
                return factor;
            }

            [domain("tri")]
            Interpolators Domain(TessellationFactors factors, OutputPatch<TessellationControlPoint,3> patch, float3 barycentricCoordinates : SV_DOMAINLOCATION) {
                Interpolators output;

                float3 positionOS = BARYCENTRIC_INTERPOLATE(positionOS);
                float3 normalOS = BARYCENTRIC_INTERPOLATE(normalOS);

                positionOS.y = cos(positionOS.x * 10 + _Time.y) * 0.05;
                normalOS = -normalize(-sin(positionOS.x * 10 + _Time.y) * 0.5 * float3(1,0,0) + -1 * float3(0,1,0));

                output.positionCS = UnityObjectToClipPos(float4(positionOS,1));
                output.positionOS = positionOS;
                output.normalOS = normalOS;

                return output;
            }

            float4 Fragment(Interpolators input) : SV_Target {
                float3 normal = input.normalOS;
                float3 light = normalize(ObjSpaceLightDir(float4(input.positionOS,1)));
                float3 reflection = normalize(2 * dot(light, normal) * normal - light);
                float3 camera = normalize(ObjSpaceViewDir(float4(input.positionOS,1)));

                float illumination = _Ka * _Ia + _Kd * _Id * dot(light, normal) + _Ks * _Is * pow(dot(reflection, camera), _A);
                //float illumination = _Ka * _Ia + _Kd * _Id * dot(light, normal);
                float3 albedo = lerp(float3(0,0,1), float3(1,0,0), input.positionOS.y * 10 + 0.5);
                //float3 albedo = float3(1,0,0);

                return float4(illumination * albedo, 1);
            }

            ENDCG
        }
    }
}
