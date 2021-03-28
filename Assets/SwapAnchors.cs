using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class SwapAnchors : MonoBehaviour
{

    public TextParticles particles;
    public TextAnchor[] anchors;


    public float swapTime;
    private float lastSwapTime;


    int currentAnchor;
    public void Swap(){
        currentAnchor ++;
        currentAnchor %= anchors.Length;
        particles.Set(anchors[currentAnchor]);
    }

    // Start is called before the first frame update
    void OnEnable()
    {
        lastSwapTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if( swapTime != 0 ){
            if( Time.time - lastSwapTime > swapTime){
                lastSwapTime = Time.time;
                Swap();
            }
        }
    }


}
