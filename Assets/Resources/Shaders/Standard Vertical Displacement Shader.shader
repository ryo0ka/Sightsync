// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "Standard Vertical Displacement Shader" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_BumpMap("Normal Map", 2D) = "bump" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
        _EmissionColor ("Emission Color", Color) = (0,0,0)
        _Cutout ("Cutout Value", Float) = 0.5
        _Displacement ("Displacement", Float) = 0
        //[KeywordEnum(_ALPHATEST_ON, _ALPHABLEND_ON, _ALPHAPREMULTIPLY_ON)]
        //_RenderMode("Render Mode", Float) = 0
        _Mode("Mode", Float) = 0
        _SrcBlend("SrcBlend", Float) = 0
        _DstBlend("DstBlend", Float) = 0
        _ZWrite("ZWrite", Float) = 0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM

		sampler2D _MainTex;
        sampler2D _BumpMap;

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;
        fixed3 _EmissionColor;
        fixed _Displacement;
        
		#pragma surface surf Standard fullforwardshadows vertex:vert// alphatest:_Cutout

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		struct Input {
			float2 uv_MainTex;
            float2 uv_BumpMap;
		};

		void surf (Input IN, inout SurfaceOutputStandard o) {
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
            o.Normal = UnpackNormal (tex2D (_BumpMap, IN.uv_BumpMap));
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
            o.Emission = _EmissionColor;
		}

        //http://answers.unity3d.com/questions/561978/modifying-vertex-position-in-a-surface-shader.html
        void vert (inout appdata_full v) {
            float4 wvv = mul(unity_ObjectToWorld, v.vertex);
            wvv.y += _Displacement;
            v.vertex = mul(unity_WorldToObject, wvv);
        }

		ENDCG
	}

	FallBack "Diffuse"
}
