// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Shader created with Shader Forge v1.26 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.26;sub:START;pass:START;ps:flbk:Unlit/Color,iptp:0,cusa:False,bamd:0,lico:0,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:6,wrdp:True,dith:6,rfrpo:False,rfrpn:Refraction,coma:15,ufog:False,aust:False,igpj:False,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False;n:type:ShaderForge.SFN_Final,id:9034,x:33948,y:32828,varname:node_9034,prsc:2|emission-9463-OUT;n:type:ShaderForge.SFN_VertexColor,id:5847,x:32860,y:32680,varname:node_5847,prsc:2;n:type:ShaderForge.SFN_ViewVector,id:7160,x:32860,y:33014,varname:node_7160,prsc:2;n:type:ShaderForge.SFN_NormalVector,id:1943,x:32860,y:32828,prsc:2,pt:False;n:type:ShaderForge.SFN_Dot,id:2696,x:33075,y:32926,varname:node_2696,prsc:2,dt:1|A-1943-OUT,B-7160-OUT;n:type:ShaderForge.SFN_Multiply,id:9463,x:33472,y:32832,varname:node_9463,prsc:2|A-5847-RGB,B-4849-OUT;n:type:ShaderForge.SFN_Clamp,id:4849,x:33279,y:32926,varname:node_4849,prsc:2|IN-2696-OUT,MIN-8704-OUT,MAX-3885-OUT;n:type:ShaderForge.SFN_Vector1,id:8704,x:33279,y:33078,varname:node_8704,prsc:2,v1:0.3;n:type:ShaderForge.SFN_Vector1,id:3885,x:33279,y:33133,varname:node_3885,prsc:2,v1:1;pass:END;sub:END;*/

Shader "Custom/HandleUnlitFresnel" {
    Properties {
    }
    SubShader {
        Tags {
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        LOD 200
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            ZTest Always
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma exclude_renderers xbox360 ps3 
            #pragma target 3.0
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 posWorld : TEXCOORD0;
                float3 normalDir : TEXCOORD1;
                float4 vertexColor : COLOR;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.vertexColor = v.vertexColor;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos(v.vertex );
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
////// Lighting:
////// Emissive:
                float3 emissive = (i.vertexColor.rgb*clamp(max(0,dot(i.normalDir,viewDirection)),0.3,1.0));
                float3 finalColor = emissive;
                return fixed4(finalColor,1);
            }
            ENDCG
        }
    }
    FallBack "Unlit/Color"
    CustomEditor "ShaderForgeMaterialInspector"
}
