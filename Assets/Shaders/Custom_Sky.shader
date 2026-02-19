Shader "Custom/Sky"
{
Properties
{
    _GradientFade ("Fade", float) = 1
    _GradientOffset ("Gradient offset", float) = 0
}
SubShader
{
Tags
{
    "QUEUE" = "Background+2000"
    "RenderType" = "Opaque"
}
Pass // ind: 1, name: 
{
Tags
{
    "QUEUE" = "Background+2000"
    "RenderType" = "Opaque"
}
ZWrite Off
Fog
{
    Mode  Off
}
// m_ProgramMask = 6
CGPROGRAM
//#pragma target 4.0

#pragma vertex vert
#pragma fragment frag

#include "UnityCG.cginc"


#define CODE_BLOCK_VERTEX
//uniform float4x4 unity_ObjectToWorld;
//uniform float4x4 unity_MatrixVP;
uniform float _SkyGradientOffset;
uniform float4 _SkyGradientBottomColor;
uniform float4 _SkyGradientTopColor;
struct appdata_t
{
    float4 vertex :POSITION0;
};

struct OUT_Data_Vert
{
    float4 color :COLOR0;
    float4 vertex :SV_POSITION;
};

struct v2f
{
    float4 color :COLOR0;
};

struct OUT_Data_Frag
{
    float4 color :SV_Target0;
};

float4 u_xlat0;
float4 u_xlat1;
float3 u_xlat2;
OUT_Data_Vert vert(appdata_t in_v)
{
    OUT_Data_Vert out_v;
    u_xlat0 = UnityObjectToClipPos(in_v.vertex);
    out_v.vertex = u_xlat0;
    u_xlat0.x = (u_xlat0.y / u_xlat0.w);
    u_xlat0.x = (u_xlat0.x + (-_SkyGradientOffset));
    u_xlat2.x = ((-_SkyGradientOffset) + 1);
    u_xlat0.x = (u_xlat0.x / u_xlat2.x);
    u_xlat0.x = clamp(u_xlat0.x, 0, 1);
    u_xlat2.xyz = ((-_SkyGradientBottomColor.xyz) + _SkyGradientTopColor.xyz);
    out_v.color.xyz = ((u_xlat0.xxx * u_xlat2.xyz) + _SkyGradientBottomColor.xyz);
    out_v.color.w = 1;
    return out_v;
}

    #define CODE_BLOCK_FRAGMENT
OUT_Data_Frag frag(v2f in_f)
{
    OUT_Data_Frag out_f;
    out_f.color = in_f.color;
    return out_f;
}


ENDCG

} // end phase
}
FallBack "Diffuse"
}
