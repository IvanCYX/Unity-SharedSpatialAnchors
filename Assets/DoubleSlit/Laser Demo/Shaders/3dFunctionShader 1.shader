Shader "Unlit/3DTextureShader" {
    Properties {

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

                #define MAX_STEP_COUNT 128
                #define EPSILON 0.00001f
                #define STEP_SIZE 0.01

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
                    color.rgb += (1.0 - color.a) * newColor.a * newColor.rgb;
                    color.a += (1.0 - color.a) * newColor.a;
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
                    /*
                    float k = 20;
                    float w = 2;

                    float f = ((sin(k*x + w*t) + sin(k*y + w*t)) * 0.25 + 0.5) * pow(2.1, -2 * z * z);
                    */

                    //float color = float4((position.x + 0.5)/20.0,(position.y + 0.5)/17.0,0,0.01);
                    float color = float4((position.x + 0.5)*frac(t/8.0),0.2,0.4,0.1);

                    return color; 
                }

                //Fragment shader
                fixed4 frag(Interpolators i) : SV_Target {
                    // Start raymarching at the front surface of the object
                    float3 rayOrigin = i.objectVertex;

                    // Use vector from camera to object surface to get ray direction
                    float3 rayDirection = normalize(mul(unity_WorldToObject, float4(normalize(i.vectorToSurface), 1))); 

                    float4 color = float4(0, 0, 0, 0);
                    float3 samplePosition = rayOrigin;

                    // Raymarch through object space
                    for (int i = 0; i < MAX_STEP_COUNT; i++) {
                        // Accumulate color only within unit sphere bounds in object space
                        if (sqrt(length(samplePosition - rayOrigin)) < 1.0f + EPSILON) {
                            float4 sampledColor = colorFunction(samplePosition);
                            if(sampledColor.w > EPSILON) {color = BlendUnder(color, sampledColor);}
                            samplePosition += rayDirection * STEP_SIZE;
                        }
                    }

                    return color;
                }

                ENDCG
            }
        }
}