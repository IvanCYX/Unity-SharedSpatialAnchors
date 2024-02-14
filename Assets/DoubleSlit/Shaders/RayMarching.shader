Shader "Unlit/Raymarching" {
    Properties {
        _StepSize ("Step Size", Range(0.001,0.01)) = 0.005
        _ProbabiltyDensityCutoffR ("Probabilty Density Cutoff R", Range(0,1)) = 0.1
        _ProbabiltyDensityCutoffG ("Probabilty Density Cutoff G", Range(0,1)) = 0.1
        _ProbabiltyDensityCutoffB ("Probabilty Density Cutoff B", Range(0,1)) = 0.1

        [IntRange] _Nx ("Nx", Range(1,10)) = 1
        [IntRange] _Ny ("Ny", Range(1,10)) = 1
        [IntRange] _Nz ("Ny", Range(1,10)) = 1
    }

    SubShader {
        Tags { 
            "RenderType" = "Transparent"
            "Queue" = "Transparent"
        }

        Pass {
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct MeshData {
                float4 vertex : POSITION;
            };

            struct Interpolators {
                float4 vertex : SV_POSITION;
                float3 position : TEXCOORD0;
                float3 directionToCamera : TEXCOORD1;
            };

            #define Epsilon 0.000001
            #define _PI 3.141592653

            float _StepSize;
            float _ProbabiltyDensityCutoffR;
            float _ProbabiltyDensityCutoffG;
            float _ProbabiltyDensityCutoffB;

            int _Nx;
            int _Ny;
            int _Nz;

            float InverseLerp(float a, float b, float v) {return (v-a)/(b-a);}
            float Remap(float iMin, float iMax, float oMin, float oMax, float v) {float t = InverseLerp(iMin, iMax, v); return lerp(oMin, oMax, t);}

            Interpolators vert(MeshData v) {
                Interpolators o;

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.position = v.vertex;

                float4 objectSpaceCamera = mul(unity_WorldToObject, float4(_WorldSpaceCameraPos, 1.0));
                o.directionToCamera = normalize(objectSpaceCamera - v.vertex);

                return o;
            }

            float WaveFunction(float x, int n) {
                return cos(n * _PI * x / 2);
            }
            
            float Wavefunction3d(float3 position) {
                return WaveFunction(position.x, _Nx) * WaveFunction(position.y, _Ny) * WaveFunction(position.z, _Nz); 
            } 

            float f1(float3 position) {
                return length(position);
            }

            float4 transferFunction1(float f) {
                if(f < 0.2) {
                    return float4(0.5, 0, 0, 0.1);
                } else if(f < 0.4) {
                    return float4(0, 0, 0.1, 0.3);
                } else if(f < 0.6) {
                    return float4(0, 0, 0, 0.4);
                }

                return float4(0, 0, 0, 0);
            }

            float4 WaveFunctionTransferFunction(float f) {
                float probablityDensity = f * f;

                if(probablityDensity > _ProbabiltyDensityCutoffR) {
                    return float4(1,0,0, probablityDensity * probablityDensity);
                } else if(probablityDensity > _ProbabiltyDensityCutoffG) {
                    return float4(0,0.5,0, probablityDensity * probablityDensity);
                } else if(probablityDensity > _ProbabiltyDensityCutoffB) {
                    return float4(0,0,0.2, probablityDensity * probablityDensity);
                }

                return float4(0,0,0,0);
            }

            float3 blendColors(float3 color1, float3 color2, float alpha1) {
                return color1 + color2 * (1 - alpha1);
            }

            float blendAlphas(float1 alpha1, float2 alpha2) {
                return alpha1 + alpha2 * (1 - alpha1);
            }

            float4 frag(Interpolators i) : SV_Target {
                float3 rayOrigin = -i.position;
                float3 samplePosition = rayOrigin;

                float4 currentColor = float4(0, 0, 0, 0);
                float4 sampledColor;
                float stepSizeColorCorrection = _StepSize * 10;

                while(length(samplePosition) < 1 + Epsilon) {
                    //sampledColor = transferFunction1(f1(samplePosition)) * stepSizeColorCorrection;
                    sampledColor = WaveFunctionTransferFunction(Wavefunction3d(samplePosition)) * stepSizeColorCorrection;

                    currentColor.rgb = blendColors(currentColor.rgb, sampledColor.rgb, currentColor.a);
                    currentColor.a = blendAlphas(currentColor.a, sampledColor.a);

                    samplePosition += _StepSize * i.directionToCamera;
                }

                return currentColor;
            }

            ENDCG
        }
    }
}