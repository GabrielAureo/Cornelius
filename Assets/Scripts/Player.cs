using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Player : MonoBehaviour
{   
    enum State{ Normal, Casting }
    [SerializeField] float walkSpeed;
    [SerializeField] float jumpHeight;
    [SerializeField] float slowGravityScale;
    [SerializeField] GameObject freezeProjectile;
    [SerializeField] AudioClip rewindSFX;
    [SerializeField] Transform grabSocket;
    [SerializeField] Transform projectileSpawn;

    bool canJump = true;

    State state;
    Rewindable lastRewindable;
    float defaultGravityScale;
    Rigidbody2D rb;
    Animator anim;
    float jumpForce;
    AudioSource source;
    Grabbable grabbing;

    void OnValidate(){
        if(rb== null) rb = GetComponent<Rigidbody2D>();
        jumpForce = Mathf.Sqrt(2f * Physics2D.gravity.magnitude * rb.gravityScale * jumpHeight) * rb.mass;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        source = GetComponent<AudioSource>();
        anim = GetComponentInChildren<Animator>();
        defaultGravityScale = rb.gravityScale;
    }

    public void Jump(){
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.AddForce(jumpForce * Vector2.up, ForceMode2D.Impulse);
        canJump = false;
    }
    void Walk(float direction){
        rb.velocity = new Vector2(walkSpeed * direction, rb.velocity.y);
        anim.SetFloat("speed", Mathf.Abs(rb.velocity.x));
    }
    void LaunchFreeze(){

        var projectile = Instantiate(freezeProjectile,projectileSpawn.position, transform.rotation);
        GameManager.instance.cameraController.SetTarget(projectile.transform, true);

        var projScript = projectile.GetComponent<GuidedProjectile>();

        projScript.Initialize(CheckRewindable, ReturnControl, KeyCode.E);

        rb.velocity = Vector2.zero;
        rb.gravityScale = slowGravityScale;
        state = State.Casting;
    }

    void CheckRewindable(MonoBehaviour effect){
        if(effect is Rewindable){
            this.lastRewindable = (Rewindable) effect;
        }
    }

    void ReturnControl(){
        state = State.Normal;
        rb.gravityScale = defaultGravityScale;
        GameManager.instance.cameraController.ReturnToPlayer();
    }

    void Movement(){
        if(Input.GetKeyDown(KeyCode.Space) && canJump){
            Jump();
        }
        var hor = Input.GetAxisRaw("Horizontal");
        if(hor != 0){
            var scale = transform.localScale;
            transform.localScale = new Vector3 (hor * Mathf.Abs(scale.x) ,scale.y,scale.z);
        }       
        Walk(hor);
    }

    void Grab(GameObject target){
        var grabbable = target.GetComponent<Grabbable>();
        if(grabbable == null) return;

    }

    void Rewind(){
        if(lastRewindable != null){
            if(!lastRewindable.isActive()){
                lastRewindable = null;
                return;
            }
            source.PlayOneShot(rewindSFX);
            lastRewindable.Rewind();
        }
        
    }

    void Attack(){
        if(Input.GetKeyDown(KeyCode.E)){
            LaunchFreeze();
        }
        if(Input.GetKeyDown(KeyCode.R)){
            Rewind();
        }

        if(Input.GetKeyDown(KeyCode.LeftShift)){
            if(grabbing != null){
                grabbing.Throw();
            }else{
                CheckForGrabbables(Vector2.down);
            }
            
        }
    }
    public void AllowJump(){
        canJump = true;
    }

    void CheckForGrabbables(Vector2 direction){
        int notPlayerLayer = ~(1 << 8);
        var hit = Physics2D.Raycast(transform.position,transform.TransformDirection(direction),2f, notPlayerLayer);
        if(hit.collider != null){
            var grabbable = hit.transform.GetComponent<Grabbable>();
            if(grabbable){
                grabbable.Grab(grabSocket);
                grabbing = grabbable;
                grabbing.onRelease.AddListener(()=>grabbing = null);
                grabbing.onThrow.AddListener(()=>grabbing = null);
                return;
            }
        }
        /*var down = Physics2D.Raycast(transform.position,-transform.up,2f, notPlayerLayer);
        if(down.collider != null){
            var grabbable = down.transform.GetComponent<Grabbable>();
            if(grabbable){
                grabbable.Grab(grabSocket);
                grabbing = grabbable;
                grabbing.onRelease.AddListener(()=>grabbing = null);
                grabbing.onThrow.AddListener(()=>grabbing = null);
                return;
            }
        }  */
    }
    
    void OnDrawGizmosSelected(){
        Gizmos.color = Color.red;
        Gizmos.DrawRay(new Ray(transform.position, transform.TransformDirection(Vector2.right) * 2f));
        Gizmos.DrawRay(new Ray(transform.position, -transform.up * 2f));
    }

    // Update is called once per frame
    void Update()
    {
        if(state == State.Normal){
            Movement();
            Attack();
        }
        
    }

    void OnCollisionEnter2D(Collision2D coll){
        if(Vector3.Dot(coll.GetContact(0).normal, Vector3.up) > 0.5){
            //collision from bellow
            if(coll.gameObject.tag == "Ground" && coll.enabled){
                canJump = true;
            
            }else{
                var stompable = coll.gameObject.GetComponent<Stompable>();
                if(stompable != null){
                Jump();
                stompable.Stomp();
                }
            }
        }
        
    }
}
