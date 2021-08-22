using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class nameTag : MonoBehaviour
{

    Transform cam;
    // Start is called before the first frame update
    void Start()
    {
        cam= Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if(cam != null){
            transform.rotation=Quaternion.LookRotation(cam.forward,cam.up);
            //transform.LookAt(cam.position,cam.up);
        } 
        else{
            cam = Camera.main.transform;
        }
    }
}
