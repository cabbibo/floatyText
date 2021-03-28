 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class God : Cycle {

public bool AllInEditMode;
private static God _instance;
  
public bool started;

public bool godPause;
public bool dontDoIt;

public List<Cycle> _cycles;
public List<Form> forms;
public List<Life> lifes;
public List<Binder> binders;
public List<GameObject> objects;

public override void Create(){
    Application.runInBackground = true;
   if( _cycles == null ){ 
        _cycles = new List<Cycle>(); 
    }
    if( forms == null ){ 
        forms = new List<Form>(); 
    }

    if( lifes == null ){ 
        lifes = new List<Life>(); 
    }

    if( binders == null ){ 
        binders = new List<Binder>(); 
    }

      if( objects == null ){ 
        objects = new List<GameObject>(); 
    }

    _cycles.Clear();
    lifes.Clear();
    forms.Clear();
    binders.Clear();
    objects.Clear();



// currently not doing any of the 
// simulation on unity physics side so might as well ignore!
 //Physics.autoSimulation = false;


    if( data != null ){
        SafePrepend(data);
    }else{
        print("DUDE WHERE'S MY DATA");
    }

   //Application.targetFrameRate = 60;


}

public override void OnGestate(){
    GetCycleInfo( this );
}
public override void OnBirthed(){
}

public void GetCycleInfo( Cycle cycle ){


    bool newGO = true;
    foreach( GameObject o in objects ){
        if( o == cycle.gameObject ){
             newGO = false; 
        }
    }
    if( newGO ){ objects.Add( cycle.gameObject ); }

    _cycles.Add( cycle );
    
    if( cycle is Form ){
        forms.Add( (Form)cycle );

       //if(((Form)cycle).alwaysRemake ){
       // EditorGUIUtility.PingObject(cycle.gameObject);
       //    print( cycle.gameObject.name );
       //}
    }
    if( cycle is Life ){
        lifes.Add( (Life)cycle );
    }

    if( cycle is Binder ){
        binders.Add( (Binder)cycle );
    }

    foreach( Cycle c in cycle.Cycles ){
        GetCycleInfo( c );
    }

}

public void SaveAllForms(){

    // No longer redeleting everything
    /*foreach( Form f  in forms ){

     if( Saveable.Check(f.saveName)){
        Saveable.Delete(f.saveName);
     }
        
    }

    Saveable.ClearNames();*/

    foreach( Form f in forms ){
        //f.saveName = Saveable.GetSafeName();
        Saveable.Save(f);
    }
}


public void FullRebuild(){

    foreach( Form f  in forms ){  
     if( Saveable.Check(f.saveName)){
        Saveable.Delete(f.saveName);
     } 
    new WaitForSeconds(3);

     f.alwaysRemake = true;
    }

    Saveable.DeleteAll();
    Reset();
    OnDisable();
    OnEnable();
  
  
    Saveable.ClearNames();

    foreach( Form f in forms ){
        f.saveName = Saveable.GetSafeName();
        Saveable.Save(f);
        f.alwaysRemake = false;
    
    }

    DebugThis("" +Saveable.CheckIfAllNamesSafe());

     
}



public void LateUpdate(){

    if(!godPause){

    if( started == false ){ 
        _OnLive(); 
        started = true;
    }
    
    if( birthing ){ _WhileBirthing(1);}
    if( living ){ _WhileLiving(1); }
    if( dying ){ _WhileDying(1); }

    if( created ){ _WhileDebug(); }
}

}




public void OnEnable(){




    started = false;


    if( _instance == null ){ _instance = this; }

    #if UNITY_EDITOR 
        EditorApplication.update += Always;
        if(!dontDoIt){
             Reset();
            _Destroy(); 
            _Create(); 
            _OnGestate();
            _OnGestated();
            _OnBirth(); 
            _OnBirthed();
        }

    #else

    

    print("god enambles");


    if( Application.isPlaying ){
    
        Reset();
        _Destroy(); 
        _Create(); 
        _OnGestate();
        _OnGestated();
        _OnBirth(); 
        _OnBirthed();

    }

    #endif


}



public void OnDisable(){


    //print("god disabblee");
    #if UNITY_EDITOR 
        EditorApplication.update -= Always;
        _Destroy();   

    #else
     
    if( Application.isPlaying ){
        _Destroy();   
    }
    #endif
}


 
void Always(){    
  #if UNITY_EDITOR 
  if( AllInEditMode ){
    if(!godPause) EditorApplication.QueuePlayerLoopUpdate();
  }
  #endif
}





public void Rebuild(){
    Reset();
    OnDisable();
    OnEnable();
}



}