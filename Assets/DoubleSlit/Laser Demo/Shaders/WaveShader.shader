Shader "Unlit/SmokeShader" {
    Properties {
        _Voltage ("Voltage", Range(0,1)) = 0.1
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
                #pragma fragment frag keepalpha

                #include "UnityCG.cginc"

                #define MAX_STEP_COUNT 120
                #define EPSILON 0.00001f

                float _Voltage;

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

                float4 BlendUnder(float4 color, float4 newColor) {
                    color.rgb *= (1.0 - color.a) + newColor.a * newColor.rgb;
                    color.a += newColor.a;
                    return color;
                }

                float InverseLerp(float a, float b, float v) {
                    return (v-a)/(b-a);
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
        
                    if(distanceToCenter < 0.4) {
                        color = float4(1-x*2,0,0,0);
                    } else if(distanceToCenter < 0.6) {
                        color = float4(Remap(0.4,0.7,0.1,0.9, distanceToCenter),0,0,0);
                    } else {
                        color = float4(0,0,0,0);
                    } 

                    return color; 
                }

                float4 electricFieldFunction(float3 position) {
                    float x = position.x; float y = position.y; float z = position.z;
                    float t = _Time[1];

                    float distanceToCenter = length(position);
                    float color;

                    float electricFieldStrength = _Voltage/distanceToCenter;
                    if(distanceToCenter > 0.8) {electricFieldStrength = 0;}

                    color = float4(electricFieldStrength,0,0,0);

                    return color;
                }

                float4 electricFieldsFunction(float3 position) {
                    float x = position.x; float y = position.y; float z = position.z;
                    float t = _Time[1];

                    float2x2 rotation = float2x2(cos(t/1.5), -sin(t/1.5), sin(t/1.5), cos(t/4));
                    float3 pointCharge1Position = float3(mul(rotation, float2(0.3 * cos(t/2),0)),0);
                    float3 pointCharge2Position = float3(mul(rotation, float2(-0.3 * cos(t/2),0)),0);

                    float distanceToCenter1 = length(position - pointCharge1Position);
                    float distanceToCenter2 = length(position - pointCharge2Position);
                    float color;

                    float electricFieldStrength = (_Voltage/distanceToCenter1 + _Voltage/distanceToCenter2) * 0.8;

                    color = float4(electricFieldStrength,0,0,0);

                    return color;
                }

                float4 squareFieldFunction(float3 position) {
                    float x = position.x; float y = position.y; float z = position.z;
                    float t = _Time[1];

                    float distanceToCenter = length(position);
                    float color;

                    float squareFieldStrength = min(pow(1.1, distanceToCenter + 0.5),0.95);
                    color = float4(0,0,0,squareFieldStrength);

                    return color;
                }

                //Fragment shader
                fixed4 frag(Interpolators i) : SV_Target {
                    
                    float3 rayOrigin = i.objectVertex;
                    float3 rayDirection = normalize(mul(unity_WorldToObject, float4(normalize(i.vectorToSurface), 0)));   

                    float4 color = float4(0, 0, 0, 0);
                    float3 samplePosition = rayOrigin;

                    float stepSize = 1.0 / float(MAX_STEP_COUNT);
                    float accumulationDistance = 0;

                    for(int i = 0; i < MAX_STEP_COUNT; i++) {
                            float4 sampledColor = electricFieldsFunction(samplePosition);
                            accumulationDistance += sampledColor[0] * stepSize;
                            samplePosition += rayDirection * stepSize;

                            if(length(samplePosition) > 0.5 + EPSILON) {break;}
                    }

                    if(accumulationDistance < 0.01) {
                        discard;
                        return float4(0,0,0,0);
                    } else {
                        return float4(lerp(float3(0,0,1), float3(1,0,0), accumulationDistance),accumulationDistance);
                    }
                }

                ENDCG
            }
        }
}