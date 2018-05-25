// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Grid Line"
{
	Properties
	{
		_AlphaCutoff ("Fade Cutoff", float) = .1
		_AlphaFade ("Fade Start", float) = .3
		_Color ("Color", Color) = ( .5, .5, .5, .8 )
		_AlphaBump ("Interval Alpha Bump", float) = .2
	}

	SubShader
	{
		Tags { "Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="Transparent" }
		Lighting Off
		ZTest LEqual
		Blend SrcAlpha OneMinusSrcAlpha
		ZWrite Off

		Pass
		{
			AlphaTest Greater .25

			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			float _AlphaCutoff;
			float _AlphaFade;
			float _AlphaBump;
			float4 _Color;

			struct appdata
			{
				float4 vertex : POSITION;
				float4 uv : TEXCOORD0;
				float3 normal : NORMAL;
			};

			struct v2f
			{
				float4 pos : POSITION;
				float4 uv : TEXCOORD0;
				float3 world : TEXCOORD1;
				float3 normal : TEXCOORD2;
			};

			v2f vert (appdata v)
			{
				v2f o;

				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				o.world = mul(unity_ObjectToWorld, v.vertex);
				o.normal = UnityObjectToWorldNormal(v.normal);

				return o;
			}

			half4 frag (v2f i) : COLOR
			{
				float3 viewDir = normalize(_WorldSpaceCameraPos.xyz - i.world.xyz);
				float d = abs(dot(i.normal, viewDir));
				float alpha = smoothstep(_AlphaCutoff, _AlphaFade, d);
				float4 col = _Color;
				col.a += i.uv.x * _AlphaBump;
				col.a *= alpha;
				return col;
			}

			ENDCG
		}
	}
}
