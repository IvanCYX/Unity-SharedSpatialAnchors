Shader "Unlit/LensShader" {
    Properties {

    }

    SubShader {
        Tags { 
            "RenderType"="Transparent"
            "Queue"="Transparent" 
        }

        Pass {
            Cull Front
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
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            Interpolators vert (MeshData v) {
                Interpolators o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }

            float4 frag (Interpolators i) : SV_Target {
                return float4(1,1,1,0.4);
            }

            ENDCG
        }
    }
}
