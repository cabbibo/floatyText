float Painterly(float val , float2 uv ){


  float4 p = tex2D( _PainterlyLightMap , uv );

  float m = val * 3;
  float f = 0;
  if( m < 1 ){
      f = lerp( p.x , p.y , m );
  }else if( m >= 1 && m < 2){
      f = lerp( p.y , p.z , m-1 );
  }else if( m >= 2 && m < 3){
      f = lerp( p.z , p.w , m-2 );
  }

  return f;

}