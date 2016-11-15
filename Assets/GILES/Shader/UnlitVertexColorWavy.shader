Shader "Custom/UnlitVertexColorWavy"
{
	Properties
	{
		_Freq("Frequency", float) = .3
		_Amp("Amplitude", float) = .3
	}

	SubShader
	{
		Tags { "Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="Transparent" }
		Lighting Off
		ZTest LEqual
		Blend SrcAlpha OneMinusSrcAlpha
		ZWrite On
		Cull Off

		Pass 
		{
			AlphaTest Greater .25

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			float4 _Color;
			float _Scale;
			float _Freq;
			float _Amp;

			struct appdata
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float4 color : COLOR;
				float4 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				float4 color : COLOR;
			};

			v2f vert (appdata v)
			{
				v2f o;

				o.pos = mul(UNITY_MATRIX_MV, v.vertex);

				float rad = (((frac(_Time.y * _Freq + v.texcoord.y) - .5) * 2) * 3.14);// * smoothstep(0, .2, v.texcoord.x);
				o.pos += fixed4(0, sin(rad) * _Amp * smoothstep(0,.3, v.texcoord.x), 0, 0);
				o.pos = mul(UNITY_MATRIX_P, o.pos);

				o.color = v.color;

				return o;
			}

			half4 frag (v2f i) : COLOR
			{
				return i.color;
			}

			ENDCG
		}
	}
}