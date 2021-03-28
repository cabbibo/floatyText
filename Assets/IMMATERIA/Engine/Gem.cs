using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gem : Cycle
{
 
  public void Live(){
    print("IM LIVING");

    Reset();
    _Destroy(); 
    _Create(); 
    _OnGestate();
    _OnGestated();
    _OnBirth(); 
    _OnBirthed();
    _OnLive(); 
    _Activate();

  }


  public void Die(){
    _Deactivate();
    _Destroy();
  }

}
