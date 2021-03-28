Shader "Debug/Glyph" {
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

            StructuredBuffer<Vert> _VertBuffer;
            StructuredBuffer<int> _TriBuffer;
            
            uniform sampler2D _TextMap;
            uniform sampler2D _ColorMap;
            uniform sampler2D _BackgroundTexture;

            float3 _Color;
            float4x4 _World;

            int _BaseID;
            int _TipID;
            int _TextCount;

            struct varyings {
                float4 pos      : SV_POSITION;
                float2 uv       : TEXCOORD1;
                float2 uv2      : TEXCOORD2;
                float3 debug    : TEXCOORD7;
            };

            float3 GetPosition( int pID ){
                return .1 * float3( float( pID ) % 20 , -floor( float( pID )/20 ) , 0);
            }

            varyings vert(uint id : SV_VertexID) {

                int pID = id/6;
                int idInP = id%6;


                Vert v = _VertBuffer[_TriBuffer[id]];

                float3 fPos     = v.pos;
                float3 fLock    = v.lock;
                float3 fVel     = v.vel;
                float3 fNor     = v.nor;
                float2 fUV      = v.uv;

               // float2 debug    = v.debug;

                varyings o;

                UNITY_INITIALIZE_OUTPUT(varyings, o);

                float3 offset = float3(1,0,0);
                float2 uv2 = 0;

                if( idInP == 0 ||  idInP == 3 ){ uv2 = float2(-1,-1); offset = float3(-1, -1,0);  }
                if( idInP == 2 ||  idInP == 4 ){ uv2 = float2( 1, 1); offset = float3( 1,  1,0);  }
                if( idInP == 1                ){ uv2 = float2( 1,-1); offset = float3( 1, -1,0);  }
                if( idInP == 5                ){ uv2 = float2(-1, 1); offset = float3(-1,  1,0);  }


                fPos = offset * .03 +  GetPosition(pID);

                o.pos = mul(UNITY_MATRIX_VP, mul(_World,float4(fPos,1)));
                o.uv =  fUV;
                o.uv2 = uv2;

                

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
                }

                o.debug = debug;

                UNITY_TRANSFER_SHADOW(o,o.worldPos);

                return o;
            }

            float4 frag(varyings v) : COLOR {
        
                float d = tex2D(_TextMap,v.uv);

                float smoothing = .2;
                float lum = smoothstep( 0.4 - smoothing , 0.4 + smoothing , d.x );

               // if( abs( v.uv2.x) < .8 &&abs( v.uv2.y) < .8 ){discard;}
          
                float3 col = v.debug;
                if( d > .4 ){ col =1;}
                return float4(col, 1);

            }

            ENDCG
        }


    }

}
