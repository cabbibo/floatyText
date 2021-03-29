float4 TriplanarTexture( float3 pos , float3 nor , float size){
  float4 t1 = tex2D(_TextureMap , pos.zy * size ) * abs(nor.x);
  float4 t2 = tex2D(_TextureMap , pos.xz * size ) * abs(nor.y);
  float4 t3 = tex2D(_TextureMap , pos.xy * size ) * abs(nor.z);
  return t1 + t2 + t3;
}