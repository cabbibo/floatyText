using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnchorSwapper : Cycle
{

  public TextParticles text;
  public TextAnchor[] anchors;

  public float timing = 0;
  public int currentAnchor = 0;

  public override void WhileLiving( float v ){
    timing ++;

    if( timing > 120 ){
      currentAnchor ++;
      currentAnchor  %= anchors.Length;
      text.Set( anchors[currentAnchor] );
      timing = 0;
    }

  }

  
}
