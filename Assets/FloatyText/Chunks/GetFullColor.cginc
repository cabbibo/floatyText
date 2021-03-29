 sampler2D _TerrainInfo1;
        sampler2D _TerrainInfo2;
        sampler2D _TerrainInfo3;
        sampler2D _TerrainInfo4;
    sampler2D _FullColorMap;

    float4 GetFullColor( float lookupVal , float2 uvPos  ){


                float maxID = 0;
                float secondMaxID = 0;
                float maxIDWeight = 0;
                float secondMaxIDWeight = 0;

                float4 info1 = tex2D( _TerrainInfo1 , float4( uvPos.x , uvPos.y , 0 , 0 ));
                float4 info2 = tex2D( _TerrainInfo2 , float4( uvPos.x , uvPos.y , 0 , 0 ));
                float4 info3 = tex2D( _TerrainInfo3 , float4( uvPos.x , uvPos.y , 0 , 0 ));
                float4 info4 = tex2D( _TerrainInfo4 , float4( uvPos.x , uvPos.y , 0 , 0 ));


                if( info1.r > maxIDWeight ){ maxID = 0; maxIDWeight = info1.r; }
                if( info1.g > maxIDWeight ){ maxID = 1; maxIDWeight = info1.g; }
                if( info1.b > maxIDWeight ){ maxID = 2; maxIDWeight = info1.b; }
                if( info1.a > maxIDWeight ){ maxID = 3; maxIDWeight = info1.a; }
                if( info2.r > maxIDWeight ){ maxID = 4; maxIDWeight = info2.r; }
                if( info2.g > maxIDWeight ){ maxID = 5; maxIDWeight = info2.g; }
                if( info2.b > maxIDWeight ){ maxID = 6; maxIDWeight = info2.b; }
                if( info2.a > maxIDWeight ){ maxID = 7; maxIDWeight = info2.a; }
                if( info3.r > maxIDWeight ){ maxID = 8; maxIDWeight = info3.r; }
                if( info3.g > maxIDWeight ){ maxID = 9; maxIDWeight = info3.g; }
                if( info3.b > maxIDWeight ){ maxID = 10; maxIDWeight = info3.b; }
                if( info3.a > maxIDWeight ){ maxID = 11; maxIDWeight = info3.a; }                
                if( info4.r > maxIDWeight ){ maxID = 12; maxIDWeight = info4.r; }
                if( info4.g > maxIDWeight ){ maxID = 13; maxIDWeight = info4.g; }
                if( info4.b > maxIDWeight ){ maxID = 14; maxIDWeight = info4.b; }
                if( info4.a > maxIDWeight ){ maxID = 15; maxIDWeight = info4.a; }


                if( info1.r > secondMaxIDWeight && info1.r != maxIDWeight ){ secondMaxID = 0; secondMaxIDWeight = info1.r; }
                if( info1.g > secondMaxIDWeight && info1.g != maxIDWeight ){ secondMaxID = 1; secondMaxIDWeight = info1.g; }
                if( info1.b > secondMaxIDWeight && info1.b != maxIDWeight ){ secondMaxID = 2; secondMaxIDWeight = info1.b; }
                if( info1.a > secondMaxIDWeight && info1.a != maxIDWeight ){ secondMaxID = 3; secondMaxIDWeight = info1.a; }
                if( info2.r > secondMaxIDWeight && info2.r != maxIDWeight ){ secondMaxID = 4; secondMaxIDWeight = info2.r; }
                if( info2.g > secondMaxIDWeight && info2.g != maxIDWeight ){ secondMaxID = 5; secondMaxIDWeight = info2.g; }
                if( info2.b > secondMaxIDWeight && info2.b != maxIDWeight ){ secondMaxID = 6; secondMaxIDWeight = info2.b; }
                if( info2.a > secondMaxIDWeight && info2.a != maxIDWeight ){ secondMaxID = 7; secondMaxIDWeight = info2.a; }
                if( info3.r > secondMaxIDWeight && info3.r != maxIDWeight ){ secondMaxID = 8; secondMaxIDWeight = info3.r; }
                if( info3.g > secondMaxIDWeight && info3.g != maxIDWeight ){ secondMaxID = 9; secondMaxIDWeight = info3.g; }
                if( info3.b > secondMaxIDWeight && info3.b != maxIDWeight ){ secondMaxID = 10; secondMaxIDWeight = info3.b; }
                if( info3.a > secondMaxIDWeight && info3.a != maxIDWeight ){ secondMaxID = 11; secondMaxIDWeight = info3.a; }                
                if( info4.r > secondMaxIDWeight && info4.r != maxIDWeight ){ secondMaxID = 12; secondMaxIDWeight = info4.r; }
                if( info4.g > secondMaxIDWeight && info4.g != maxIDWeight ){ secondMaxID = 13; secondMaxIDWeight = info4.g; }
                if( info4.b > secondMaxIDWeight && info4.b != maxIDWeight ){ secondMaxID = 14; secondMaxIDWeight = info4.b; }
                if( info4.a > secondMaxIDWeight && info4.a != maxIDWeight ){ secondMaxID = 15; secondMaxIDWeight = info4.a; }





           float4 cMap1 = tex2D( _FullColorMap , float2( lookupVal, 1-(maxID  + .5) / 16));
            float4 cMap2 = tex2D( _FullColorMap , float2( lookupVal, 1-(secondMaxID  + .5) / 16));

           return  cMap1  * maxIDWeight + cMap2 * secondMaxIDWeight;//(v.nor * .5 + .5) * v.color.w * _Color;//1;///_Color;//saturate(sin(length(dif) * .1 - _Time.y * 3));// / 1000;//lerp( 0 , c2 , l * .1);//_Color * (v.nor * .5 + .5)  - l;// hsv(v.normal.y * .5,1,1);


    }