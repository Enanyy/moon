Shader "X1/worldline_shader" {
	Properties 
    {
		//_MainTex ("Albedo (RGB)", 2D) = "white" {}
	}
	SubShader 
    {
        Tags 
        {
            "QUEUE" = "Geometry" 
            "RenderType" = "Opaque"
        }
        //ZWrite off
        //Cull Off
        Pass
        {
            //Tags { "LightMode" = "ForwardBase" }
            CGPROGRAM
            #include "UnityCG.cginc"
            #pragma target 3.0
            #pragma vertex vert
            #pragma fragment frag
            #pragma exclude_renderers xbox360 flash	
            #pragma multi_compile _ ENABLE_PARABOLOID

            sampler2D _MainTex;
            struct appdata
            {
                half4 vertex : POSITION;
                half4 color : COLOR;
            };

            struct VSOut
            {
                half4 pos		: SV_POSITION;
                half4 color     : COLOR;
            };

            VSOut vert(appdata v)
            {
                VSOut o;								
                o.color = v.color;
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            half4 frag(VSOut i) : COLOR
            {			
                half4 DF = i.color; // *half4((half3)1.0, tex2D(_DiffuseAlpha,  i.uv).r);
                return DF;
            }
            ENDCG
        }
    }
}
