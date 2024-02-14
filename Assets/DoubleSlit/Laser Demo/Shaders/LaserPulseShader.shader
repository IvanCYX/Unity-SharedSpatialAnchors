Shader "Unlit/LaserPulse" {
    Properties{

    }
    SubShader
    {
        Tags {
            "Queue" = "Transparent"
            "RenderType" = "Transparent"
        }

        Blend One OneMinusSrcAlpha
        LOD 100

        Pass {
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            #define MAX_STEP_COUNT 256
            #define EPSILON 0.00001f

            struct MeshData {
                float4 vertex : POSITION;
            };

            struct Interpolators {
                float4 vertex : SV_POSITION;
                float3 objectVertex : TEXCOORD0;
                float3 vectorToSurface : TEXCOORD1;
            };

            //Vertex shader
            Interpolators vert(MeshData v) {
                Interpolators o;

                // Vertex in object space this will be the starting point of raymarching
                o.objectVertex = v.vertex;

                // Calculate vector from camera to vertex in world space
                float3 worldVertex = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.vectorToSurface = worldVertex - _WorldSpaceCameraPos;

                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }

            /*float4 BlendUnder(float4 color, float4 newColor) {
                color.rgb *= (1.0 - color.a) + newColor.a * newColor.rgb;
                color.a += newColor.a;
                return color;
            }*/

            float InverseLerp(float a, float b, float v) {
                return (v - a) / (b - a);
            }

            float Remap(float iMin, float iMax, float oMin, float oMax, float v) {
                float t = InverseLerp(iMin, iMax, v);
                return lerp(oMin, oMax, t);
            }

            float4 colorFunction(float3 position) {
                float x = position.x; float y = position.y; float z = position.z;
                float t = _Time[1];

                float distanceToCenter = length(position);
                float color;
                float k = 15;
                float w = 3.14;
                float value = .005*sin(k*x+w*t)*exp(-15*(z*z+y*y));
                value = 500*value * value;
               

                float cubeSize = 1;
                if (abs(x) < cubeSize/2 && abs(y) < cubeSize/2 && abs(z) < cubeSize/2) {
                    /*if (value >= 0) {
                        color = float4(value, 0, 0, value);
                    }
                    else {
                        color = float4(0, value, value, value);
                    }*/

                    //color = float4(value, 0, 0, value);
                    color = float4(value, -value, -value, value);
                }
                else {
                    color = float4(0, 0, 0, 0);
                }

                return color;
            }

            //Fragment shader
            fixed4 frag(Interpolators i) : SV_Target {

                float3 rayOrigin = i.objectVertex;
                //float3 rayDirection = normalize(i.vectorToSurface);
                float3 rayDirection = normalize(mul(unity_WorldToObject, float4(normalize(i.vectorToSurface), 0)));

                float4 color = float4(0, 0, 0, 0);
                float3 samplePosition = rayOrigin;

                float stepSize = 1.0 / float(MAX_STEP_COUNT);
                float accumulationDistance = 0;
                float3 finalColor = float3(0,0,0);

                for (int i = 0; i < MAX_STEP_COUNT; i++) {
                        float4 sampledColor = colorFunction(samplePosition);
                        accumulationDistance += abs(sampledColor[3]) * stepSize;
                        finalColor.xyz += sampledColor.xyz;
                        samplePosition += rayDirection * stepSize;
                }

                return float4(finalColor.x, finalColor.y, finalColor.z, 20*accumulationDistance);
            }

            ENDCG
        }
    }
}