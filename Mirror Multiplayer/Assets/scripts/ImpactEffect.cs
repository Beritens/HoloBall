using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpactEffect : MonoBehaviour
{
    [SerializeField] GameObject shockwavePrefab;
    [SerializeField] MeshFilter mesh;
    void OnCollisionEnter(Collision other)
    {
        if(other.gameObject.tag=="Ball"){
            shockwave wave = Instantiate(shockwavePrefab,transform.position,transform.rotation).GetComponent<shockwave>();
            wave.transform.localScale=transform.localScale;
            wave.Setup(other.contacts[0].point,other.relativeVelocity.magnitude,mesh);
        }
    }
}
