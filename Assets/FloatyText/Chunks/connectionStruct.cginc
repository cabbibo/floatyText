struct Vert{
  float3 pos;
  float3 vel;
  float3 nor;
  float3 tan;
  float2 uv;
  float2 offset;
  float4 debug;
  float3 connections[16];
};