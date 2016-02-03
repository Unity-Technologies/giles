// Shader created with Shader Forge v1.13 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.13;sub:START;pass:START;ps:flbk:,lico:1,lgpr:1,nrmq:1,nrsp:0,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,rprd:False,enco:False,rmgx:True,rpth:0,hqsc:True,hqlp:False,tesm:0,bsrc:0,bdst:1,culm:0,dpts:6,wrdp:True,dith:6,ufog:False,aust:False,igpj:False,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,ofsf:0,ofsu:0,f2p0:False;n:type:ShaderForge.SFN_Final,id:9034,x:33948,y:32828,varname:node_9034,prsc:2|emission-1951-OUT,alpha-5847-A;n:type:ShaderForge.SFN_VertexColor,id:5847,x:32543,y:32634,varname:node_5847,prsc:2;n:type:ShaderForge.SFN_ViewVector,id:7160,x:32860,y:33014,varname:node_7160,prsc:2;n:type:ShaderForge.SFN_NormalVector,id:1943,x:32860,y:32828,prsc:2,pt:False;n:type:ShaderForge.SFN_Dot,id:2696,x:33116,y:32946,varname:node_2696,prsc:2,dt:4|A-1943-OUT,B-7160-OUT;n:type:ShaderForge.SFN_Multiply,id:777,x:33618,y:32951,varname:node_777,prsc:2|A-5847-RGB,B-48-OUT;n:type:ShaderForge.SFN_RemapRangeAdvanced,id:48,x:33399,y:33073,varname:node_48,prsc:2|IN-2696-OUT,IMIN-8798-OUT,IMAX-7556-OUT,OMIN-3218-OUT,OMAX-4119-OUT;n:type:ShaderForge.SFN_Vector1,id:7556,x:32836,y:33337,varname:node_7556,prsc:2,v1:1;n:type:ShaderForge.SFN_Subtract,id:4119,x:33054,y:33243,varname:node_4119,prsc:2|A-7556-OUT,B-3226-OUT;n:type:ShaderForge.SFN_Subtract,id:3218,x:33054,y:33384,varname:node_3218,prsc:2|A-7556-OUT,B-799-OUT;n:type:ShaderForge.SFN_Vector1,id:8798,x:32836,y:33267,varname:node_8798,prsc:2,v1:0;n:type:ShaderForge.SFN_Slider,id:3226,x:32531,y:33192,ptovrint:False,ptlb:Fresnel Min,ptin:_FresnelMin,varname:node_9121,prsc:2,min:0,cur:0,max:0.5;n:type:ShaderForge.SFN_Slider,id:799,x:32531,y:33428,ptovrint:False,ptlb:Fresnel Max,ptin:_FresnelMax,varname:_node_9121_copy,prsc:2,min:0.5,cur:0.5,max:2;n:type:ShaderForge.SFN_If,id:1951,x:33639,y:32758,varname:node_1951,prsc:2|A-5847-A,B-8298-OUT,GT-777-OUT,EQ-777-OUT,LT-5847-RGB;n:type:ShaderForge.SFN_Vector1,id:8298,x:33378,y:32678,varname:node_8298,prsc:2,v1:0.8;proporder:3226-799;pass:END;sub:END;*/

Shader "Custom/HandleUnlitFresnel" {
    Properties {
        _FresnelMin ("Fresnel Min", Range(0, 0.5)) = 0
        _FresnelMax ("Fresnel Max", Range(0.5, 2)) = 0.5
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
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
            uniform float _FresnelMin;
            uniform float _FresnelMax;
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
                o.posWorld = mul(_Object2World, v.vertex);
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
/////// Vectors:
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
////// Lighting:
////// Emissive:
                float node_1951_if_leA = step(i.vertexColor.a,0.8);
                float node_1951_if_leB = step(0.8,i.vertexColor.a);
                float node_8798 = 0.0;
                float node_7556 = 1.0;
                float node_3218 = (node_7556-_FresnelMax);
                float3 node_777 = (i.vertexColor.rgb*(node_3218 + ( (0.5*dot(i.normalDir,viewDirection)+0.5 - node_8798) * ((node_7556-_FresnelMin) - node_3218) ) / (node_7556 - node_8798)));
                float3 emissive = lerp((node_1951_if_leA*i.vertexColor.rgb)+(node_1951_if_leB*node_777),node_777,node_1951_if_leA*node_1951_if_leB);
                float3 finalColor = emissive;
                return fixed4(finalColor,i.vertexColor.a);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
