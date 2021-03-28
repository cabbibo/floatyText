using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class ClosestLife : Life{
  
  public int numberGroups;
  public float[] values;
  public Vector4 value;
  public ComputeBuffer _buffer;
  public uint count;

  public Vector3 closest;
  public float closestID;

  public float oClosestID;
  public Vector3 oClosest;


  public Vector3 closestPos;
  public float  closestDist;




  public float closestTime;

  public Form oForm;
  public Form inspectorForm;

  public bool alwaysCheck;

  public bool tmpActive;


  Queue<AsyncGPUReadbackRequest> _requests = new Queue<AsyncGPUReadbackRequest>();
  
  public override void _Create(){
    
    /*
      Normal base class creation stuff!
    */
    
    DoCreate();
    boundForms = new Dictionary<string, Form>();
    boundInts = new Dictionary<string, int>();
    boundAttributes = new List<BoundAttribute>();

    boundIntList = new List<BoundInt>();
    boundFloatList = new List<BoundFloat>();
    boundFloatsList = new List<BoundFloats>();
    boundVector2List = new List<BoundVector2>();
    boundVector3List = new List<BoundVector3>();
    boundVector4List = new List<BoundVector4>();
    boundMatrixList = new List<BoundMatrix>();
    boundTextureList = new List<BoundTexture>();
    boundBufferList = new List<BoundBuffer>();
    FindKernel();
    GetNumThreads();
  
    count = numThreads;
    primaryForm = null;
    
    active = false;

  }

  public void Set(Form newForm ){
    BindPrimaryForm("_VertBuffer", newForm );
    oForm = primaryForm;
    inspectorForm = primaryForm;
    GetNumGroups();
    if( _buffer != null ){ _buffer.Release(); }
    _buffer = new ComputeBuffer((int)numGroups, 8 * sizeof(float));
    values = new float[numGroups*8];
    _buffer.SetData(values);
    active = true;

    data.BindRayData(this);
  }


  public void Unset(){
    if( _buffer  != null ) _buffer.Release();
    oForm = null;
    primaryForm = null;
    inspectorForm = null;
    closestID = -1;
    closestDist = 10000000;
    closestTime = 1000000;
    closest = Vector3.one * 1000000;
  }

  public override void _SetInternal(){

    inspectorForm = primaryForm;
    tmpActive = active;
    
    if( active ){
    
      if( primaryForm == null  || (data.inputEvents.Down < .5f && !alwaysCheck)  ){ 
        active = false; 
      }else{
        shader.SetFloat("_Time", Time.time);
        shader.SetFloat("_Delta", Time.deltaTime);
        shader.SetBuffer(kernel, "_OutBuffer" , _buffer );///, Time.deltaTime);
        shader.SetInt( "_OutBuffer_COUNT" , (int)count );///, Time.deltaTime);
      }
  

    }
  }

  public override void WhileLiving( float v ){
    
  }


  public void GetAllData(float[] values){
    closest = Vector3.one * 100000;
    closestID = -1;
    Vector3 v;
    Vector3 p;
    float id;
    float d;
    for( int i = 0; i < numGroups; i++ ){
      v = new Vector3( values[i*8+0]
                     , values[i*8+1]
                     , values[i*8+2] );

      id = values[i*8+3];
      p = new Vector3( values[i*8+4]
                     , values[i*8+5]
                     , values[i*8+6] );
      d = values[i*8+7];

      if( v.magnitude < closest.magnitude ){
        closest = v;
        closestID = id;
        closestPos = v;
        closestDist = d;
      }
    }

    value = new Vector4( closest.x , closest.y , closest.z , closestID );
  }
  public override void AfterDispatch(){

    oClosest = closest;
    oClosestID = closestID;

    if( active ){

      numberGroups = numGroups;

        _buffer.GetData(values);
        GetAllData(values);
      
    }
    
    if( closestID != oClosestID ){
      closestTime = Time.time;
    }
    active = tmpActive;

  }

}


