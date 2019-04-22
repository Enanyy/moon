﻿Shader "Unlit/diban"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_BlendTex("Blend", 2D) = "white" {}

	    _Speed ("Speed",float) = 0
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;

			sampler2D _BlendTex;
			float4 _BlendTex_ST;

			float _Speed;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{				
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);
				float2 uv = i.uv;
				uv.y += _Time.y * _Speed;
				float4 blend = tex2D(_BlendTex, uv);
				col.rgb *= blend.rgb;
				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
			ENDCG
		}
	}
}
