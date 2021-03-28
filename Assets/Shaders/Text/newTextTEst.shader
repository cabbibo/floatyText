Shader "Text/Text2TEST" {
  Properties {

    _Color ("Color", Color) = (1,1,1,1)
    _BaseHue ("Base Hue", float ) = 1
    _Thickness ("_Thickness", float ) = .5
    _Falloff ("_Falloff", float ) = 10
    _NoiseSize ("_NoiseSize", float ) = 10
    _NoisePower ("_NoisePower", float ) = 10
    
    _TextMap ("Textmap", 2D) = "white" {}
    _CutoffMap ("CutoffMap", 2D) = "white" {}

    
  }

  SubShader {
    // COLOR PASS


       
    Pass {
      //Tags {"Queue"="Transparent+10" "RenderType"="Transparent" }
      Tags{ "LightMode" = "ForwardBase" }
      Cull Off
       //ZWrite Off
        Blend One One
        ZTest Always
      CGPROGRAM
      #pragma target 4.5
      #pragma vertex vert
      #pragma fragment frag

      #pragma multi_compile_fwdbase nolightmap nodirlightmap nodynlightmap novertexlight
      
      #include "UnityCG.cginc"
      #include "AutoLight.cginc"    

      struct Vert{
        float3 pos;
        float3 vel;
        float3 nor;
        float3 lock;
        float2 uv;
        float2 offset;
        float4 extra;
      };

      StructuredBuffer<Vert> _VertBuffer;
      StructuredBuffer<int> _TriBuffer;

      uniform sampler2D _TextMap;
      uniform sampler2D _CutoffMap;

      float3 _Color;
      float _BaseHue;
      float _Thickness;
      float _Falloff;
      float _NoiseSize;
      float _NoisePower;

      struct varyings {
        float4 pos    : SV_POSITION;
        float3 nor    : TEXCOORD0;
        float2 uv     : TEXCOORD1;
        float3 eye      : TEXCOORD5;
        float3 worldPos : TEXCOORD6;
        float3 debug    : TEXCOORD7;
        float3 closest    : TEXCOORD8;
        float4 screenPos : TEXCOORD9;
        float  textureVal : TEXCOORD10;
        float  oversize : TEXCOORD11;
        float  hueExtra : TEXCOORD12;
        float  special  : TEXCOORD13;
        UNITY_SHADOW_COORDS(2)
      };

      varyings vert(uint id : SV_VertexID) {

        Vert v = _VertBuffer[_TriBuffer[id]];

        float3 fPos   = v.pos;
        float3 fLock  = v.lock;
        float3 fVel   = v.vel;
        float3 fNor   = v.nor;
        float2 fUV    = v.uv;
        float2 debug  = v.offset;

        varyings o;

        UNITY_INITIALIZE_OUTPUT(varyings, o);

        fPos -=  UNITY_MATRIX_IT_MV[2].xyz * v.extra.w * .1 ;

        o.pos = mul(UNITY_MATRIX_VP, float4(fPos,1));
        o.worldPos = fPos;
        o.screenPos = ComputeScreenPos( mul(UNITY_MATRIX_VP, float4(fPos,1)));
        o.eye = _WorldSpaceCameraPos - fPos;
        o.nor = fNor;
        o.uv =  fUV;

        UNITY_TRANSFER_SHADOW(o,o.worldPos);
        return o;
      }

      float4 frag(varyings v) : COLOR {
    
        float d;
        float d2;
        d = tex2D(_TextMap,v.uv).x;
        d2 = tex2D(_CutoffMap,v.uv * float2(8,1) * _NoiseSize + _Time.x * .004).z;

        d = lerp(d,d*d2,d2)*1.2;// * 4;//d-d2 * _NoisePower;//smoothstep( d * 10 , 0,1);

        d = clamp( (d - _Thickness) * _Falloff , -1, 1) + 1;
        d /= 2;
        if( d < .4 ){ discard; }

        float3 col = d;//float3(1,0,0);
        return float4( col , 1);
        //return 1;

      }

      ENDCG
    }




    Pass
    {
      Tags{ "LightMode" = "ShadowCaster" }



      CGPROGRAM

      #pragma target 4.5
      #pragma vertex vert
      #pragma fragment frag

      #pragma multi_compile_shadowcaster
      #pragma fragmentoption ARB_precision_hint_fastest

      #include "UnityCG.cginc"
      sampler2D _MainTex;
      uniform sampler2D _TextMap;
      uniform sampler2D _CutoffMap;
      uniform float _NoiseSize;
      uniform float _Thickness;
      uniform float _Falloff;

      #include "../Chunks/ShadowCasterPos.cginc"
   

        struct Vert{
        float3 pos;
        float3 vel;
        float3 nor;
        float3 lock;
        float2 uv;
        float2 offset;
        float4 extra;
      };

      StructuredBuffer<Vert> _VertBuffer;
      StructuredBuffer<int> _TriBuffer;


 struct v2f {
        V2F_SHADOW_CASTER;
        float3 nor : NORMAL;
        float3 worldPos : TEXCOORD1;
        float2 uv : TEXCOORD0;
      };


      v2f vert(appdata_base input, uint id : SV_VertexID)
      {
        v2f o;
        Vert v = _VertBuffer[_TriBuffer[id]];

        float4 position = ShadowCasterPos(v.pos, -v.nor);
        o.pos = UnityApplyLinearShadowBias(position);
        o.worldPos = v.pos;
        o.uv = v.uv;
        return o;
      }
 
      v2f vert(uint id : SV_VertexID) {

        v2f o;
        Vert v = _VertBuffer[_TriBuffer[id]];

        float4 position = ShadowCasterPos(v.pos, -v.nor);
        o.pos = UnityApplyLinearShadowBias(position);
        o.worldPos = v.pos;
        o.uv = v.uv;
        return o;
      }

      float4 frag(v2f v) : COLOR {
    
        float d;
        float d2;
        d = tex2D(_TextMap,v.uv).x;
        d2 = tex2D(_CutoffMap,v.uv * float2(8,1) * _NoiseSize + _Time.x * .004).z;

        d = lerp(d,d*d2,d2)*1.2;// * 4;//d-d2 * _NoisePower;//smoothstep( d * 10 , 0,1);

        d = clamp( (d - _Thickness) * _Falloff , -1, 1) + 1;
        d /= 2;
        if( d < .2 ){ discard; }

        float3 col = d;//float3(1,0,0);
        return float4( col , 1);
        //return 1;

      }

      ENDCG
    }
  
    


  }

    FallBack "Diffuse"

}
