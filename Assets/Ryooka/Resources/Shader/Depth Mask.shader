Shader "Depth Mask/Depth Mask" {
    Properties {
        _Color ("Main Color", Color) = (1,1,1,1)
        _ColorMask ("Color Mask", Float) = 15
    }

    SubShader {
    	Tags { "Queue" = "Geometry-10" }
        
        ZWrite On

        ColorMask [_ColorMask]

        Pass{ }

        CGPROGRAM
    
        #pragma surface surf Lambert

        fixed4 _Color;
        
        struct Input {
            float2 uv_MainTex;
        };

        void surf (Input IN, inout SurfaceOutput o) {
            o.Albedo = _Color.rgb;
        }
    
        ENDCG
    }

	Fallback "Diffuse"
}
