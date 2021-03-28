Shader "Debug/TextDebug" {
	Properties {
    _Color ("Color", Color) = (1,1,1,1)
    _Size ("Size", float) = .01
    _TextMap ("Textmap", 2D) = "white" {}
	}


  SubShader{
    Cull Off
    Pass{

      CGPROGRAM      


      #pragma target 4.5

      #pragma vertex vert
      #pragma fragment frag

      #include "UnityCG.cginc"


    struct Vert{
		    float3 pos;
		    float3 nor;
		    float4 glyph;
		    float2 debug;
		};

      uniform int _Count;
      uniform float _Size;
      uniform float3 _Color;
      uniform sampler2D _TextMap;
      StructuredBuffer<Vert> _VertBuffer;


      //uniform float4x4 worldMat;

      //A simple input struct for our pixel shader step containing a position.
      struct varyings {
          float4 pos      : SV_POSITION;
          float3 nor      : TEXCOORD0;
          float3 worldPos : TEXCOORD1;
          float3 eye      : TEXCOORD2;
          float3 debug    : TEXCOORD3;
          float2 uv       : TEXCOORD4;
          float2  textPos    : TEXCOORD6;
          float2  dimensions : TEXCOORD7;
          float2  offset     : TEXCOORD8;
      };


      //Our vertex function simply fetches a point from the buffer corresponding to the vertex index
      //which we transform with the view-projection matrix before passing to the pixel program.
      varyings vert (uint id : SV_VertexID){

        varyings o;

        int base = id / 6;
        int alternate = id %6;
        if( base < _Count ){

        	float3 extra = float3(0,0,0);

          float3 l = UNITY_MATRIX_V[0].xyz;
          float3 u = UNITY_MATRIX_V[1].xyz;
          float2 uv = float2(0,0);

        	if( alternate == 0 ){ extra = -l - u; uv = float2(0,0); }
          if( alternate == 1 ){ extra =  l - u; uv = float2(1,0); }
          if( alternate == 2 ){ extra =  l + u; uv = float2(1,1); }
          if( alternate == 3 ){ extra = -l - u; uv = float2(0,0); }
          if( alternate == 4 ){ extra =  l + u; uv = float2(1,1); }
        	if( alternate == 5 ){ extra = -l + u; uv = float2(0,1); }


        	Vert v = _VertBuffer[base % _Count];
       		o.worldPos = (v.pos) + extra * _Size;///* .001/(.1+length(v.debug));//*(1/(.1+max(length(v.debug),0)));//mul( worldMat , float4( v.pos , 1.) ).xyz;
	        //o.debug = v.debug.x;
	        o.eye = _WorldSpaceCameraPos - o.worldPos;
          o.nor =v.nor;
          o.uv = uv;//v.uv;

          o.textPos = v.glyph.xy;
          o.dimensions = v.glyph.zw;
          o.offset= v.debug.xy;

	        o.pos = mul (UNITY_MATRIX_VP, float4(o.worldPos,1.0f));

       	}

        return o;

      }




      //Pixel function returns a solid color for each point.
      float4 frag (varyings v) : COLOR {

      	float3 c = float3(0,1,0);

      	//float x = v.textPos.x - v.offset.x
  				float2 fUV = (v.uv *v.dimensions-v.offset + float2(0,1)+float2(1,-1)*v.textPos );


      	c = tex2D(_TextMap,fUV).xyz;

        return float4(c,1 );
      }

      ENDCG

    }
  }

  Fallback Off


}
