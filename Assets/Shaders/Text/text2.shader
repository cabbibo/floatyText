Shader "Text/Text2" {
  Properties {

    _Color ("Color", Color) = (1,1,1,1)
    _BaseHue ("Base Hue", float ) = 1
    
       _TextMap ("Textmap", 2D) = "white" {}
       _TextMap2 ("Textmap2", 2D) = "white" {}
       _TextMap3 ("Textmap3", 2D) = "white" {}
       _ColorMap ("ColorMap", 2D) = "white" {}
    
  }

  SubShader {
    // COLOR PASS


       
    Pass {
      Tags {"Queue"="Transparent" "RenderType"="Transparent" }
      Cull Off
      // ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

      CGPROGRAM
      #pragma target 4.5
      #pragma vertex vert
      #pragma fragment frag
      
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
      uniform sampler2D _TextMap2;
      uniform sampler2D _TextMap3;
      uniform sampler2D _ColorMap;
      uniform sampler2D _BackgroundTexture;

      float3 _Color;
      float _BaseHue;

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
        o.debug = float3(debug.x,debug.y,length(fVel));
        o.textureVal = v.extra.x;
        o.oversize  = v.extra.y;
        o.hueExtra = v.extra.z;
        o.special  = v.extra.w;

        UNITY_TRANSFER_SHADOW(o,o.worldPos);

        return o;
      }

      float4 frag(varyings v) : COLOR {
    
        fixed shadow = UNITY_SHADOW_ATTENUATION(v,v.worldPos -v.nor ) * .9 + .1 ;

        float d;
        if( v.textureVal < .5 ){
          d = tex2D(_TextMap,v.uv).a;
        }else if(v.textureVal > .5 && v.textureVal < 1.5 ){
          d = tex2D(_TextMap2,v.uv).a;
        }else{
          d = tex2D(_TextMap3,v.uv).a;
        }

        float smoothing = .1;
        float lum = smoothstep( 0.9 - smoothing , 0.9 + smoothing , d.x );

        

        if( d < .5 ){discard;}
        float3 bg = tex2Dproj(_BackgroundTexture, v.screenPos );
        float3 c = tex2D(_ColorMap,float2(v.debug.z * 10.1 + .7  + _BaseHue + v.hueExtra + d * .1,0) ).xyz;
        

        if( v.special > .5 ){
          c=1;
        }
        c = 1;//
         //if( length(bg)/3 < .4 ){ c = 1; }else{
         //c = 0;
         //}//1-d;

         //c = float3(1,0,0);
       //c *= lum;

        return float4(  c * 1.4 , saturate(2*d));
        //return 1;

      }

      ENDCG
    }


   // SHADOW PASS
/*
    Pass
    {
      Tags{ "LightMode" = "ShadowCaster" }


      Fog{ Mode Off }
      ZWrite On
      ZTest LEqual
      Cull Off
      Offset 1, 1
      CGPROGRAM

      #pragma target 4.5
      #pragma vertex vert
      #pragma fragment frag
      #pragma multi_compile_shadowcaster
      #pragma fragmentoption ARB_precision_hint_fastest
      #include "UnityCG.cginc"
      #include "../Chunks/StructIfDefs.cginc"
sampler2D _TextMap;
      struct v2f {
        V2F_SHADOW_CASTER;

        float2 uv : TEXCOORD1;
      };


      v2f vert(appdata_base v, uint id : SV_VertexID)
      {
        v2f o;
                o.uv =  _TransferBuffer[id].uv;
        o.pos = mul(UNITY_MATRIX_VP, float4(_TransferBuffer[id].pos, 1));
        return o;
      }

      float4 frag(v2f i) : COLOR
      {

        float4 col = tex2D(_TextMap,i.uv);
        if( col.a < .4){discard;}
        SHADOW_CASTER_FRAGMENT(i)
      }
      ENDCG
    }*/
  


  }

}
