Shader "Clawbear Games/Self Illumin Diffuse Fade" 
{
	Properties 
	{
		_Color ("Tint Color", Color) = (1,1,1,1)
		_MainTex ("Texture (RGB)", 2D) = "white" {}
		_StartY("Fade Start", Float) = -1
		_EndY("Fade End", Float) = -10
	}
	SubShader 
	{
		Tags { "RenderType" = "Transparent" "Queue" = "AlphaTest" "IgnoreProjector" = "True"}
		LOD 200

		Pass
		{
			ZWrite On
			ColorMask 0
		}

		CGPROGRAM
		#pragma surface surf Lambert alpha

		sampler2D _MainTex;
		fixed4 _Color;
		float  _StartY;
		float  _EndY;

		struct Input {
			float2 uv_MainTex;
			float3 worldPos;
		};

		void surf (Input IN, inout SurfaceOutput o) 
		{
			fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
			fixed4 c = tex * _Color;
			o.Albedo = c.rgb;
			o.Emission = c.rgb;
			float alphaBlend = clamp((IN.worldPos.y - _EndY) / (_StartY - _EndY), 0, 1);
			o.Alpha = lerp(0, 1, alphaBlend);
		}
		ENDCG
	}
	FallBack "Mobile/Diffuse"
}