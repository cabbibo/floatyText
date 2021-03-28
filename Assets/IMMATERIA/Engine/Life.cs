﻿using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using System;
using UnityEngine;
using UnityEditor;//.EditorGUI;

public class Life : Cycle {

  // automatically turns itself off
  public bool selfStop;

  [HideInInspector] public string primaryName;
  [HideInInspector] public Form primaryForm;
  
  public ComputeShader shader;
  public string kernelName;
  public int countMultiplier = 1;

  [HideInInspector] public int kernel;
  [HideInInspector] public float executionTime;

  public Dictionary<string, Form> boundForms;
  public Dictionary<string, int> boundInts;

  protected bool allBuffersSet;
  protected int numGroups;
  protected uint numThreads;

  public struct BoundAttribute {
    public string nameInShader;
    
   //public Type valType;
    //public delegate 

    public string attributeName;
    public FieldInfo info;
    public System.Object boundObject;
  }


  public struct BoundInt{
    public string nameInShader;
    public Func<int> lambda;
  }

  public struct BoundFloat{
    public string nameInShader;
    public Func<float> lambda;
  }

  public struct BoundFloats{
    public string nameInShader;
    public Func<float[]> lambda;
  }


   public struct BoundVector2{
    public string nameInShader;
    public Func<Vector2> lambda;
  }

  public struct BoundVector3{
    public string nameInShader;
    public Func<Vector3> lambda;
  }

  public struct BoundVector4{
    public string nameInShader;
    public Func<Vector4> lambda;
  }

  public struct BoundMatrix{
    public string nameInShader;
    public Func<Matrix4x4> lambda;
  }

  public struct BoundTexture{
    public string nameInShader;
    public Func<Texture> lambda;
  }

  public struct BoundBuffer{
    public string nameInShader;
    public Func<ComputeBuffer> lambda;
  }


  public List<BoundInt>     boundIntList;
  public List<BoundFloat>   boundFloatList;
  public List<BoundFloats>  boundFloatsList;
  public List<BoundVector2> boundVector2List;
  public List<BoundVector3> boundVector3List;
  public List<BoundVector4> boundVector4List;
  public List<BoundMatrix>  boundMatrixList;
  public List<BoundTexture> boundTextureList;
  public List<BoundBuffer>  boundBufferList;
  






  public List<BoundAttribute> boundAttributes;
 // public List<BoundFloats> b


  public delegate void SetValues(ComputeShader shader, int kernel);
  public event SetValues OnSetValues;

  public override void _Create(){

    //DebugThis("Creatio");
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
  }

  public virtual void FindKernel(){
    //DebugThis("TEST");try
        try{
            // Do something that can throw an exception

          kernel = shader.FindKernel( kernelName );
        }
        catch (Exception e)
        {
            DebugThis("Your Kernel name couldn't be found");
               #if UNITY_EDITOR
                EditorGUIUtility.PingObject(this.gameObject);
                #endif
        }
  }

  public virtual void GetNumThreads(){
    uint y; uint z;
    shader.GetKernelThreadGroupSizes(kernel, out numThreads , out y, out z);
  }

  public virtual void GetNumGroups(){

    //print(primaryForm);
    numGroups = (primaryForm.count*countMultiplier+((int)numThreads-1))/(int)numThreads;
  }
 
  public void BindForm( string name , Form form ){
   //print(name);
   //print(form);
   //print(boundForms);
    boundForms.Add( name ,form );
  }

  public void RebindForm(string name , Form form ){
  if( boundForms[name] ){
    boundForms[name] = form;
  }else{
    print("BORKEN no form reset");
  }
  }

    public void RebindPrimaryForm(string name , Form form ){

      if( primaryForm ){

          primaryForm = form;
          primaryName = name;
    }else{
//      print("no primary form to reset");
      primaryForm = form;
      primaryName = name;
    }
  }

   public void BindInt( string name , int form ){
    boundInts.Add( name ,form );
  }

  public void BindPrimaryForm(string name , Form form){
    primaryForm = form;
    primaryName = name;
  }

  public override void _WhileLiving( float v ){
    if( active ){ Live(); }
    if( selfStop ){ active = false; }
  }

  public void Live(){


    
//    DebugThis("LIVING");

    if( OnSetValues != null ){ OnSetValues(shader,kernel); }
    
    GetNumGroups();
    SetShaderValues();
    BindAttributes();

    // set this true every frame, 
    // and allow each buffer to make 
    // untrue as needed
    allBuffersSet = true;

    _SetInternal();

    shader.SetInt("_Count", primaryForm.count );


    foreach(KeyValuePair<string,Form> form in boundForms){
      if( debug == true ){ DebugThis( form.Key );}
      SetBuffer(form.Key , form.Value);
    }

    if( debug == true ){ DebugThis( "PRIMARY FORM" + primaryForm );;}
    SetBuffer( primaryName , primaryForm );


    // This
    foreach(KeyValuePair<string,int> form in boundInts){
      shader.SetInt(form.Key , form.Value);
    }


    // if its still true than we can dispatch
    if ( allBuffersSet ){
      if( debug ) print( "name : " + kernelName + " Num groups : " + numGroups );
      shader.Dispatch( kernel,numGroups ,1,1);
    }

    AfterDispatch();

  }

  public virtual void _SetInternal(){    
    shader.SetFloat("_Time", Time.time);
    shader.SetFloat("_Delta", Time.deltaTime);
    shader.SetFloat("_DT", Time.deltaTime);
  }


  public virtual void AfterDispatch(){}
  public virtual void SetShaderValues(){}

  private void SetBuffer(string name , Form form){
    //print(form);
    //print(name);


    if( form != null){


      if( form._buffer != null ){
        
        
        shader.SetBuffer( kernel , name , form._buffer);
        shader.SetInt(name+"_COUNT" , form.count );
        if( debug == true ){ DebugThis("Bound Form : " + form.gameObject );}
        if( debug == true ){ DebugThis("Count : " + form.count );}
      }else{
        allBuffersSet = false;
        DebugThis("YOUR BUFFER : " + name +  " IS NULL!");
      }
    }else{
 //     print("WAHT YR FORM IS NULL");
//      print( name );
    }
  }

  /*public void BindAttribute( string nameInShader, string attributeName , System.Object obj ){
   
    BoundAttribute a = new BoundAttribute();
    a.nameInShader = nameInShader;
    a.attributeName = attributeName;
    a.boundObject = obj;
    a.info = obj.GetType().GetField(attributeName);


//    a.lamba = () -> obj.GetMethod
    bool replaced = false;
    int id = 0;

//    print( boundAttributes );

    foreach( BoundAttribute ba in boundAttributes){

      if( ba.nameInShader == nameInShader ){
        boundAttributes[id] = a;
        //DebugThis( ba.nameInShader + " is being rebound" );
        replaced = true;
        break;
      }

      id ++;
    }

    if( replaced == false ){  boundAttributes.Add(a); }
  
  }*/



   public void BindInt(string nameInShader, Func<int> lambda ){

    BoundInt attribute = new BoundInt();
    attribute.nameInShader = nameInShader;
    attribute.lambda = lambda;

    bool replaced = false;
    int id = 0;
    foreach( BoundInt ba in boundIntList){

      if( ba.nameInShader == nameInShader ){
        boundIntList[id] =  attribute;
        //DebugThis( ba.nameInShader + " is being rebound" );
        replaced = true;
        break;
      }

      id ++;
    }

    if( replaced == false ){  boundIntList.Add( attribute); }

  }
  public void BindFloat(string nameInShader, Func<float> lambda ){

    BoundFloat attribute = new BoundFloat();
    attribute.nameInShader = nameInShader;
    attribute.lambda = lambda;

    bool replaced = false;
    int id = 0;
    foreach( BoundFloat ba in boundFloatList){

      if( ba.nameInShader == nameInShader ){
        boundFloatList[id] =  attribute;
        //DebugThis( ba.nameInShader + " is being rebound" );
        replaced = true;
        break;
      }

      id ++;
    }

    if( replaced == false ){  boundFloatList.Add( attribute); }

  }


  public void BindFloats(string nameInShader, Func<float[]> lambda ){

    BoundFloats attribute = new BoundFloats();
    attribute.nameInShader = nameInShader;
    attribute.lambda = lambda;

    bool replaced = false;
    int id = 0;
    foreach( BoundFloats ba in boundFloatsList){

      if( ba.nameInShader == nameInShader ){
        boundFloatsList[id] =  attribute;
        //DebugThis( ba.nameInShader + " is being rebound" );
        replaced = true;
        break;
      }

      id ++;
    }

    if( replaced == false ){  boundFloatsList.Add( attribute); }

  }



  public void BindVector2(string nameInShader, Func<Vector2> lambda ){

    BoundVector2 attribute = new BoundVector2();
    attribute.nameInShader = nameInShader;
    attribute.lambda = lambda;

    bool replaced = false;
    int id = 0;
    foreach( BoundVector2 ba in boundVector2List){

      if( ba.nameInShader == nameInShader ){
        boundVector2List[id] =  attribute;
        //DebugThis( ba.nameInShader + " is being rebound" );
        replaced = true;
        break;
      }

      id ++;
    }

    if( replaced == false ){  boundVector2List.Add( attribute); }

  }



    public void BindVector3(string nameInShader, Func<Vector3> lambda ){

    BoundVector3 attribute = new BoundVector3();
    attribute.nameInShader = nameInShader;
    attribute.lambda = lambda;

    bool replaced = false;
    int id = 0;


    foreach( BoundVector3 ba in boundVector3List){

      if( ba.nameInShader == nameInShader ){
        boundVector3List[id] =  attribute;
        //DebugThis( ba.nameInShader + " is being rebound" );
        replaced = true;
        break;
      }

      id ++;
    }

    if( replaced == false ){  boundVector3List.Add( attribute); }

  }

  public void BindVector4(string nameInShader, Func<Vector4> lambda ){

    BoundVector4 attribute = new BoundVector4();
    attribute.nameInShader = nameInShader;
    attribute.lambda = lambda;

    bool replaced = false;
    int id = 0;
    foreach( BoundVector4 ba in boundVector4List){

      if( ba.nameInShader == nameInShader ){
        boundVector4List[id] =  attribute;
        //DebugThis( ba.nameInShader + " is being rebound" );
        replaced = true;
        break;
      }

      id ++;
    }

    if( replaced == false ){  boundVector4List.Add( attribute); }

  }

    public void BindMatrix(string nameInShader, Func<Matrix4x4> lambda ){

    BoundMatrix attribute = new BoundMatrix();
    attribute.nameInShader = nameInShader;
    attribute.lambda = lambda;

    bool replaced = false;
    int id = 0;
    foreach( BoundMatrix ba in boundMatrixList){

      if( ba.nameInShader == nameInShader ){
        boundMatrixList[id] =  attribute;
        //DebugThis( ba.nameInShader + " is being rebound" );
        replaced = true;
        break;
      }

      id ++;
    }

    if( replaced == false ){  boundMatrixList.Add( attribute); }

  }


  public void BindTexture(string nameInShader, Func<Texture> lambda ){

    BoundTexture attribute = new BoundTexture();
    attribute.nameInShader = nameInShader;
    attribute.lambda = lambda;

    bool replaced = false;
    int id = 0;
    foreach( BoundTexture ba in boundTextureList){

      if( ba.nameInShader == nameInShader ){
        boundTextureList[id] =  attribute;
        //DebugThis( ba.nameInShader + " is being rebound" );
        replaced = true;
        break;
      }

      id ++;
    }

    if( replaced == false ){  boundTextureList.Add( attribute); }

  }


    public void BindBuffer(string nameInShader, Func<ComputeBuffer> lambda ){

    BoundBuffer attribute = new BoundBuffer();
    attribute.nameInShader = nameInShader;
    attribute.lambda = lambda;

    bool replaced = false;
    int id = 0;
    foreach( BoundBuffer ba in boundBufferList){

      if( ba.nameInShader == nameInShader ){
        boundBufferList[id] =  attribute;
        //DebugThis( ba.nameInShader + " is being rebound" );
        replaced = true;
        break;
      }

      id ++;
    }

    if( replaced == false ){  boundBufferList.Add( attribute); }

  }

  /*
  public void BindAttributes(){
    
    foreach(  BoundAttribute b in boundAttributes ){


      string s = "";
      if( debug == true ){
        s +="UNIFORM : "  + b.nameInShader;
      }

      if( b.info == null ){
        print("------------------------------");
        print("THIS ATTRIBUTE DOESN'T EXIST: ");
        print("Bound Object: " + b.boundObject);
        print("Attribute Name: " + b.attributeName);
        print("Name In Shader: " + b.nameInShader);
        print("I AM ON GAME OBJECT: " + this);
        print("------------------------------");
      }

      if( b.info.FieldType == typeof(float) ){
        float value = (float)b.info.GetValue(b.boundObject);
        if( debug == true ){ print( s + " || VALUE : " + value);}
        shader.SetFloat(b.nameInShader,value);
      }else if(b.info.FieldType == typeof(float[]) ){
        float[] value = (float[])b.info.GetValue(b.boundObject);
        if( debug == true ){ print( s + " || VALUE : " + value);}
        shader.SetFloats(b.nameInShader,value);
      }else if( b.info.FieldType == typeof(int) ){
        int value = (int)b.info.GetValue(b.boundObject);
        if( debug == true ){ print( s + " || VALUE : " + value);}
        shader.SetInt(b.nameInShader,value);
      }else if( b.info.FieldType == typeof(Vector3)){
        Vector3 value = (Vector3)b.info.GetValue(b.boundObject);
        if( debug == true ){ print( s + " || VALUE : " + value);}
        shader.SetVector(b.nameInShader,value);
      }else if( b.info.FieldType == typeof(Vector4)){
        Vector4 value = (Vector4)b.info.GetValue(b.boundObject);
        if( debug == true ){ print( s + " || VALUE : " + value);}
        shader.SetVector(b.nameInShader,value);
      }else if( b.info.FieldType == typeof(Matrix4x4)){
        Matrix4x4 value = (Matrix4x4)b.info.GetValue(b.boundObject);
        if( debug == true ){ print( s + " || VALUE : " + value);}
        shader.SetMatrix(b.nameInShader,value);
      }else if( b.info.FieldType == typeof(Texture) ){
        Texture value = (Texture)b.info.GetValue(b.boundObject);
        if( debug == true ){ print( s + " || VALUE : " + value);}
        shader.SetTexture(kernel,b.nameInShader,value);
      }else if( b.info.FieldType == typeof(Texture2D) ){
        Texture2D value = (Texture2D)b.info.GetValue(b.boundObject);
        if( debug == true ){ print( s + " || VALUE : " + value);}
        shader.SetTexture(kernel,b.nameInShader,value);
      }else if(b.info.FieldType == typeof(ComputeBuffer) ){
        ComputeBuffer value = (ComputeBuffer)b.info.GetValue(b.boundObject);
        if( debug == true ){ print( s + " || VALUE : " + value);}
        shader.SetBuffer(kernel,b.nameInShader,value);
      }

    }


  }*/

  public void BindAttributes(){
    
    foreach( BoundInt b in boundIntList ){
      if( debug == true ){ DebugThis( b.nameInShader  + " : " + b.lambda() );}
      shader.SetInt(b.nameInShader,b.lambda());
    }

    foreach( BoundFloat b in boundFloatList ){
      if( debug == true ){ DebugThis( b.nameInShader  + " : " + b.lambda() );}
      shader.SetFloat(b.nameInShader,b.lambda());
    }

    foreach( BoundFloats b in boundFloatsList ){
      if( debug == true ){ DebugThis( b.nameInShader  + " : " + b.lambda() );}
      shader.SetFloats(b.nameInShader,b.lambda());
    }

    foreach( BoundVector2 b in boundVector2List ){
      if( debug == true ){ DebugThis( b.nameInShader  + " : " + b.lambda() );}
      shader.SetVector(b.nameInShader,b.lambda());
    }

    foreach( BoundVector3 b in boundVector3List ){
      if( debug == true ){ DebugThis( b.nameInShader  + " : " + b.lambda() );}
      shader.SetVector(b.nameInShader,b.lambda());
    }

    foreach( BoundVector4 b in boundVector4List ){
      if( debug == true ){ DebugThis( b.nameInShader  + " : " + b.lambda() );}
      shader.SetVector(b.nameInShader,b.lambda());
    }

    foreach( BoundMatrix b in boundMatrixList ){
      if( debug == true ){ DebugThis( b.nameInShader  + " : " + b.lambda() );}
      shader.SetMatrix(b.nameInShader,b.lambda());
    }

    foreach( BoundTexture b in boundTextureList ){
      if( debug == true ){ DebugThis( b.nameInShader  + " : " + b.lambda() );}
      shader.SetTexture(kernel,b.nameInShader,b.lambda());
    }

    foreach( BoundBuffer b in boundBufferList ){
      if( debug == true ){ DebugThis( b.nameInShader  + " : " + b.lambda() );}
      shader.SetBuffer(kernel,b.nameInShader,b.lambda());
    }

    if( debug == true ){ DebugThis("DOI:NGS"); }

  }

  public void YOLO(){
    active = true;
    _WhileLiving(1);
    active = false;
  }

}
