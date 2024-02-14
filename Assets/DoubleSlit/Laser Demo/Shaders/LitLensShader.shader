Shader "Custom/LitLensShader" {
    Properties {
        _Transparency ("Transparency", Range(0,1)) = 0.5

        _IndexOfRefraction ("Index of Refraction", Range(1.1,5.0)) = 1.5
        _RadiusOfCurvature ("Radius of Curvature", Range(-25.0,25.0)) = 0.5
    }
    
    SubShader {
        Tags { 
            "RenderType"="Transparent"
            "Queue"="Transparent" 
        }

        Cull Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        CGPROGRAM

        #pragma surface surf Standard fullforwardshadows alpha
        #pragma target 3.0

        struct Input {
            float2 uv_MainTex;
        };

        float inverseLerp(float a, float b, float v) {
            return (v-a)/(b-a);
        }

        half _Transparency;
        float _IndexOfRefraction;
        float _Tint;

        void surf (Input IN, inout SurfaceOutputStandard o) {
            _Tint = lerp(float3(1,1,1), float3(0,0,1), inverseLerp(1.1,5,_IndexOfRefraction)/2);
            o.Albedo = float3(_Tint,_Tint,1);
            //o.Albedo = float3(1,1,1);
            o.Alpha = 1-_Transparency;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
