Shader "Custom/Distorted/Multiply"
{
Properties
{
    _MainTex ("Base (RGB)", 2D) = "white" {}
}
SubShader
{
    Tags
{
    "IGNOREPROJECTOR" = "true"
    "QUEUE" = "Transparent"
    "RenderType" = "Transparent"
}
Pass // ind: 1, name: 
{
Tags
{
    "IGNOREPROJECTOR" = "true"
    "QUEUE" = "Transparent"
    "RenderType" = "Transparent"
}
Fog
{
Mode  Off
}
Blend DstColor Zero
// m_ProgramMask = 6
CGPROGRAM
//#pragma target 4.0

#pragma vertex vert
#pragma fragment frag

#include "UnityCG.cginc"


#define CODE_BLOCK_VERTEX
//uniform float4x4 unity_ObjectToWorld;
//uniform float4x4 unity_MatrixVP;
uniform float4 _MainTex_ST;
uniform float4 _Distort;
uniform sampler2D _MainTex;
struct appdata_t
{
    float4 vertex :POSITION0;
    float4 texcoord :TEXCOORD0;
};

struct OUT_Data_Vert
{
    float2 texcoord :TEXCOORD0;
    float texcoord1 :TEXCOORD1;
    float4 vertex :SV_POSITION;
};

struct v2f
{
    float2 texcoord :TEXCOORD0;
    float texcoord1 :TEXCOORD1;
};

struct OUT_Data_Frag
{
    float4 color :SV_Target0;
};

float4 u_xlat0;
float4 u_xlat1;
OUT_Data_Vert vert(appdata_t in_v)
{
    OUT_Data_Vert out_v;
    u_xlat0 = UnityObjectToClipPos(in_v.vertex);
    u_xlat1.x = (u_xlat0.z * u_xlat0.z);
    out_v.vertex.xy = ((u_xlat1.xx * _Distort.xy) + u_xlat0.xy);
    out_v.vertex.zw = u_xlat0.zw;
    u_xlat0.x = ((-u_xlat0.w) + 700);
    u_xlat0.x = (u_xlat0.x * 0.00200000009);
    u_xlat0.x = clamp(u_xlat0.x, 0, 1);
    out_v.texcoord1 = ((-u_xlat0.x) + 1);
    out_v.texcoord.xy = TRANSFORM_TEX(in_v.texcoord.xy, _MainTex);
    return out_v;
}

    #define CODE_BLOCK_FRAGMENT
float4 u_xlat0_d;
OUT_Data_Frag frag(v2f in_f)
{
    OUT_Data_Frag out_f;
    u_xlat0_d = tex2D(_MainTex, in_f.texcoord.xy);
    float _tmp_dvx_0 = in_f.texcoord1;
    u_xlat0_d.xyz = (u_xlat0_d.xyz + float3(_tmp_dvx_0, _tmp_dvx_0, _tmp_dvx_0));
    out_f.color = u_xlat0_d;
    return out_f;
}


ENDCG

} // end phase
}
FallBack "Diffuse"
}