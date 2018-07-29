Shader "Custom/Miniture Shader" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Emission ("Emission", Color) = (1,1,1,1)
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM

        #pragma surface surf Lambert

		fixed4 _Color;
		sampler2D _MainTex;
        fixed4 _Emission;

		struct Input {
			float2 uv_MainTex;
		};

        void surf(Input IN, inout SurfaceOutput o) {
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;

            o.Emission = _Emission;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
