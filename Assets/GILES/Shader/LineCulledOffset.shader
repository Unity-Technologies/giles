// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Custom/Cull Lines by Normal and Offset"
{
	Properties
	{
	//	_Min ("Cutoff", Range(0,.5)) = .3
		_Color ("Color", Color) = (.1,.2,.7,1)
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

			const float _Min = .5;
			float4 _Color;

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

				o.pos = mul(UNITY_MATRIX_MV, v.vertex);
				o.pos *= .98;
				o.pos = mul(UNITY_MATRIX_P, o.pos);

				o.world = mul(unity_ObjectToWorld, v.vertex);
				o.normal = UnityObjectToWorldNormal(v.normal);
				o.color = v.color;

				return o;
			}

			half4 frag (v2f i) : COLOR
			{
				float3 viewDir = normalize(_WorldSpaceCameraPos.xyz - i.world.xyz);
				float d = dot(i.normal, viewDir) * .5 + .5;
				float4 col = _Color;
			//	col.a = (d < _Min ? 0 : d) * _Color.a;
				return col;
			}

			ENDCG
		}
	}
}
