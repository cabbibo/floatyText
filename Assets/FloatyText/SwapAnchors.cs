using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class SwapAnchors : MonoBehaviour
{

    public float lerpSpeed;
    public TextParticles particles;
    public TextAnchor[] anchors;


    public float swapTime;
    private float lastSwapTime;

    public Transform camera;

    
    int currentAnchor;

    public void Swap(){
        
        if( currentAnchor != -1 ){  anchors[currentAnchor].debug = false; }
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
        currentAnchor = -1;
        lastSwapTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {

        if( currentAnchor != -1 ){
            camera.position = Vector3.Lerp( camera.position , anchors[currentAnchor].transform.position , lerpSpeed );
            camera.rotation = Quaternion.Slerp( camera.rotation, anchors[currentAnchor].transform.rotation, lerpSpeed );
        }
        if( swapTime != 0 ){
            if( Time.time - lastSwapTime > swapTime){
                lastSwapTime = Time.time;
                Toggle();
            }
        }
    }

    public bool held = false;
    void Toggle(){

        Swap();/*
        held = !held;

        if( held ){
            Swap();
        }else{
            Release();
        }*/
    }


}
