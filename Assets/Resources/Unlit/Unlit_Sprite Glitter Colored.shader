Shader "Unlit/Sprite Glitter Colored"
{
Properties
{
    _MainTex ("RGB", 2D) = "black" {}
_FlowLightTex ("FlowLight Texture", 2D) = "white" {}
_FlowLightPower ("FlowLightPower", float) = 1
_IsOpenFlowLight ("IsOpenFlowLight", float) = 0
_FlowLightOffset ("FlowLight Offset", float) = 0
_WidthRate ("Sprite Width / Altas Width", float) = 0
_HeightRate ("Sprite Height / Altas Height", float) = 0
_OffsetXRate ("Sprite Offset X / Altas Width", float) = 0
_OffsetYRate ("Sprite Offset Y / Altas Height", float) = 0
}
SubShader
{
    Tags
{
    "IGNOREPROJECTOR" = "true"
    "QUEUE" = "Transparent"
    "RenderType" = "Transparent"
}
LOD 200
Pass // ind: 1, name: 
{
    Tags
{
    "IGNOREPROJECTOR" = "true"
    "QUEUE" = "Transparent"
    "RenderType" = "Transparent"
}
LOD 200
ZWrite Off
Cull Off
Offset -1, -1
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
uniform float _FlowLightPower;
uniform int _IsOpenFlowLight;
uniform float _FlowLightOffset;
uniform float _WidthRate;
uniform float _HeightRate;
uniform float _OffsetXRate;
uniform float _OffsetYRate;
uniform sampler2D _MainTex;
uniform sampler2D _FlowLightTex;
struct appdata_t
{
    float4 vertex :POSITION0;
    float2 texcoord :TEXCOORD0;
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
    out_v.texcoord.xy = in_v.texcoord.xy;
    return out_v;
}

    #define CODE_BLOCK_FRAGMENT
float4 u_xlat10_0;
float2 u_xlat1_d;
float3 u_xlat16_1;
float3 u_xlat10_1;
int u_xlatb1;
int u_xlatb3;
OUT_Data_Frag frag(v2f in_f)
{
    OUT_Data_Frag out_f;
    u_xlat10_0 = tex2D(_MainTex, in_f.texcoord.xy);
    u_xlatb1 = (0.99000001<u_xlat10_0.w);
    u_xlatb3 = (_IsOpenFlowLight==1);
    u_xlatb1 = (u_xlatb3 && u_xlatb1);
    if(u_xlatb1)
    {
        u_xlat1_d.xy = (in_f.texcoord.xy + (-float2(_OffsetXRate, _OffsetYRate)));
        u_xlat1_d.xy = (u_xlat1_d.xy / float2(_WidthRate, _HeightRate));
        u_xlat16_1.x = ((u_xlat1_d.x * 0.5) + (-_FlowLightOffset));
        u_xlat1_d.x = u_xlat16_1.x;
        u_xlat10_1.xyz = tex2D(_FlowLightTex, u_xlat1_d.xy).xyz;
        u_xlat16_1.xyz = (u_xlat10_1.xyz * float3(_FlowLightPower, _FlowLightPower, _FlowLightPower));
        out_f.color.xyz = ((u_xlat16_1.xyz * u_xlat10_0.xyz) + u_xlat10_0.xyz);
        out_f.color.w = (u_xlat10_0.w * u_xlat10_0.w);
    }
    else
    {
        out_f.color = u_xlat10_0;
    }
    return out_f;
}


ENDCG

} // end phase
}
SubShader
{
    Tags
{
    "IGNOREPROJECTOR" = "true"
    "QUEUE" = "Transparent"
    "RenderType" = "Transparent"
}
LOD 100
Pass // ind: 1, name: 
{
    Tags
{
    "IGNOREPROJECTOR" = "true"
    "QUEUE" = "Transparent"
    "RenderType" = "Transparent"
}
LOD 100
ZWrite Off
Cull Off
Offset -1, -1
Fog
{
    Mode  Off
}
Blend SrcAlpha OneMinusSrcAlpha
ColorMask RGB
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
uniform float4 _AlphaTex_ST;
uniform sampler2D _MainTex;
uniform sampler2D _AlphaTex;
struct appdata_t
{
    float3 vertex :POSITION0;
    float4 color :COLOR0;
    float3 texcoord :TEXCOORD0;
};

struct OUT_Data_Vert
{
    float4 color :COLOR0;
    float2 texcoord :TEXCOORD0;
    float2 texcoord1 :TEXCOORD1;
    float4 vertex :SV_POSITION;
};

struct v2f
{
    float4 color :COLOR0;
    float2 texcoord :TEXCOORD0;
    float2 texcoord1 :TEXCOORD1;
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
    out_v.color = in_v.color;
    out_v.color = clamp(out_v.color, 0, 1);
    out_v.texcoord.xy = TRANSFORM_TEX(in_v.texcoord.xy, _MainTex);
    out_v.texcoord1.xy = TRANSFORM_TEX(in_v.texcoord.xy, _AlphaTex);
    out_v.vertex = UnityObjectToClipPos(float4(in_v.vertex, 0));
    return out_v;
}

    #define CODE_BLOCK_FRAGMENT
float3 u_xlat10_0;
OUT_Data_Frag frag(v2f in_f)
{
    OUT_Data_Frag out_f;
    u_xlat10_0.xyz = tex2D(_MainTex, in_f.texcoord.xy).xyz;
    out_f.color.xyz = (u_xlat10_0.xyz * in_f.color.xyz);
    u_xlat10_0.x = tex2D(_AlphaTex, in_f.texcoord1.xy).w;
    out_f.color.w = (u_xlat10_0.x * in_f.color.w);
    return out_f;
}


ENDCG

} // end phase
}
FallBack Off
}
