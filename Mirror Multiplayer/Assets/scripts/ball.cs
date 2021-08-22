using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ball : MonoBehaviour
{
    [SerializeField]AudioClip impact;
    [SerializeField]AudioSource source;
    [SerializeField]LayerMask mask;
    void OnCollisionEnter(Collision other)
    {
        if(mask == (mask | (1<<other.gameObject.layer))){
            source.PlayOneShot(impact);
        }
    }
}
