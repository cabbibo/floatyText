
#include "UnityCG.cginc"
#include "AutoLight.cginc"

struct v2f{ 
  float4 pos        : SV_POSITION; 
  float3 nor        : NORMAL; 

  float debug       : TEXCOORD0; 
  
  float3 eye        : TEXCOORD1;
  float3 world      : TEXCOORD2;  
  float2 uv         : TEXCOORD3; 
  float4 screenPos  : TEXCOORD4;

  // For our matrix
  float3 t1         : TEXCOORD5;
  float3 t2         : TEXCOORD6;
  float3 t3         : TEXCOORD7;

            float3 vel : TEXCOORD8;
  
            UNITY_SHADOW_COORDS(9)

};




StructuredBuffer<Vert> _VertBuffer;
StructuredBuffer<int> _TriBuffer;

sampler2D _ColorMap;
sampler2D _TextureMap;
sampler2D _AudioMap;
sampler2D _NormalMap;

sampler2D _PainterlyLightMap;
samplerCUBE _CubeMap;

float2 _NormalSize;
float2 _PaintSize;
float _NormalDepth;

float _ColorBase;
float _ColorSize;

v2f vert ( uint vid : SV_VertexID )
{
    v2f o;

    UNITY_INITIALIZE_OUTPUT(v2f, o);

    Vert v = _VertBuffer[_TriBuffer[vid]];

    o.world = v.pos;
    o.uv = v.uv;
    o.pos = mul (UNITY_MATRIX_VP, float4(v.pos,1.0f));
    o.nor = v.nor;//normalize(cross(v0.pos - v1.pos , v0.pos - v2.pos ));
    o.debug = v.debug;
    o.eye = v.pos - _WorldSpaceCameraPos;
    o.screenPos = ComputeScreenPos(o.pos);
    o.vel = v.vel;

    float3 bi = cross(v.nor, v.tan);
    
    // output the tangent space matrix
    o.t1 =  float3(v.tan.x, bi.x, v.nor.x);
    o.t2 =  float3(v.tan.y, bi.y, v.nor.y);
    o.t3 =  float3(v.tan.z, bi.z, v.nor.z);

    TRANSFER_SHADOW(o);
    
    return o;
}