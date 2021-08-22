using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DetectBall : MonoBehaviour
{
    public string tagy;
    public UnityEvent enter;
    /// <summary>
    /// OnTriggerEnter is called when the Collider other enters the trigger.
    /// </summary>
    /// <param name="other">The other Collider involved in this collision.</param>
    void OnTriggerEnter(Collider other)
    {
        Debug.Log("hello");
        if(other.tag==tagy||tag==""){
            
            enter.Invoke();
        }
    }
    
}
