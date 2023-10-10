Shader "Unlit/FieldLineGenerator" {
    Properties { 
        _StepSize ("Step Size", Range(0,0.1)) = 0.0169
        _MaxFieldThreshold ("Max Field Threshold", Range(100,10000)) = 4500
        _MinFieldThreshold ("Min Field Threshold", Range(0,10)) = 0.58
        _MaxDistance ("Max Distance", Range(0,10)) = 6.17
    }

    SubShader {
        Tags { 
            "RenderType" = "Opaque" 
        }

        Pass {
            Cull Off

            CGPROGRAM

            #pragma target 5.0

            #pragma vertex Vertex
            #pragma geometry Geometry
            #pragma fragment Fragment

            #include "UnityCG.cginc"

            struct MeshData {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct ControlPoints {
                float4 positionWS : TEXCOORD0;
                float2 uv : TEXCOORD1;
            };

            struct Interpolators {
                float4 positionCS : SV_POSITION;
            };

            float _StepSize;
            float _MaxFieldThreshold;
            float _MaxDistance;
            float _MinFieldThreshold;

            //Shader arrays are required to be a fixed size at compile time but the size can be arbitrarily large
            int _NumberOfCharges;
            float4 _Charges[10];

            //Vertex shader
            ControlPoints Vertex(MeshData input) {
                ControlPoints output;

                //Passing mesh data that is already in world space to geometry shader.  
                //The x component of the uv coord specifies whether a given point is a source or a sink 

                output.positionWS = input.positionOS;
                output.uv = input.uv;

                return output;
            }

            //Utility functions for color schemes
            float InverseLerp(float a, float b, float v) {return (v-a)/(b-a);}
            float Remap(float iMin, float iMax, float oMin, float oMax, float v) {float t = InverseLerp(iMin, iMax, v); return lerp(oMin, oMax, t);}

            //Loops through all of the charges and computes the electric field at a point.  The w component of _Charges is the charge
            float3 calculateElectricFieldFromPointCharges(float3 radialVector) {
                float3 electricField = float3(0,0,0);

                for(int i = 0; i < _NumberOfCharges; i++) {
                    electricField += _Charges[i].w/pow(length(radialVector - _Charges[i].xyz), 2) * normalize(radialVector - _Charges[i].xyz);
                }

                return electricField;
            } 

            //Geometry shader -- inputs source or sink points are computes field lines containing <= 250 line segments
            [maxvertexcount(251)]
            void Geometry(point ControlPoints input[1], inout LineStream<Interpolators> lineStream) {
                Interpolators output;

                float4 currentPoint = input[0].positionWS;
                
                for(int i = 0; i < 250; i++) {
                    output.positionCS = UnityWorldToClipPos(currentPoint);

                    float3 field = input[0].uv.x * calculateElectricFieldFromPointCharges(currentPoint.xyz);
                    if(length(field) > _MaxFieldThreshold || length(field) < _MinFieldThreshold || length(currentPoint) > _MaxDistance) {break;}
                    
                    currentPoint += float4(normalize(field) * _StepSize,0);
                    lineStream.Append(output);
                } 

                lineStream.RestartStrip();
            }

            //Fragment shader
            float4 Fragment(Interpolators input) : SV_Target {
                return float4(1,0,0,1);
            }

            ENDCG
        }
    }
}