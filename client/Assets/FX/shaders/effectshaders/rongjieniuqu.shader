// Shader created with Shader Forge v1.38 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:1,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5147059,fgcg:0.6184583,fgcb:1,fgca:1,fgde:1,fgrn:0,fgrf:180,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:True,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:7886,x:33532,y:32554,varname:node_7886,prsc:2|emission-5403-OUT,alpha-6981-OUT,refract-3383-OUT;n:type:ShaderForge.SFN_TexCoord,id:1034,x:31604,y:32644,varname:node_1034,prsc:2,uv:1,uaff:True;n:type:ShaderForge.SFN_Step,id:7915,x:32310,y:32945,varname:node_7915,prsc:2|A-1034-U,B-4332-R;n:type:ShaderForge.SFN_Tex2d,id:4332,x:32019,y:33070,ptovrint:False,ptlb:opa_tex,ptin:_opa_tex,varname:node_4332,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:5256c0f985816994082dd09e5ceba31e,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:4469,x:32042,y:32474,ptovrint:False,ptlb:diffuse,ptin:_diffuse,varname:node_4469,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:0b999d3471bc4934eaba8d78fc5570da,ntxv:0,isnm:False|UVIN-9291-OUT;n:type:ShaderForge.SFN_Multiply,id:5403,x:32754,y:32647,varname:node_5403,prsc:2|A-4469-RGB,B-257-RGB,C-602-RGB;n:type:ShaderForge.SFN_Color,id:257,x:32042,y:32652,ptovrint:False,ptlb:color,ptin:_color,varname:node_257,prsc:2,glob:False,taghide:False,taghdr:True,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Multiply,id:6981,x:32754,y:32850,varname:node_6981,prsc:2|A-4469-A,B-602-A,C-7915-OUT;n:type:ShaderForge.SFN_VertexColor,id:602,x:32020,y:32772,varname:node_602,prsc:2;n:type:ShaderForge.SFN_Add,id:9291,x:31842,y:32462,varname:node_9291,prsc:2|A-1775-UVOUT,B-9226-OUT;n:type:ShaderForge.SFN_TexCoord,id:1775,x:31604,y:32327,varname:node_1775,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Append,id:9226,x:31604,y:32506,varname:node_9226,prsc:2|A-1034-Z,B-1034-W;n:type:ShaderForge.SFN_TexCoord,id:4564,x:32542,y:33150,varname:node_4564,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Append,id:5188,x:32823,y:33164,varname:node_5188,prsc:2|A-4564-U,B-4564-V;n:type:ShaderForge.SFN_Tex2d,id:2856,x:32557,y:32987,ptovrint:False,ptlb:Ref,ptin:_Ref,varname:node_2856,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:4496,x:33101,y:33037,varname:node_4496,prsc:2|A-2856-R,B-5188-OUT;n:type:ShaderForge.SFN_Multiply,id:3383,x:33339,y:33013,varname:node_3383,prsc:2|A-4496-OUT,B-1034-V,C-4469-R;proporder:4332-4469-257-2856;pass:END;sub:END;*/

Shader "Effect/rongjieniuqu" {
    Properties {
        _opa_tex ("opa_tex", 2D) = "white" {}
        _diffuse ("diffuse", 2D) = "white" {}
        [HDR]_color ("color", Color) = (0.5,0.5,0.5,1)
        _Ref ("Ref", 2D) = "white" {}
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        LOD 100
        GrabPass{ }
        Pass {
            Name "FORWARD"
            //Tags {
            //    "LightMode"="ForwardBase"
            //}
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            //#define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            //#pragma multi_compile_fwdbase
            //#pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform sampler2D _GrabTexture;
            uniform sampler2D _opa_tex; uniform float4 _opa_tex_ST;
            uniform sampler2D _diffuse; uniform float4 _diffuse_ST;
            uniform float4 _color;
            uniform sampler2D _Ref; uniform float4 _Ref_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float4 texcoord1 : TEXCOORD1;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 uv1 : TEXCOORD1;
                float4 vertexColor : COLOR;
                float4 projPos : TEXCOORD2;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.vertexColor = v.vertexColor;
                o.pos = UnityObjectToClipPos( v.vertex );
                o.projPos = ComputeScreenPos (o.pos);
                COMPUTE_EYEDEPTH(o.projPos.z);
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                float4 _Ref_var = tex2D(_Ref,TRANSFORM_TEX(i.uv0, _Ref));
                float2 node_9291 = (i.uv0+float2(i.uv1.b,i.uv1.a));
                float4 _diffuse_var = tex2D(_diffuse,TRANSFORM_TEX(node_9291, _diffuse));
                float2 sceneUVs = (i.projPos.xy / i.projPos.w) + ((_Ref_var.r*float2(i.uv0.r,i.uv0.g))*i.uv1.g*_diffuse_var.r);
                float4 sceneColor = tex2D(_GrabTexture, sceneUVs);
////// Lighting:
////// Emissive:
                float3 emissive = (_diffuse_var.rgb*_color.rgb*i.vertexColor.rgb);
                float3 finalColor = emissive;
                float4 _opa_tex_var = tex2D(_opa_tex,TRANSFORM_TEX(i.uv0, _opa_tex));
                return fixed4(lerp(sceneColor.rgb, finalColor,(_diffuse_var.a*i.vertexColor.a*step(i.uv1.r,_opa_tex_var.r))),1);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
