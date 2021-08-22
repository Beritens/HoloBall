using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraMovement : MonoBehaviour
{
    public Transform target;
    public float smoothTime = 0.3f;
    public float rotationLerp = 0.3f;
    private Vector3 velocity = Vector3.zero;
    public LayerMask mask;

    // Update is called once per frame
    void FixedUpdate()
    {
        if(target == null){
            return;
        }
        //transform.position=target.position;
        transform.position=Vector3.SmoothDamp(transform.position,target.position,ref velocity,smoothTime);
        
        //transform.rotation=Quaternion.Slerp(transform.rotation,target.rotation,rotationLerp);
    }
    /// <summary>
    /// LateUpdate is called every frame, if the Behaviour is enabled.
    /// It is called after all Update functions have been called.
    /// </summary>
    void LateUpdate()
    {
        if(target == null){
            return;
        }
        transform.rotation=Quaternion.Lerp(transform.rotation,target.rotation,rotationLerp);
    }
}
