using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class Jiggle : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += Vector3.right  * Mathf.Sin( Time.time ) * .1f;
    }
}
