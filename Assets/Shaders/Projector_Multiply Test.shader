Shader "Projector/Multiply Test"
{
Properties
{
    _Color ("Main Color", Color) = (1,1,1,1)
    _ShadowTex ("Cookie", 2D) = "gray" {}
_FalloffTex ("FallOff", 2D) = "white" {}
_ShadowStrength ("Strength", float) = 1
_Diffuse ("Diffuse", Color) = (1,1,1,1)
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
Offset -5, -1
Fog
{
    Mode  Off
}
Blend One One
ColorMask RGB
// m_ProgramMask = 6
CGPROGRAM
//#pragma target 4.0

#pragma vertex vert
#pragma fragment frag

#include "UnityCG.cginc"
#define conv_mxt4x4_0(mat4x4) float4(mat4x4[0].x,mat4x4[1].x,mat4x4[2].x,mat4x4[3].x)
#define conv_mxt4x4_1(mat4x4) float4(mat4x4[0].y,mat4x4[1].y,mat4x4[2].y,mat4x4[3].y)
#define conv_mxt4x4_2(mat4x4) float4(mat4x4[0].z,mat4x4[1].z,mat4x4[2].z,mat4x4[3].z)


#define CODE_BLOCK_VERTEX
//uniform float4x4 unity_ObjectToWorld;
//uniform float4x4 unity_WorldToObject;
//uniform float4x4 unity_MatrixVP;
uniform float4 _Color;
uniform float4 _Distort;
uniform float4 _Diffuse;
uniform float4x4 unity_Projector;
uniform float4x4 unity_ProjectorClip;
uniform float _ShadowStrength;
uniform float _Factor;
uniform sampler2D _ShadowTex;
uniform sampler2D _FalloffTex;
struct appdata_t
{
    float3 normal :NORMAL0;
    float4 vertex :POSITION0;
};

struct OUT_Data_Vert
{
    float4 texcoord :TEXCOORD0;
    float4 texcoord1 :TEXCOORD1;
    float3 color :COLOR0;
    float4 vertex :SV_POSITION;
};

struct v2f
{
    float4 texcoord :TEXCOORD0;
    float4 texcoord1 :TEXCOORD1;
    float3 color :COLOR0;
};

struct OUT_Data_Frag
{
    float4 color :SV_Target0;
};

float4 u_xlat0;
float4 u_xlat1;
float u_xlat16_2;
float3 u_xlat16_5;
float u_xlat9;
OUT_Data_Vert vert(appdata_t in_v)
{
    OUT_Data_Vert out_v;
    out_v.texcoord = mul(unity_Projector, in_v.vertex);
    out_v.texcoord1 = mul(unity_ProjectorClip, in_v.vertex);
    u_xlat0 = UnityObjectToClipPos(in_v.vertex);
    u_xlat1.x = (u_xlat0.z * u_xlat0.z);
    out_v.vertex.xy = ((u_xlat1.xx * _Distort.xy) + u_xlat0.xy);
    out_v.vertex.zw = u_xlat0.zw;
    u_xlat0.x = dot(in_v.normal.xyz, conv_mxt4x4_0(unity_WorldToObject).xyz);
u_xlat0.y = dot(in_v.normal.xyz, conv_mxt4x4_1(unity_WorldToObject).xyz);
u_xlat0.z = dot(in_v.normal.xyz, conv_mxt4x4_2(unity_WorldToObject).xyz);
u_xlat0.xyz = normalize(u_xlat0.xyz);
u_xlat16_2 = dot(u_xlat0.xyz, float3(0.577350259, 0.577350259, 0.577350259));
u_xlat16_2 = clamp(u_xlat16_2, 0, 1);
u_xlat16_5.xyz = (_Color.xyz * _Diffuse.xyz);
out_v.color.xyz = (float3(u_xlat16_2, u_xlat16_2, u_xlat16_2) * u_xlat16_5.xyz);
return out_v;
}

#define CODE_BLOCK_FRAGMENT
float4 u_xlat0_d;
float2 u_xlat1_d;
float4 u_xlat16_1;
float4 u_xlat10_1;
float u_xlat16_2_d;
float u_xlat10_9;
OUT_Data_Frag frag(v2f in_f)
{
    OUT_Data_Frag out_f;
    u_xlat0_d.xyz = (in_f.color.xyz * float3(_ShadowStrength, _ShadowStrength, _ShadowStrength));
    u_xlat1_d.xy = (in_f.texcoord.xy / in_f.texcoord.ww);
    u_xlat10_1 = tex2D(_ShadowTex, u_xlat1_d.xy);
    u_xlat0_d.xyz = (u_xlat0_d.xyz * u_xlat10_1.xyz);
    u_xlat16_2_d = ((-u_xlat10_1.w) + 1);
    u_xlat1_d.xy = (in_f.texcoord1.xy / in_f.texcoord1.ww);
    u_xlat10_9 = tex2D(_FalloffTex, u_xlat1_d.xy).w;
    u_xlat16_1.xyz = (float3(u_xlat10_9, u_xlat10_9, u_xlat10_9) * u_xlat0_d.xyz);
    u_xlat16_1.w = (u_xlat10_9 * u_xlat16_2_d);
    u_xlat0_d = (u_xlat16_1 * float4(_Factor, _Factor, _Factor, _Factor));
    out_f.color = u_xlat0_d;
    return out_f;
}


ENDCG

} // end phase
}
FallBack Off
}
