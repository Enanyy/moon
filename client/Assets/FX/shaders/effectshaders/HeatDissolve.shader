// Shader created with Shader Forge v1.38 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:2,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:9361,x:33620,y:32749,varname:node_9361,prsc:2|alpha-3851-OUT,refract-8254-OUT;n:type:ShaderForge.SFN_Tex2d,id:6489,x:32623,y:32877,ptovrint:False,ptlb:Main_Tex,ptin:_Main_Tex,varname:_Main_Tex,prsc:0,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Append,id:4000,x:32925,y:32934,varname:node_4000,prsc:2|A-6489-R,B-6489-G;n:type:ShaderForge.SFN_Multiply,id:8254,x:32925,y:33137,varname:node_8254,prsc:2|A-4000-OUT,B-4185-OUT,C-5009-RGB,D-5009-A;n:type:ShaderForge.SFN_Slider,id:4185,x:32493,y:33121,ptovrint:False,ptlb:Indensity,ptin:_Indensity,varname:_Indensity,prsc:0,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.7419199,max:1;n:type:ShaderForge.SFN_VertexColor,id:5009,x:32623,y:33255,varname:node_5009,prsc:2;n:type:ShaderForge.SFN_Vector1,id:3851,x:33242,y:32718,varname:node_3851,prsc:2,v1:0;proporder:6489-4185;pass:END;sub:END;*/

Shader "Effect/HeatDissolve" {
    Properties {
        _Main_Tex ("Main_Tex", 2D) = "white" {}
        _Indensity ("Indensity", Range(0, 1)) = 0.7419199
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        GrabPass{ }
        Pass {
            Name "FORWARD"
            //Tags {
            //    "LightMode"="ForwardBase"
            //}
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            //#pragma multi_compile_fwdbase
            //#pragma only_renderers d3d9 d3d11 glcore gles gles3 metal n3ds wiiu 
            #pragma target 3.0
            uniform sampler2D _GrabTexture;
            uniform sampler2D _Main_Tex; uniform float4 _Main_Tex_ST;
            uniform fixed _Indensity;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 vertexColor : COLOR;
                float4 projPos : TEXCOORD1;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.pos = UnityObjectToClipPos( v.vertex );
                o.projPos = ComputeScreenPos (o.pos);
                COMPUTE_EYEDEPTH(o.projPos.z);
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                fixed4 _Main_Tex_var = tex2D(_Main_Tex,TRANSFORM_TEX(i.uv0, _Main_Tex));
                float2 sceneUVs = (i.projPos.xy / i.projPos.w) + (float3(float2(_Main_Tex_var.r,_Main_Tex_var.g),0.0)*_Indensity*i.vertexColor.rgb*i.vertexColor.a).rg;
                float4 sceneColor = tex2D(_GrabTexture, sceneUVs);
////// Lighting:
                float3 finalColor = 0;
                return fixed4(lerp(sceneColor.rgb, finalColor,0.0),1);
            }
            ENDCG
        }
        
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
