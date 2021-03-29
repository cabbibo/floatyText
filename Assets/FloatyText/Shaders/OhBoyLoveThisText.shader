Shader "Text/OhBoyLoveThisTexts" {
  Properties {

    _Color ("Color", Color) = (1,1,1,1)
    _BaseHue ("Base Hue", float ) = 1
    _Thickness ("_Thickness", float ) = .5
    _Falloff ("_Falloff", float ) = 10
    _NoiseSize ("_NoiseSize", float ) = 10
    _NoisePower ("_NoisePower", float ) = 10
    
    _TextMap ("Textmap", 2D) = "white" {}
    _CutoffMap ("CutoffMap", 2D) = "white" {}

    _AlphaCutoff ("_AlphaCutoff", float ) = .3

    
    _ColorMap ("ColorMap", 2D) = "white" {}

    
  }



     
    CGINCLUDE 
    
      #pragma target 4.5
      #pragma vertex vert
      #pragma fragment frag

      
        uniform sampler2D _TextMap;
    
      uniform sampler2D _CutoffMap; 

      

      float _AlphaCutoff;    
      float _Thickness;
      float _Falloff;
      float _NoiseSize;
      float _NoisePower;   
      
      
       struct Vert{
        float3 pos;
        float3 vel;
        float3 nor;
        float3 lock;
        float2 uv;
        float2 offset;
        float textureVal;
        float scaleOffset;
        float hueOffset;
        float special; 

      };

    StructuredBuffer<Vert> _VertBuffer;
      StructuredBuffer<int> _TriBuffer;



      
  
      float getD( float2 uv , float textureVal){
        
        float4 tCol = tex2D(_TextMap,uv);

        float d = tCol.x;
        if( textureVal > .5 && textureVal < 1.5 ){
            d = tCol.y;
        }else if( textureVal > 1.5 && textureVal < 2.5 ){
            d = tCol.z;
        }else if( textureVal > 2.5 && textureVal < 3.5 ){
            d = tCol.w;
        }

        return d;

      }


      float GetCutoff(float2 uv , float textureVal){
        float d;
        float d2;

        d = getD( uv , textureVal);
        d2 = tex2D(_CutoffMap,uv * float2(8,1) * _NoiseSize + _Time.x * .004).z;

        d = lerp(d,d*d2,d2 *_NoisePower)*1.2;// * 4;//d-d2 * _NoisePower;//smoothstep( d * 10 , 0,1);

        d = clamp( (d - _Thickness) * _Falloff , -1, 1) + 1;
        d /= 2;

        return d;
      }
    ENDCG

  SubShader {
    // COLOR PASS



       
    Pass {
     //Tags {"Queue"="Transparent+10" "RenderType"="Transparent" }
      Tags{ "LightMode" = "ForwardBase" }
      Cull Off
      ZWrite On
        Blend One One
        ZTest Always



     //CGPROGRAM
     //#pragma target 4.5
     //#pragma vertex vert
     //#pragma fragment frag
     //#pragma multi_compile_fwdbase nolightmap nodirlightmap nodynlightmap novertexlight
      CGPROGRAM

      #pragma multi_compile_fwdbase nolightmap nodirlightmap nodynlightmap novertexlight
      
      #include "UnityCG.cginc"
      #include "AutoLight.cginc"    




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
        float  scaleOffset : TEXCOORD11;
        float  hueOffset : TEXCOORD12;
        float  special  : TEXCOORD13;
        float  vel  : TEXCOORD14;
        UNITY_SHADOW_COORDS(2)
      };

      varyings vert(uint id : SV_VertexID) {

        Vert v = _VertBuffer[_TriBuffer[id]];

        varyings o;

        UNITY_INITIALIZE_OUTPUT(varyings, o);

        v.pos -=  UNITY_MATRIX_IT_MV[2].xyz * v.special * .1 ;

        o.pos = mul(UNITY_MATRIX_VP, float4(v.pos,1));
        o.worldPos = v.pos;
        o.screenPos = ComputeScreenPos( mul(UNITY_MATRIX_VP, float4(v.pos,1)));
        o.eye = _WorldSpaceCameraPos - v.pos;
        
        o.nor = v.nor;
        o.uv =  v.uv;
        o.special = v.special;
        o.scaleOffset = v.scaleOffset;
        o.textureVal = v.textureVal;
        o.hueOffset = v.hueOffset;
        o.vel = length( v.vel);


        UNITY_TRANSFER_SHADOW(o,o.worldPos);
        return o;
      }

    
      uniform sampler2D _ColorMap;



      float4 frag(varyings v) : COLOR {

      
        float d = GetCutoff( v.uv , v.textureVal );
        if( d < _AlphaCutoff ){ discard; }


        float3 c = tex2D(_ColorMap,float2(v.vel * 10.1 + .7  + _BaseHue + v.hueOffset + d * .1,0) ).xyz;
        

       

        return float4(  c * 1.4 , saturate(2*d));

        float3 col = c;//float3(1,0,0);
        return float4( col , d);
        //return 1;

      }

      ENDCG
    }




    Pass
    {
      Tags{ "LightMode" = "ShadowCaster" }



      CGPROGRAM


      #pragma multi_compile_shadowcaster
      #pragma fragmentoption ARB_precision_hint_fastest

      #include "UnityCG.cginc"
    

      #include "../Chunks/ShadowCasterPos.cginc"
   

  


 struct v2f {
        V2F_SHADOW_CASTER;
        float3 nor : NORMAL;
        float3 worldPos : TEXCOORD1;
        float2 uv : TEXCOORD0;
        float  textureVal : TEXCOORD10;
        float  scaleOffset : TEXCOORD11;
        float  hueOffset : TEXCOORD12;
        float  special  : TEXCOORD13;
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
        
        o.uv =  v.uv;
        o.special = v.special;
        o.scaleOffset = v.scaleOffset;
        o.textureVal = v.textureVal;
        o.hueOffset = v.hueOffset;
        return o;
      }

      float4 frag(v2f v) : COLOR {
    
        float d = GetCutoff( v.uv , v.textureVal );
        if( d < _AlphaCutoff ){ discard; }

        float3 col = d;//float3(1,0,0);
        return float4( col , d);
        //return 1;

      }

      ENDCG
    }
  
    


  }

    FallBack "Diffuse"

}
