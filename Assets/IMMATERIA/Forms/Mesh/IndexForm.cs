﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;


public class IndexForm : Form {


  public Form toIndex;

  public override void _Create(){

    if( mpb == null ){ mpb = new MaterialPropertyBlock(); }
    if( toIndex == null ){ toIndex = GetComponent<Form>(); } 
    DoCreate();
    SetStructSize();
    SetCount();
    SetBufferType();
  }
  public override void SetBufferType(){  intBuffer = true; }
  public override void SetStructSize(){ structSize = 1; }
  

  public override void WhileDebug(){

    mpb.SetBuffer("_VertBuffer", toIndex._buffer);
    mpb.SetBuffer("_TriBuffer", _buffer);
    mpb.SetInt("_Count",count);
    mpb.SetInt("_VertCount",toIndex.count);
     Graphics.DrawProcedural(debugMaterial,  new Bounds(transform.position, Vector3.one * 5000), MeshTopology.Lines, count  * 2 , 1, null, mpb, ShadowCastingMode.Off, true, LayerMask.NameToLayer("Debug"));

  }

}
