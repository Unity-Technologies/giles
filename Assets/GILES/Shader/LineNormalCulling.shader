// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Custom/Cull Lines by Normal"
{
	Properties
	{
		_Min ("Cutoff", Range(0,.5)) = .2
	}

	SubShader
	{
		Tags { "Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="Transparent" }
		Lighting Off
		ZTest Always
		Blend SrcAlpha OneMinusSrcAlpha
		ZWrite On

		Pass 
		{
			AlphaTest Greater .25

			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			float _Min;

			struct appdata
			{
				float4 vertex : POSITION;
				float4 color : COLOR;
				float3 normal : NORMAL;
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				float4 world : TEXCOORD0;
				float3 normal : TEXCOORD1;
				float4 color : COLOR;
			};

			v2f vert (appdata v)
			{
				v2f o;

				o.pos = UnityObjectToClipPos(v.vertex);
				o.world = mul(unity_ObjectToWorld, v.vertex);
				o.normal = UnityObjectToWorldNormal(v.normal);
				o.color = v.color;

				return o;
			}

			half4 frag (v2f i) : COLOR
			{
				float3 viewDir = normalize(_WorldSpaceCameraPos.xyz - i.world.xyz);
				float d = dot(i.normal, viewDir) * .5 + .5;
				float4 col = i.color;
				col.a = d < _Min ? 0 : d;
				return col;
			}

			ENDCG
		}
	}
}