using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shockwave : MonoBehaviour
{
    [SerializeField]float time;
    [SerializeField]AnimationCurve size;
    [SerializeField]AnimationCurve strength;
    [SerializeField]AnimationCurve velStrength;
    [SerializeField]Renderer rend;
    [SerializeField]MeshFilter mesh;
    float speed = 1;
    float t;
    void Start()
    {
        //renderer = GetComponent<Renderer>();
    }
    public void Setup(Vector3 point,float _speed, MeshFilter _mesh){
        rend.material.SetVector("center",point);
        speed= velStrength.Evaluate(_speed);
        time = Mathf.Max(time*speed,0.5f);
        mesh.sharedMesh = _mesh.sharedMesh;
    }

    // Update is called once per frame
    void Update()
    {
         t+= Time.deltaTime;
        rend.material.SetFloat("radius",size.Evaluate(t/time)*speed);
        rend.material.SetFloat("strength",strength.Evaluate(t/time));
        // transform.localScale = Vector3.one*size.Evaluate(t);
        if(t>=time){
            Destroy(gameObject);
        }

    }
}
