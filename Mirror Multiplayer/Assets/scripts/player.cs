using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; 

public class player : NetworkBehaviour
{
    Rigidbody rb;
    public float rotateSpeed;
    public float speed;
    
    Transform cam;
    public Vector3 spawn;
    public float kickStrength;
    public float kickRadius;
    [Header("KickBox")]
    [SerializeField]Vector3 center;
    [SerializeField]Vector3 size;
    [SerializeField]LayerMask mask;
    [SerializeField]ParticleSystem[] backParticles;
    [SerializeField] ParticleSystem upParticles;
    [SerializeField] ParticleSystem kickParticles;

    [Header("sound")]
    [SerializeField]AudioSource audioSource;
    [SerializeField]AudioClip kickSound;

    // Start is called before the first frame update
    void Start()
    {
        spawn = transform.position;
        rb= GetComponent<Rigidbody>();
        if(hasAuthority &&GameObject.FindObjectOfType<cameraMovement>()!=null){
            cam= GameObject.FindObjectOfType<cameraMovement>().transform;
            cam.GetComponent<cameraMovement>().target= transform;
        }
        
        
    }
    void OnLevelWasLoaded(){
        //doing this again in the case that the scene is changed
        if(GameObject.FindObjectOfType<cameraMovement>()!=null&& hasAuthority){
            cam= GameObject.FindObjectOfType<cameraMovement>().transform;
            cam.GetComponent<cameraMovement>().target= transform;
        }
    }
    void Update()
    {
        if(hasAuthority){
            Look();
            Kick();
        }   
        
    }
    void Kick(){
        if(Input.GetButtonDown("Fire1")){
            
            CmdKick(transform.rotation);
            kickParticles.Emit(1);
        }
        
    }
    [Command]
    private void CmdKick(Quaternion rot){
        Collider[] hitColliders = Physics.OverlapBox(transform.TransformPoint(center),size,transform.rotation,mask);
        bool k = false;
        foreach (var hitCollider in hitColliders)
        {
            if(hitCollider.gameObject.tag=="Ball"){
                
                Rigidbody ball=hitCollider.gameObject.GetComponent<Rigidbody>();
                k=true;
                ball.AddForce((rot*Vector3.forward).normalized*kickStrength,ForceMode.Impulse);
                //CmdKick((transform.forward).normalized*kickStrength);
            }
        }
        RpcKick(k);
    }
    [ClientRpc]
    private void RpcKick(bool k){
        if(!hasAuthority){
            kickParticles.Emit(1);
        }
        if(k){
            audioSource.PlayOneShot(kickSound);
        }
        
        // if(k){
        //     Rigidbody ball=GameObject.FindGameObjectWithTag("Ball").GetComponent<Rigidbody>();
        //     ball.AddForce((transform.forward).normalized*kickStrength,ForceMode.Impulse);
        // }

        
    }

    void FixedUpdate()
    {
        if(hasAuthority){
            
            Move();
            
        }
        
    }
    void Look(){
        float x = Input.GetAxis("Mouse X");
        float y = Input.GetAxis("Mouse Y");
        Quaternion rotX = Quaternion.identity;
        
        Quaternion rotZ = Quaternion.identity;
        //different rotation when holding the middle mousbutton
        if(Input.GetButton("Fire3")||Input.GetButton("Fire2")){
            rotX = Quaternion.AngleAxis(-x*rotateSpeed,Vector3.forward);
        }
        else{
            rotX = Quaternion.AngleAxis(x*rotateSpeed,Vector3.up);
        }
        Quaternion rotY = Quaternion.AngleAxis(-y*rotateSpeed,Vector3.right);
        transform.rotation = (transform.rotation*rotY)*rotX*rotZ;
    }
    void Move(){
        float x = Input.GetAxisRaw("Horizontal");
        float z= Input.GetAxisRaw("Vertical");
        float y = Input.GetAxisRaw("Y");
        Vector3 movement = transform.forward*z+transform.right*x+transform.up*y;
        movement = Vector3.ClampMagnitude(movement, 1);
        rb.AddForce(movement*speed);
        bool changed = false;
        bool front = false;
        bool up = false;
        //checking if particles are already off/on: if they changed, activate or deactivate them on all clients
        if(z>0.01f && !backParticles[0].emission.enabled){
            changed = true;
            front = true;
        }
        else if(z<=0 && backParticles[0].emission.enabled){
            changed = true;
            front = false;
        }
        if(y>0.01f && !upParticles.emission.enabled){
            changed=true;
            up = true;
        }
        else if(y<= 0 & upParticles.emission.enabled){
            changed=true;
            up = false;
        }
        if(changed){
            CmdActivateParticles(up,front);
        }
        
        
    }
    [Command]
    void CmdActivateParticles(bool up, bool front){
        RpcActivateParticles(up,front);
    }
    [ClientRpc]
    void RpcActivateParticles(bool up, bool front){
        foreach (ParticleSystem p in backParticles)
        {   
            var emis = p.emission;
            emis.enabled = front;
        }
        var emission = upParticles.emission;
        emission.enabled=up;
    }
    
}
