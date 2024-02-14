Shader "Unlit/BoundingBoxShader" {
    Properties {
        _Color ("Color", Color) = (1,0,0,1)
    }
    SubShader {
        Tags {"RenderType"="Opaque"}

        Pass {
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct MeshData {
                float4 vertex : POSITION;
            };

            struct Interpolaters {
                float4 vertex : SV_POSITION;
            };

            float4 _Color;

            Interpolaters vert (MeshData v) {
                Interpolaters o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag (Interpolaters i) : SV_Target {
                return _Color;
            }
            
            ENDCG
        }
    }
}
