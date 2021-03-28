float4 SampleAudio( float v){
  return tex2D(_AudioMap, float2(v,0));
}