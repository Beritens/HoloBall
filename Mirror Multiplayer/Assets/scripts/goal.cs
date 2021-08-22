using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class goal : MonoBehaviour
{

    public UnityEvent drinBoi;
    public float yeetForce;
    bool inGoal = false;
    void OnTriggerEnter(Collider other)
    {
        Rigidbody ball = other.GetComponent<Rigidbody>();
        if(other.tag=="Ball"){
            if(Vector3.Dot(transform.forward,transform.position-ball.position)<0){
                inGoal=true;
            }
            else{
                ball.AddForce(transform.forward*yeetForce,ForceMode.Impulse);
            }
        }
    }
    void OnTriggerExit(Collider other)
    {
        if(other.tag=="Ball" ){
            if(Vector3.Dot(transform.forward,transform.position-other.transform.position)>0 && inGoal){
                drinBoi.Invoke();
                
            }
            inGoal=false;
        }

    }
}
