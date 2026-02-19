Shader "Custom/Distorted/Transparent"
{
Properties
{
    _MainTex ("Base (RGB)", 2D) = "white" {}
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
Blend SrcAlpha OneMinusSrcAlpha
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
uniform float _SkyGradientOffset;
uniform float4 _FogSilhouetteColor;
uniform float4 _SkyGradientBottomColor;
uniform float4 _SkyGradientTopColor;
uniform float _Factor;
uniform sampler2D _MainTex;
struct appdata_t
{
    float4 vertex :POSITION0;
    float4 texcoord :TEXCOORD0;
};

struct OUT_Data_Vert
{
    float2 texcoord :TEXCOORD0;
    float4 texcoord1 :TEXCOORD1;
    float4 vertex :SV_POSITION;
};

struct v2f
{
    float2 texcoord :TEXCOORD0;
    float4 texcoord1 :TEXCOORD1;
};

struct OUT_Data_Frag
{
    float4 color :SV_Target0;
};

float4 u_xlat0;
float4 u_xlat1;
float3 u_xlat2;
float2 u_xlat3;
float u_xlat9;
OUT_Data_Vert vert(appdata_t in_v)
{
    OUT_Data_Vert out_v;
    u_xlat0 = UnityObjectToClipPos(in_v.vertex);
    u_xlat1.x = (u_xlat0.z * u_xlat0.z);
    u_xlat0.xy = ((u_xlat1.xx * _Distort.xy) + u_xlat0.xy);
    out_v.vertex.xy = u_xlat0.xy;
    u_xlat0.x = (u_xlat0.y / u_xlat0.w);
    u_xlat0.x = (u_xlat0.x + (-_SkyGradientOffset));
    out_v.vertex.zw = u_xlat0.zw;
    u_xlat3.xy = ((-u_xlat0.ww) + float2(1000, 700));
    u_xlat3.xy = (u_xlat3.xy * float2(0.00333333341, 0.00200000009));
    out_v.texcoord.xy = TRANSFORM_TEX(in_v.texcoord.xy, _MainTex);
    u_xlat9 = ((-_SkyGradientOffset) + 1);
    u_xlat0.x = (u_xlat0.x / u_xlat9);
    u_xlat0.x = clamp(u_xlat0.x, 0, 1);
    u_xlat1.xyz = ((-_SkyGradientBottomColor.xyz) + _SkyGradientTopColor.xyz);
    u_xlat1.xyz = ((u_xlat0.xxx * u_xlat1.xyz) + _SkyGradientBottomColor.xyz);
    u_xlat2.xyz = ((-u_xlat1.xyz) + _FogSilhouetteColor.xyz);
    u_xlat3.x = u_xlat3.x;
    u_xlat3.x = clamp(u_xlat3.x, 0, 1);
    out_v.texcoord1.w = u_xlat3.y;
    out_v.texcoord1.w = clamp(out_v.texcoord1.w, 0, 1);
    out_v.texcoord1.xyz = ((u_xlat3.xxx * u_xlat2.xyz) + u_xlat1.xyz);
    return out_v;
}

    #define CODE_BLOCK_FRAGMENT
float4 u_xlat0_d;
float3 u_xlat1_d;
OUT_Data_Frag frag(v2f in_f)
{
    OUT_Data_Frag out_f;
    u_xlat0_d = tex2D(_MainTex, in_f.texcoord.xy);
    u_xlat1_d.xyz = (u_xlat0_d.xyz + (-in_f.texcoord1.xyz));
    u_xlat1_d.xyz = ((in_f.texcoord1.www * u_xlat1_d.xyz) + in_f.texcoord1.xyz);
    u_xlat0_d.xyz = (u_xlat1_d.xyz * float3(_Factor, _Factor, _Factor));
    out_f.color = u_xlat0_d;
    return out_f;
}


ENDCG

} // end phase
}
FallBack Off
}