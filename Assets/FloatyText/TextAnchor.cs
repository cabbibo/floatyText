using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
 using System.Xml;
 using System.Xml.Serialization;


[ExecuteAlways]
public class TextAnchor : MonoBehaviour{

  public string fontName;

  
  public bool alwaysReset;

  public bool debug;
  public ComputeBuffer _buffer;
  public int count;
  public int structSize;


  //public FontInfo font;
public class glyph{

  public int column;
  public int row;
  public int id;
  public char character;

  public float x;
  public float y;
  public float w;
  public float h;
  public float offsetX;
  public float offsetY;
  public float advance;

  public float left;
  public float top;

  public glyph( float l, float t , int i , Char vals ){
   
    left = l;
    top = t;

    x = vals.x;
    y = vals.y;

    w = vals.width;
    h = vals.height;

    offsetX = vals.originX;
    offsetY = vals.originY;

    advance = vals.advance;
    //column = c;
    //row = r;
    id = i;
  }
}


 [TextArea(15,20)]
public string text;
  
  public float characterSize=1;
  public float lineHeight =1;
  public float advance = 1;

  public List<glyph> glyphs;

  public float scale;


  [HideInInspector]
  public float scaledCharacterSize;
  
  [HideInInspector]
  public float scaledLineHeight;
  
  [HideInInspector]
  public float scaledAdvance;


  [HideInInspector]
  public float currentTextureVal;
  
  [HideInInspector]
  public float currentScaleOffset;
  
  [HideInInspector]
  public float currentHueOffset;
  
  [HideInInspector]
  public float currentSpecial;

 //public int row;
 //public int column;
 
  [HideInInspector]
  public float locationX;
  
  [HideInInspector]
  public float locationY;




  void OnEnable(){
    LoadFont();
    SetFrame();
    SetStructSize();
    SetCount();
    MakeBuffer();
    Embody();
  }

  void OnDisable(){
    ReleaseBuffer();
  }

  public void ReleaseBuffer(){
     if( _buffer != null ){
      _buffer.Release();
    }

  }

  public void MakeBuffer(){
    ReleaseBuffer();
    _buffer = new ComputeBuffer( count , structSize * sizeof(float));
  }
  
  public void Update(){ 

      if( alwaysReset ){
        SetFrame();

        int tmpCount = count;
        SetCount();
        
        if( tmpCount != count ){
          MakeBuffer();
        }

        Embody();
      }         

      if( debug ){
        WhileDebug();
      }
  }




  public class Char{
    public char character;
    public int x;
    public int y;
    public int width;
    public int height;
    public int originX;
    public int originY;
    public int advance;

    public Char( string text ){

      text = text.Replace(" ", "");
      text = text.Replace("{", "");
      text = text.Replace("}", "");
      text = text.Replace("\"", "");

      var pairs = text.Split(',');

      for( int j = 0; j < pairs.Length; j++ ){

          var propVal =  pairs[j].Split(':');

          if( propVal.Length == 2 ){

            if( propVal[0] == "x" ){
              this.x = int.Parse(propVal[1]);
            }

             if( propVal[0] == "y" ){
              this.y = int.Parse(propVal[1]);
            }

            if( propVal[0] == "originX" ){
              this.originX = int.Parse(propVal[1]);
            }

            if( propVal[0] == "originY" ){
              this.originY = int.Parse(propVal[1]);
            }

            if( propVal[0] == "width" ){
              this.width = int.Parse(propVal[1]);
            }

            if( propVal[0] == "height" ){
              this.height = int.Parse(propVal[1]);
            }

            if( propVal[0] == "advance" ){
              this.advance = int.Parse(propVal[1]);
            }
          }
      }
      

    }
  }



  /*

    This is for parsing the data you get from
    

  */

  public class FontData{
    public string name;
    public int size;
    public bool bold;
    public bool italic;
    public int width;
    public int height;

    
    
    public Dictionary<char,Char> characters;


    public FontData( string text ){

      
      string[] lines = text.Split('\n');

      this.characters = new Dictionary<char,Char>();

      for( int i = 0; i < lines.Length; i++ ){

        
       // print(i);
        var split = lines[i].Split(':');
        if( split.Length > 1 ){

          string property = split[0];
          string value = split[1];

          property = property.Replace("\"", "");
          property = property.Replace(" ", "");
          
          value = value.Replace("\"", "");
          value = value.Replace(" ", "");
          value = value.Replace(",", "");
        
          if(property == "name"){
            this.name = value;
          }

          if(property== "size"){
            this.size = int.Parse(value);
          }

          if(property=="bold"){
            this.bold = bool.Parse(value);
          }

          if(property=="italic"){
            this.italic = bool.Parse(value);
          }


          if(property=="height"){
            this.height = int.Parse(value);
          }

          if(property=="width"){
            this.width = int.Parse(value);
          }



          //print(property.ToCharArray().Length);
          
          if( property.ToCharArray().Length == 1  ){

            var pieces = lines[i].Split(new[]{':'},2);

            var character = pieces[0];
            character = character.Replace("\"", "");
            character = character.Replace(" ", "");

           // print(character.ToCharArray()[0] );
            //print(character.)
            
          
            this.characters.Add( character.ToCharArray()[0] , new Char(pieces[1]));
   
          }

        }
      }

      
    }

  }

  public FontData fontData;
  void LoadFont(){


     var textFile = Resources.Load<TextAsset>("TextData/" + fontName + "/font");



    fontData = new FontData( textFile.text );///JsonUtility.FromJson<FontData>( textFile.text );



    print(fontData.characters.ContainsKey('r'));

  }

  void SetStructSize(){ structSize = 20; }
  
 void SetCount(){

  
    currentSpecial       = 0;
    currentHueOffset     = 0;
    currentScaleOffset   = 1;
    currentTextureVal    = 0;

    scaledCharacterSize = scale  * characterSize;
    scaledLineHeight = scale * lineHeight;
    scaledAdvance = scale * advance;

      //print( "setting count");
   // scale = distance / 3;
    //row = 0;
    //column = 0;
    locationX = 0;
    locationY = 0;


   // print("SETTING COUNT + this : " + this.transform.parent);
    //scount = text.length
    glyphs = new List<glyph>();

    count = 0;


    
  glyphs.Clear();
  MakeGlyphs(text);





    
  
    //print(words[0]);

  }

  public void MakeGlyphs( string parsedText ){

    // split it into words
    string[] words = parsedText.Split(' ');
    
    int first = 0;
    foreach( string word in words ){
 
      
      // makes sure we skip the first space of the section
      if( first != 0 ){
        //column ++;
        locationX += scaledAdvance;
      }else{
        first=1;
      }


      char[] letters = word.ToCharArray();

 


 
      float newLine = 0;
      float wordWidth = 0;
      foreach( char c in letters ){ 
        if( c == '\n'){
        }else{
          if( fontData.characters.ContainsKey(c)){

            Char v1 = fontData.characters[c];
            wordWidth += ((float)v1.advance/fontData.size) * scaledAdvance;// * scale

          }
        }
      }

      if( locationX + wordWidth >= width ){
          //row ++;
          locationY += scaledLineHeight;
          locationX  = 0;
          //column = 0;
      }


      foreach( char c in letters ){ 

        if( c == '\n'){
          //row ++;
          locationY += scaledLineHeight;
          locationX  = 0;
          //column = 0;
        }else{

          if( fontData.characters.ContainsKey(c)){

            Char v1 = fontData.characters[c];


            // print( c );
            glyph g = new glyph(locationX,locationY,count,v1);
            glyphs.Add(g);

            locationX +=  ((float)v1.advance/fontData.size) * scaledAdvance;
            //column ++;
            count ++;

          }else{
            print( "CANT FIND: " + c );
            //print( row );
            //print( column );
          }
        }
      }




    }
    
  }
  

  public Vector3[] finalPositions; 

  void Embody(){


    finalPositions = new Vector3[count];

    float[] values = new float[count*structSize];
    int index = 0;
    Vector3 dir = (topRight - topLeft).normalized;
    Vector3 down = (bottomRight - topRight).normalized;
    

    Vector3 p;
    for( int i = 0; i < count; i ++ ){

      //print(glyphs[i]);
//      print(glyphs[i]);

      p = topLeft + dir * glyphs[i].left + down * glyphs[i].top;


    finalPositions[i] = p;
      // position
      values[ index ++ ] = p.x;
      values[ index ++ ] = p.y;
      values[ index ++ ] = p.z;

      // normal
      values[ index ++ ] = normal.x;
      values[ index ++ ] = normal.y;
      values[ index ++ ] = normal.z;

      //float[] gInfo = getTextCoordinates(glyphs[i].character);

      //Character Info
      values[ index ++ ] = glyphs[i].x;
      values[ index ++ ] = glyphs[i].y;

      values[ index ++ ] = glyphs[i].w;
      values[ index ++ ] = glyphs[i].h;
   
      // debug
      values[ index ++ ] = glyphs[i].offsetX;
      values[ index ++ ] = glyphs[i].offsetY;

      //LOCATION
      values[ index ++ ] =  glyphs[i].left;
      values[ index ++ ] =  glyphs[i].top;


      values[ index ++ ] = glyphs[i].w;
      values[ index ++ ] = glyphs[i].h;//(scaledPadding + glyphs[i].row * scaledLineHeight)/height;


      values[index++] = 0;//glyphs[i].textureVal;// Mathf.Floor(Random.Range(0,1.999f));
      values[index++] = 0;//glyphs[i].scaleOffset; //Random.Range(.8f,1.2f );
      values[index++] = 0;//glyphs[i].hueOffset;
      values[index++] = 0;//glyphs[i].special;

    }

    SetData( values );

  }


/*

  //TODO: Make with and height of letter, for later use
  float[] getTextCoordinates( char letter ){
    
    int  charCode = (int)letter;

    if( charCode == 8216 ){ charCode = 39; }
    if( charCode == 8217 ){ charCode = 39; }
    if( charCode == 8212 ){ charCode = 45; }



    float[] index = ubuntuMono.info[charCode];

    float[] newIndex = new float[index.Length];

    for( int i = 0; i< index.Length; i++ ){
      newIndex[i] = index[i] / 1024;
    }


    return newIndex;//new Vector4(left,top,width,height);

  }*/

  public Material debugMaterial;
  private MaterialPropertyBlock mpb;
  void WhileDebug(){

    
    debugMaterial.SetPass(0);
    debugMaterial.SetBuffer("_VertBuffer", _buffer);
    debugMaterial.SetInt("_Count",count);
    Graphics.DrawProceduralNow(MeshTopology.Triangles, count * 3 * 2 );


    mpb = new MaterialPropertyBlock();
    mpb.SetBuffer("_VertBuffer", _buffer);
    mpb.SetInt("_Count",count);


  // Infinit bounds so its always drawn!
    Graphics.DrawProcedural(debugMaterial, new Bounds(transform.position, Vector3.one * 100000), MeshTopology.Triangles, count * 3 * 2, 1, null, mpb, ShadowCastingMode.TwoSided, true, gameObject.layer);
  
  }




  public float distance;

  public float borderLeft;
  public float borderRight;
  public float borderTop;
  public float borderBottom;

  public bool constant;

  private float _ratio;

  //public LineRenderer borderLine;

  public Vector3 bottomLeft;
  public Vector3 bottomRight;
  public Vector3 topLeft;
  public Vector3 topRight;


  public Vector3 fullBottomLeft;
  public Vector3 fullBottomRight;
  public Vector3 fullTopLeft;
  public Vector3 fullTopRight;


  public Vector3 center;

  public float width;
  public float height;
  public Vector3 normal;
  public Vector3 up;
  public Vector3 right;

  
  public void SetFrame(){

    _ratio = (float)Screen.width / (float)Screen.height;

    Camera cam = Camera.main;

    Vector3  tmpP = cam.transform.position;
    Quaternion tmpR = cam.transform.rotation;

    cam.transform.position = transform.position;
    cam.transform.rotation = transform.rotation;

    bottomLeft = Camera.main.ViewportToWorldPoint(new Vector3( borderLeft ,_ratio *borderBottom,distance));  
    bottomRight = Camera.main.ViewportToWorldPoint(new Vector3(1- borderRight,_ratio *borderBottom ,distance));
    topLeft = Camera.main.ViewportToWorldPoint(new Vector3(borderLeft,1-_ratio * borderTop,distance));
    topRight = Camera.main.ViewportToWorldPoint(new Vector3(1-borderRight,1-_ratio * borderTop,distance));



    
    fullBottomLeft = Camera.main.ViewportToWorldPoint(new Vector3( 0 ,0,distance));  
   fullBottomRight = Camera.main.ViewportToWorldPoint(new Vector3(1,0,distance));
    fullTopLeft = Camera.main.ViewportToWorldPoint(new Vector3(0,1,distance));
    fullTopRight = Camera.main.ViewportToWorldPoint(new Vector3(1,1,distance));


    center = Camera.main.ViewportToWorldPoint(new Vector3( .5f , .5f , distance )); 

    normal = transform.forward;


    up = -(bottomLeft - topLeft).normalized;
    right = -(bottomLeft - bottomRight).normalized;


    width = (bottomLeft - bottomRight).magnitude;
    height = (bottomLeft - topLeft).magnitude;
    
    cam.transform.position = tmpP;
    cam.transform.rotation = tmpR;

  }


  public void SetData( float[] values ){ 
    if( _buffer != null ){
    _buffer.SetData( values );
    }else{
      print("NO BUFF");
      MakeBuffer();
    }
  }

 void OnDrawGizmosSelected()
    {
        // Draw a yellow sphere at the transform's position
    
        Gizmos.color = Color.red;
         Gizmos.DrawLine(transform.position, fullBottomLeft);
        Gizmos.DrawLine(transform.position , fullBottomRight);
        Gizmos.DrawLine(transform.position , fullTopLeft);
        Gizmos.DrawLine(transform.position , fullTopRight);

        
        Gizmos.DrawLine(fullBottomLeft, fullTopLeft);
        Gizmos.DrawLine(fullTopLeft, fullTopRight);
        Gizmos.DrawLine(fullTopRight, fullBottomRight);
        Gizmos.DrawLine(fullBottomRight, fullBottomLeft);
    Gizmos.color = Color.yellow;
   

        
        Gizmos.DrawLine(bottomLeft, topLeft);
        Gizmos.DrawLine(topLeft, topRight);
        Gizmos.DrawLine(topRight, bottomRight);
        Gizmos.DrawLine(bottomRight, bottomLeft);


   Gizmos.color = Color.white;
        for( int i=0; i < finalPositions.Length; i++ ){
          Gizmos.DrawWireCube( finalPositions[i], Vector3.one * .01f);
        }
    }
}


