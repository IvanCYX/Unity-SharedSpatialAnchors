Shader "Unlit/LaserShader" {
    Properties {
        //_Transparency ("Transparency", Range(0,1)) = 0.1
        //_Color ("Color", Color) = (1,0,0,1)
        _brightness ("Brightness", Range(0,1)) = 0.5
        _dimColor ("Dim Color", Color) = (0.0, 0.0, 0.0, 1.0)
        _brightColor ("Bright Color", Color) = (1.0, 0.0, 0.0, 1.0)
    }

    SubShader {
        Tags { 
            //"RenderType"="Transparent"
            //"Queue"="Transparent" 

            "RenderType"="Opaque"
        }

        Pass {
            //Cull Off
            //ZWrite Off
            //Blend SrcAlpha OneMinusSrcAlpha
            //Blend SrcColor One

            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            //half _Transparency;
            float4 _brightColor;
            float4 _dimColor;
            float _brightness;

            struct MeshData {
                float4 vertex : POSITION;
                float2 uv0 : TEXCOORD0;
            };

            struct Interpolators {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            Interpolators vert(MeshData v) {
                Interpolators o;
                o.uv = v.uv0;
                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }

            float4 frag(Interpolators i) : SV_Target {
                return lerp(_dimColor, _brightColor, _brightness);
                //return lerp(_dimColor, _brightColor, i.uv.y);
            }

            ENDCG
        }
    }
}
