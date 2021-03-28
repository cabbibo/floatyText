Shader "Debug/TextLocation" {
  Properties {
    _TextMap ("Textmap", 2D) = "white" {}
  }

    SubShader {
        // COLOR PASS


        Pass {
            //Tags{ "LightMode" = "ForwardBase" }
            Cull Off
            //Blend OneMinusDstColor One // Soft Additive

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
              float2 debug;
            };

            StructuredBuffer<Vert> _ParticleBuffer;
            
            

            float3 _Color;
            float4x4 _World;

            int _BaseID;
            int _TipID;
            int _TextCount;

            struct varyings {
                float4 pos      : SV_POSITION;
                float3 nor      : TEXCOORD0;
                float2 uv       : TEXCOORD1;
                float3 eye      : TEXCOORD5;
                float3 worldPos : TEXCOORD6;
                float3 debug    : TEXCOORD7;
                float3 closest    : TEXCOORD8;
                UNITY_SHADOW_COORDS(2)
            };

            float3 GetPosition( int pID ){
                return .1 *float3( float( pID ) % 20 , -floor( float( pID )/20 ) , 0);
            }

            varyings vert(uint id : SV_VertexID) {

                int pID = id/3;
                int idInP = id%3;


                Vert v = _ParticleBuffer[pID];

                float3 fPos     = v.pos;

                varyings o;

                UNITY_INITIALIZE_OUTPUT(varyings, o);

         


                float3 baseLocation = GetPosition(pID);

                baseLocation  = mul(_World,float4(baseLocation,1)).xyz;

                if( idInP == 0 ){ fPos = baseLocation + mul(_World,float4(1,0,0,0)).xyz * .01; }
                if( idInP == 1 ){ fPos = baseLocation - mul(_World,float4(1,0,0,0)).xyz * .01; }

                o.pos = mul(UNITY_MATRIX_VP, float4(fPos,1));
             

                int inBase = _BaseID % _TextCount;
                int dif = _TipID - _BaseID;

                int newID = pID;
                if( inBase + dif > _TextCount ){
                  if( newID < inBase ){ newID += _TextCount; }
                }

                float3 debug = 1;
                if( newID >=  inBase && newID < inBase + dif ){
                   debug = float3(1,.8,.3);

                }else{
                    debug = float3(0.3,1,.8);
                    o.pos = 0;
                }

                o.debug = debug;

                UNITY_TRANSFER_SHADOW(o,o.worldPos);

                return o;
            }

            float4 frag(varyings v) : COLOR {
        
              
          
                return float4(v.debug, 1);

            }

            ENDCG
        }


    }

}
