﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshVerts : Form {

  [HideInInspector]public Mesh mesh;
  public MeshFilter meshFilter;
  public bool transformVerts;
  
  /*struct Vert{
    public Vector3 pos;
    public Vector3 vel;
    public Vector3 nor;
    public Vector3 tan;
    public Vector2 uv;
    public float2 debug;
  };*/

  public override void Create(){

    if( meshFilter == null ){ meshFilter = GetComponent<MeshFilter>(); } 
    mesh = meshFilter.sharedMesh;

  }

  public override void SetStructSize(){ 
    structSize = 16; 
  }

  public override void SetCount(){ 
    count = mesh.vertices.Length;
  }

  public override void Embody(){

    Vector3[] verts = mesh.vertices;
    Vector2[] uvs   = mesh.uv;
    Vector3[] nors  = mesh.normals;
    Vector4[] tans  = mesh.tangents;

    bool hasTan = false;
    if( tans.Length == verts.Length ){ hasTan = true; }

    bool hasUV = false;
    if( uvs.Length == verts.Length ){ hasUV = true; }

    int index = 0;


//    print("Creatings");

    float[] values = new float[count*structSize];
    for( int i = 0; i < count; i ++ ){


      if( transformVerts ){ verts[i] = transform.TransformPoint( verts[i] ); }
      values[ index ++ ] = verts[i].x;
      values[ index ++ ] = verts[i].y;
      values[ index ++ ] = verts[i].z;

      values[ index ++ ] = 0;
      values[ index ++ ] = 0;
      values[ index ++ ] = 0;

      if( transformVerts ){ nors[i] = transform.TransformDirection( nors[i] ); }
      values[ index ++ ] = nors[i].x;
      values[ index ++ ] = nors[i].y;
      values[ index ++ ] = nors[i].z;


      if(hasTan){

        Vector3 tT = meshFilter.transform.TransformDirection( HELP.ToV3(tans[i]) ) ;
        
        if( transformVerts ){ tans[i] = new Vector4(tT.x , tT.y , tT.z ,1); }

        values[ index ++ ] = tans[i].x;
        values[ index ++ ] = tans[i].y;
        values[ index ++ ] = tans[i].z;
        
      }else{
        values[ index ++ ] = 0;
        values[ index ++ ] = 0;
        values[ index ++ ] = 0;
      }


      if( hasUV ){
        values[ index ++ ] = uvs[i].x;
        values[ index ++ ] = uvs[i].y;
      }else{
        values[ index ++ ] = 0;
        values[ index ++ ] = 0;
      }

      values[ index ++ ] = (float)i/(float)count;
      values[ index ++ ] = count;
    }

    SetData( values );

  }

}