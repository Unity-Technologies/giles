// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Custom/Highlight" 
{
	Properties
	{
		_Scale ("Scale", Range(0,1)) = .1
		_Color ("Color Tint", Color) = (1,1,1,1)
		[HideInInspector] _Center ("Center", Vector) = (0,0,0,1)
	}

	SubShader
	{
		Tags { "IgnoreProjector"="True" "RenderType"="Geometry" }
		Lighting Off
		ZTest LEqual
		ZWrite On
		Cull Front
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			AlphaTest Greater .25

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			float _Scale;
			float4 _Color;
			float4 _Center;

			struct appdata
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				float4 world : TEXCOORD0;
				float3 normal : TEXCOORD1;
			};

			v2f vert (appdata v)
			{
				v2f o;

				float4 vert = v.vertex;
				vert.xyz += normalize(vert.xyz - _Center) * _Scale;
				o.pos = mul(UNITY_MATRIX_MV, vert);
				o.pos.xyz *= 1.01;
				o.pos = mul(UNITY_MATRIX_P, o.pos);

				o.normal = UnityObjectToWorldNormal(v.normal);
                o.world = mul(unity_ObjectToWorld, v.vertex);

				return o;
			}

			half4 frag (v2f i) : COLOR
			{
				return _Color;// * fresnel;
			}

			ENDCG
		}
	}
}