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
        
        anchors[currentAnchor].debug = false;
        currentAnchor ++;
        currentAnchor %= anchors.Length;
        anchors[currentAnchor].debug = true;
        particles.Set(anchors[currentAnchor]);
    }

    public void Release(){
        anchors[currentAnchor].debug = false;
        particles.Release();
    }

    // Start is called before the first frame update
    void OnEnable()
    {
        currentAnchor = 0;
        lastSwapTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if( swapTime != 0 ){
            if( Time.time - lastSwapTime > swapTime){
                lastSwapTime = Time.time;
                Toggle();
            }
        }
    }

    public bool held = false;
    void Toggle(){
        held = !held;

        if( held ){
            Swap();
        }else{
            Release();
        }
    }


}
