Shader "Unlit/ParticleInABoxTime" {
    Properties {
        [Header(Quality)]
        _StepSize ("Step Size", Range(0.001,0.01)) = 0.005
        _Scale ("Scale", Range(0.5,1)) = 1
        [IntRange] _Outline ("Toggle Outline", Range(0,1)) = 0

        [Header(Transfer Function)]
        _ProbabiltyDensityCutoffR ("Probabilty Density Cutoff R", Range(0,1)) = 0.1
        _ProbabiltyDensityCutoffG ("Probabilty Density Cutoff G", Range(0,1)) = 0.1
        _ProbabiltyDensityCutoffB ("Probabilty Density Cutoff B", Range(0,1)) = 0.1

        [Header(Wave Functions)]
        [IntRange] _Nx1 ("Nx1", Range(1,10)) = 1
        [IntRange] _Ny1 ("Ny1", Range(1,10)) = 1
        [IntRange] _Nz1 ("Nz1", Range(1,10)) = 1

        [IntRange] _Nx2 ("Nx2", Range(1,10)) = 2
        [IntRange] _Ny2 ("Ny2", Range(1,10)) = 1
        [IntRange] _Nz2 ("Nz2", Range(1,10)) = 1

        _WaveFunctionRatio ("Wave Function Ratio", Range(0,1)) = 0.5
        _FrequencyScale ("Frequency Scale", Range(0,10)) = 1
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
            float _Scale;
            bool _Outline;

            uint _Nx1;
            uint _Ny1;
            uint _Nz1;

            uint _Nx2;
            uint _Ny2;
            uint _Nz2;

            float _WaveFunctionRatio;
            float _FrequencyScale;

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

            bool checkBoundingBox(float3 position) {
                return abs(position.x) < 0.5 + Epsilon && abs(position.y) < 0.5 + Epsilon && abs(position.z) < 0.5 + Epsilon;
            }

            float WaveFunctionEven(float x, uint n) {
                return cos(n * _PI * x);
            }

            float WaveFunctionOdd(float x, uint n) {
                return sin(n * _PI * x);
            }
            
            float Wavefunction3d(float3 position, uint nX, uint nY, uint nZ) {
                position /= _Scale;
                if(!checkBoundingBox(position)) {return 0;}

                float xWaveFunction; float yWaveFunction; float zWaveFunction;

                if(nX % 2 != 0) {
                    xWaveFunction = WaveFunctionEven(position.x, nX);
                } else {
                    xWaveFunction = WaveFunctionOdd(position.x, nX);
                }

                if(nY % 2 != 0) {
                    yWaveFunction = WaveFunctionEven(position.y, nY);
                } else {
                    yWaveFunction = WaveFunctionOdd(position.y, nY);
                }

                if(nZ % 2 != 0) {
                    zWaveFunction = WaveFunctionEven(position.z, nZ);
                } else {
                    zWaveFunction = WaveFunctionOdd(position.z, nZ);
                }
               
                return xWaveFunction * yWaveFunction * zWaveFunction; 

            } 

            float TimeWavefuctionProbabiltyDensity(float3 position) {
                float c1 = sqrt(_WaveFunctionRatio);
                float c2 = sqrt(1 - _WaveFunctionRatio);

                float waveFunction1 = c1 * Wavefunction3d(position, _Nx1, _Ny1,  _Nz1);
                float waveFunction2 = c2 * Wavefunction3d(position, _Nx2, _Ny2,  _Nz2);

                float waveFunctionEnergy1 = _Nx1 * _Nx1 + _Ny1 * _Ny1 + _Nz1 * _Nz1;
                float waveFunctionEnergy2 = _Nx2 * _Nx2 + _Ny2 * _Ny2 + _Nz2 * _Nz2;

                float waveFunction = waveFunction1 * waveFunction1 + waveFunction2 * waveFunction2 + 2 * 
                                     waveFunction1 * waveFunction2 * cos(_Time.x * (waveFunctionEnergy1 - waveFunctionEnergy2) * _FrequencyScale);
                return waveFunction;
            }

            float4 WaveFunctionTransferFunction(float f) {
                float probablityDensity = f * f;

                if(probablityDensity > _ProbabiltyDensityCutoffR) {
                    return float4(1,0,0, probablityDensity * probablityDensity);
                } else if(probablityDensity > _ProbabiltyDensityCutoffG) {
                    return float4(0,0.5,0, probablityDensity * probablityDensity / 2);
                } else if(probablityDensity > _ProbabiltyDensityCutoffB) {
                    return float4(0,0,0.2, probablityDensity * probablityDensity / 4);
                } else if(probablityDensity > 0 && _Outline) {
                    return float4(0.1,0.1,0.1,0.1);
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
                float3 rayOrigin = i.position;
                float3 samplePosition = rayOrigin;

                float4 currentColor = float4(0, 0, 0, 0);
                float4 sampledColor;
                float stepSizeColorCorrection = _StepSize * 10;

                while(checkBoundingBox(samplePosition)) {
                    sampledColor = WaveFunctionTransferFunction(TimeWavefuctionProbabiltyDensity(samplePosition)) * stepSizeColorCorrection;

                    currentColor.rgb = blendColors(currentColor.rgb, sampledColor.rgb, currentColor.a);
                    currentColor.a = blendAlphas(currentColor.a, sampledColor.a);

                    samplePosition -= _StepSize * i.directionToCamera;
                }

                return currentColor;
            }

            ENDCG
        }
    }
}