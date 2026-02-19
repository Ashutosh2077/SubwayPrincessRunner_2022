Shader "Custom/Additive Constant Color"
{
Properties
{
    _MainTex ("Main Texture", 2D) = "white" {}
_MainColor ("Color (RGBC)", Color) = (1,1,1,0)
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
    float4 vertex :SV_POSITION;
};

struct v2f
{
    float2 texcoord :TEXCOORD0;
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
    out_v.vertex = UnityObjectToClipPos(in_v.vertex);
    out_v.texcoord.xy = TRANSFORM_TEX(in_v.texcoord.xy, _MainTex);
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
    out_f.color = u_xlat0_d;
    return out_f;
}


ENDCG

} // end phase
}
FallBack Off
}
