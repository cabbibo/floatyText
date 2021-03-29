using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Rendering;
using UnityEditor;

[ExecuteAlways]
public class TextParticles : MonoBehaviour{


  public Material material;

  public bool updateInEditMode;

  public TextAnchor anchor;


  public Vector3 emitterPosition;


  public ComputeShader shader;

  private int setAnchorKernel;
  private int setGlyphKernel;
  private int setPageKernel;

  private int simulateKernel;
  private int transferKernel;



private uint setGlyphThreadSize;
private uint setAnchorThreadSize;
private uint setPageThreadSize;
private uint simulateThreadSize;
private uint transferThreadSize;


  private float pageStart;

  public float radius;
  public float scale;

  public int currentMax;
  public int currentMin;



  public int maxParticleCount;
  public ComputeBuffer _particles;
  public ComputeBuffer _verts;
  public ComputeBuffer _tris;


  public bool debug;
  private int structSize;



  public void ReleaseBuffers(){
    
    if( _particles != null ){
      _particles.Release();
    }

    if( _verts != null ){
      _verts.Release();
    }

    if( _tris != null ){
      _tris.Release();
    }

  }


  void OnEnable(){

        // MAKES sure we run smoothly in edit mode
        #if UNITY_EDITOR 
          EditorApplication.update += Always;
        #endif

        currentMin = 0;
        currentMax = 0;
        structSize = 20;


        MakeBuffers();
        GetKernels();
        Set(anchor);
  }

  void OnDisable(){

    ReleaseBuffers();
     // MAKES sure we run smoothly in edit mode
        #if UNITY_EDITOR 
          EditorApplication.update -= Always;
        #endif
  }

  // Make it so we quee the player loop
  // to make sure we can run the simulation 
  // in edit mode
   void Always(){
        #if UNITY_EDITOR 
          if(updateInEditMode) EditorApplication.QueuePlayerLoopUpdate();
        #endif
    }

  void Update(){
 
    SetValues();
    Simulate();
    Transfer();
    Render();

    if( debug ){
      WhileDebug();
    }




  }


  // Sets the glyphs aka what letter is which
  void SetGlyphs(){
     DispatchShader((currentMax-currentMin)*4, setGlyphKernel, setGlyphThreadSize );
  }

  //Sets the anchors  aka target locations for each letter
  void SetAnchor(){
    DispatchShader(currentMax-currentMin, setAnchorKernel, setAnchorThreadSize );
  }


  //does the actual simulation of the letters
  void Simulate(){
    DispatchShader( maxParticleCount , simulateKernel, simulateThreadSize  );
  }

  // Takes all of our values and makes a mesh out of it
  void Transfer(){
    DispatchShader( maxParticleCount * 4 , transferKernel  , transferThreadSize );
  }


  //Running a shader with a specific count
  void DispatchShader( int count , int kernel , uint threadSize ){
    shader.Dispatch( kernel , numGroupsThreadSize(threadSize,count) ,1,1);
  }

  // Geting number of threads out of our compute shader
  uint numThreads( int kernel){
    uint y; uint z; uint threads;
    shader.GetKernelThreadGroupSizes(kernel, out threads , out y, out z);
    return threads;
  }

  int numGroups(int kernel , int count){
    int threads = (int)numThreads(kernel);
    return (count+((int)threads-1))/(int)threads;
  }

  int numGroupsThreadSize(uint threads , int count){
    return (count+((int)threads-1))/(int)threads;
  }


  public void Set(TextAnchor t){
  
    currentMin = currentMax;
    currentMax = currentMin + t.count; 
    
    anchor = t;
    scale = t.scale;

    SetValues();

    SetAnchor();
    SetGlyphs();

  }



  public void Release(){
    currentMin = currentMax;
  }


  public void MakeBuffers(){
    
    ReleaseBuffers();
  
    _particles = new ComputeBuffer(maxParticleCount, structSize * sizeof(float));
    _verts = new ComputeBuffer(maxParticleCount * 4, structSize * sizeof(float));
    _tris = new ComputeBuffer(maxParticleCount * 3 * 2 , sizeof(int));

    int[] values = new int[maxParticleCount * 3 * 2];
    int index = 0;

    // 1-0
    // |/|
    // 3-2
    for( int i = 0; i < maxParticleCount; i++ ){
        int bID = i * 4;
        values[ index ++ ] = bID + 0;
        values[ index ++ ] = bID + 1;
        values[ index ++ ] = bID + 3;
        values[ index ++ ] = bID + 0;
        values[ index ++ ] = bID + 3;
        values[ index ++ ] = bID + 2;
    }
    _tris.SetData(values);
  

  }

  public void GetKernels(){
    setGlyphKernel = shader.FindKernel( "SetGlyph" );
    setAnchorKernel = shader.FindKernel( "SetAnchor" );
    setPageKernel = shader.FindKernel( "SetPageAtEmitPos" );
    simulateKernel = shader.FindKernel( "SimulationTest" );
    transferKernel = shader.FindKernel("Transfer");


    setGlyphThreadSize = numThreads( setGlyphKernel );
    setAnchorThreadSize = numThreads( setAnchorKernel );
    setPageThreadSize = numThreads( setPageKernel );
    simulateThreadSize = numThreads( simulateKernel );
    transferThreadSize = numThreads( transferKernel );
  }


  public void SetValues(){


    if( anchor != null && anchor._buffer != null && _verts != null && _tris != null && _particles !=null ){

    shader.SetBuffer( setGlyphKernel , "_TransferBuffer",_verts);
    shader.SetBuffer( setGlyphKernel , "_VertBuffer",_particles);
    shader.SetBuffer( setGlyphKernel , "_AnchorBuffer",anchor._buffer);

    shader.SetBuffer( setAnchorKernel , "_VertBuffer",_particles);
    shader.SetBuffer( setAnchorKernel , "_AnchorBuffer",anchor._buffer);

    shader.SetBuffer( setPageKernel , "_VertBuffer",_particles);
    shader.SetBuffer( setPageKernel , "_AnchorBuffer",anchor._buffer);


    
    shader.SetBuffer( simulateKernel , "_TransferBuffer",_verts);
    shader.SetBuffer( simulateKernel , "_VertBuffer",_particles);
    shader.SetBuffer( simulateKernel , "_AnchorBuffer",anchor._buffer);

    
    shader.SetBuffer( transferKernel , "_TransferBuffer",_verts);
    shader.SetBuffer( transferKernel , "_VertBuffer",_particles);
    shader.SetBuffer( transferKernel , "_AnchorBuffer",anchor._buffer);

    shader.SetVector("_FrameTopLeft", anchor.topLeft );
    shader.SetFloat("_FrameWidth", anchor.width );
    shader.SetFloat("_FrameHeight", anchor.height );
    shader.SetVector("_FrameUp", anchor.up );
    shader.SetVector("_FrameRight", anchor.right );

    shader.SetFloat( "_Radius" , radius);
    shader.SetFloat( "_Scale" , scale);
    shader.SetInt("_BaseID" , currentMin);
    shader.SetInt("_TipID" , currentMax);

    shader.SetInt("_FullCount", maxParticleCount);
    shader.SetInt("_AnchorCount" , currentMax-currentMin);
    

    shader.SetFloat(  "_FontWidth" , anchor.fontData.width );
    shader.SetFloat(  "_FontHeight" , anchor.fontData.height );
    shader.SetFloat(  "_FontSize" , anchor.fontData.size );


    shader.SetVector( "_CameraUp" , Camera.main.transform.up );
    shader.SetVector( "_CameraRight" , Camera.main.transform.right );
    shader.SetVector( "_CameraForward" , Camera.main.transform.forward );


    
    shader.SetFloat("_Time", Time.time);
    shader.SetFloat("_Delta", Time.deltaTime);
    shader.SetFloat("_DT", Time.deltaTime);

    }else{
      print("we got a null");
    }


  }

  private MaterialPropertyBlock mpb;
  
  public void Render(){

      if (mpb == null) { mpb = new MaterialPropertyBlock(); }
      
        mpb.SetInt("_VertCount", maxParticleCount );
        mpb.SetBuffer("_VertBuffer", _verts );
        mpb.SetBuffer("_TriBuffer", _tris);

        Graphics.DrawProcedural(material, new Bounds(transform.position, Vector3.one * 1000), MeshTopology.Triangles, maxParticleCount * 3 * 2, 1, null, mpb, ShadowCastingMode.TwoSided, true, gameObject.layer);
    
  }




  public Material debugMaterial;
  private MaterialPropertyBlock debugMPB;
  void WhileDebug(){


    int whichCount = maxParticleCount * 4;
    ComputeBuffer  whichBuffer = _particles;


    if(debugMPB== null){ debugMPB = new MaterialPropertyBlock();}
    debugMPB.SetBuffer("_VertBuffer", whichBuffer);
    debugMPB.SetInt("_Count",whichCount);


  // Infinit bounds so its always drawn!
    Graphics.DrawProcedural(debugMaterial, new Bounds(transform.position, Vector3.one * 100000), MeshTopology.Triangles, whichCount * 3 * 2, 1, null, debugMPB, ShadowCastingMode.TwoSided, true, gameObject.layer);
  
  }

}
