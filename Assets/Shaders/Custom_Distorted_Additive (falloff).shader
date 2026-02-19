Shader "Custom/Distorted/Additive (falloff)"
{
Properties
{
    _MainTex ("Base (RGB)", 2D) = "white" {}
_MainColor ("Color (RGBC)", Color) = (1,1,1,0)
_Falloff ("Falloff Distance", float) = 200
}
SubShader
{
    Tags
{
    "QUEUE" = "Transparent"
}
Pass // ind: 1, name: 
{
Tags
{
    "QUEUE" = "Transparent"
}
ZWrite Off
Fog
{
    Mode  Off
}
Blend One One
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
uniform float _Falloff;
uniform float _Factor;
uniform float4 _MainColor;
uniform sampler2D _MainTex;
struct appdata_t
{
    float4 vertex :POSITION0;
    float4 texcoord :TEXCOORD0;
};

struct OUT_Data_Vert
{
    float2 texcoord :TEXCOORD0;
    float4 color :COLOR0;
    float4 vertex :SV_POSITION;
};

struct v2f
{
    float2 texcoord :TEXCOORD0;
    float4 color :COLOR0;
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
    u_xlat0.x = (u_xlat0.z / _Falloff);
    out_v.color.x = (((-u_xlat0.x) * u_xlat0.x) + 1);
    out_v.texcoord.xy = TRANSFORM_TEX(in_v.texcoord.xy, _MainTex);
    out_v.color.yzw = float3(0, 0, 0);
    return out_v;
}

    #define CODE_BLOCK_FRAGMENT
float4 u_xlat0_d;
float4 u_xlat10_0;
OUT_Data_Frag frag(v2f in_f)
{
    OUT_Data_Frag out_f;
    u_xlat10_0 = tex2D(_MainTex, in_f.texcoord.xy);
    u_xlat0_d = (u_xlat10_0 * _MainColor);
    u_xlat0_d = (u_xlat0_d * in_f.color.xxxx);
    u_xlat0_d = (u_xlat0_d * float4(_Factor, _Factor, _Factor, _Factor));
    out_f.color = u_xlat0_d;
    return out_f;
}


ENDCG

} // end phase
}
FallBack Off
}