Shader "Unlit/3DWaves" {
    Properties{
        //Quality properties 
        [Header(Quality)]
        _StepSize("Step Size", Range(0.001,0.01)) = 0.001
        _Scale("Scale", Range(0.5,1)) = 1
        _Height("Height", Range(0,1)) = 0.49
        _Width("Width", Range(0,1)) = 0.166
        _Outline("Outline Strength", Range(0,1)) = 0.12

            //Transfer function properties
            [Header(Transfer Function)]
            _ProbabiltyDensityCutoffR("Probabilty Density Cutoff R", Range(0,1)) = 0.478
            _ProbabiltyDensityCutoffG("Probabilty Density Cutoff G", Range(0,1)) = 0.165
            _ProbabiltyDensityCutoffB("Probabilty Density Cutoff B", Range(0,1)) = 0.01

                //Plane wave properties
                [Header(Plane Wave)]
                _waveNumber("Wave number k", Range(10, 50)) = 23.6
                _angularVelocity("Angular Velocity w", Range(0, 5)) = 2.21
                _gaussianCoefficient("Gaussian Coefficient", Range(1, 50)) = 20.4
                _spreadStart("Spread Start", Range(0, 1)) = 0
                _spreadCoefficient("Spread Coefficient", Range(0.5, 2)) = 1

                    //Interfernce wave properties
                    [Header(Interference Wave)]
                    _slitStart("Slit Start", Range(-1, 1)) = 0
                    _slitSpacing("Slit Spacing", Range(0, 1)) = 0.465
                    _widthScale("Width Scale", Range(1, 5)) = 3.53
                    _intensityScale("Intensity Scale", Range(0, 1)) = 0.895
    }

        SubShader{
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
                float _Height;
                float _Width;
                float _Scale;
                float _Outline;

                float _waveNumber;
                float _angularVelocity;
                float _gaussianCoefficient;
                float _spreadStart;
                float _spreadCoefficient;

                float _slitStart;
                float _slitSpacing;
                float _widthScale;
                float _intensityScale;

                //Utility functions
                float InverseLerp(float a, float b, float v) { return (v - a) / (b - a); }
                float Remap(float iMin, float iMax, float oMin, float oMax, float v) { float t = InverseLerp(iMin, iMax, v); return lerp(oMin, oMax, t); }

                //Vertex shader
                Interpolators vert(MeshData v) {
                    Interpolators o;

                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.position = v.vertex;

                    //Find direction from camera position to vertex position in object space
                    float4 objectSpaceCamera = mul(unity_WorldToObject, float4(_WorldSpaceCameraPos, 1.0));
                    o.directionToCamera = normalize(objectSpaceCamera - v.vertex);

                    return o;
                }

                //Bounding box functions
                bool checkBoundingBox(float3 position) {
                    return abs(position.y) < 2 + Epsilon && abs(position.x) < 1 + Epsilon && abs(position.z) < 1 + Epsilon;
                }

                bool checkInnerBoundingBox(float3 position) {
                    return abs(position.y) < 2 * _Height + Epsilon && abs(position.x) < 2 * _Width + Epsilon && abs(position.z) < 2 * _Width + Epsilon;
                }

                //Wave functions
                float PlaneWave(float3 position) {
                    return sin(position.y * _waveNumber - _Time.y * _angularVelocity);
                }

                float InterferenceWave(float3 position) {
                    position.z *= _widthScale;
                    return cos(length(position.yz - float2(0, _slitSpacing / 2)) * _waveNumber - _Time.y * _angularVelocity) + cos(length(position.yz + float2(0, _slitSpacing / 2)) * _waveNumber - _Time.y * _angularVelocity);
                }

                float GaussianPlaneWave(float3 position) {
                    position /= _Scale;
                    if (!checkInnerBoundingBox(position)) { return 0; }

                    //return PlaneWave(position) * exp(-_gaussianCoefficient*(position.x * position.x)) * exp(-_gaussianCoefficient*(position.z * position.z)) * (position.y + 1.2);
                    return PlaneWave(position) * exp(-_gaussianCoefficient * (position.x * position.x) / (position.y * _spreadCoefficient + 1 + _spreadStart)) * exp(-_gaussianCoefficient * (position.z * position.z) / (position.y * _spreadCoefficient + 1 + _spreadStart)) * 0.7;
                }

                float GaussianInterferenceWave(float3 position) {
                    position /= _Scale;
                    if (!checkInnerBoundingBox(position)) { return 0; }

                    position.y -= _slitStart *= _Scale;

                    return InterferenceWave(position) * exp(-_gaussianCoefficient * (position.x * position.x)) * _intensityScale;
                }

                //Transfer functions
                float4 WaveFunctionTransferFunction(float f) {
                    float probablityDensity = f * f;

                    if (probablityDensity > _ProbabiltyDensityCutoffR) {
                        return float4(1,0,0, probablityDensity * probablityDensity);
                    }
                    else if (probablityDensity > _ProbabiltyDensityCutoffG) {
                        return float4(0,0.5,0, probablityDensity * probablityDensity);
                        }
                    else if (probablityDensity > _ProbabiltyDensityCutoffB) {
                        return float4(0,0,0.2, probablityDensity * probablityDensity);
                        }
                    else if (probablityDensity > 0) {
                        return float4(_Outline.xxxx);
                        }

                    return float4(0,0,0,0);
                }

                //Blending functions
                float3 blendColors(float3 color1, float3 color2, float alpha1) {
                    return color1 + color2 * (1 - alpha1);
                }

                float blendAlphas(float1 alpha1, float2 alpha2) {
                    return alpha1 + alpha2 * (1 - alpha1);
                }

                //Fragment shader
                float4 frag(Interpolators i) : SV_Target {
                    float3 rayOrigin = i.position;
                    float3 samplePosition = rayOrigin;

                    float4 currentColor = float4(0, 0, 0, 0);
                    float4 sampledColor;
                    float stepSizeColorCorrection = _StepSize * 10;

                    //Raymarch through the sphere until it leaves the outer bounding box
                    while (checkBoundingBox(samplePosition)) {
                        if (samplePosition.y < _slitStart) {
                            sampledColor = WaveFunctionTransferFunction(GaussianPlaneWave(samplePosition)) * stepSizeColorCorrection;
                        }
                        else {
                            sampledColor = WaveFunctionTransferFunction(GaussianInterferenceWave(samplePosition)) * stepSizeColorCorrection;
                        }

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