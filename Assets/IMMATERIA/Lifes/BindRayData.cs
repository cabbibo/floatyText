using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BindRayData : Binder
{

  public override void Bind(){
    data.BindRayData( toBind );
  }
}
