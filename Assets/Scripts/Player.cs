using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

public class Player : MonoBehaviour
{   
    enum State{ Normal, Casting, Grabbing }
    public float walkSpeed;
    public float maxJumpHeight;
    public float minJumpHeight;
    public float slowGravityScale;

    public float throwForce;

    public GameObject freezeProjectile;
    public AudioClip rewindSFX;
    public Transform projectileSpawn;
    public Grabber grabber;
    public SpriteRenderer spriteRenderer;
    public float invulnerabilityDuration;

    bool invulnerable;
    bool canJump = true;
    bool releasedJump = false;
    bool jumping = false;
    float jumpForce;
    float defaultGravityScale;

    GuidedProjectile currentProjectile;

    State state;
    Rewindable lastRewindable;
    

    Rigidbody2D rb;
    Animator anim;
    AudioSource source;
    

    void OnValidate(){
        if(rb== null) rb = GetComponent<Rigidbody2D>();
        jumpForce = Mathf.Sqrt(2f * Physics2D.gravity.magnitude * rb.gravityScale * maxJumpHeight) * rb.mass;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        source = GetComponent<AudioSource>();
        anim = GetComponentInChildren<Animator>();
        defaultGravityScale = rb.gravityScale;
        jumpForce = Mathf.Sqrt(2f * Physics2D.gravity.magnitude * rb.gravityScale * maxJumpHeight) * rb.mass;
    }
    #region Basic Actions
    public void Jump(){
        jumping = true;
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.AddForce(jumpForce * Vector2.up, ForceMode2D.Impulse);
        canJump = false;
    }
    void Walk(float direction){
        rb.velocity = new Vector2(walkSpeed * direction, rb.velocity.y);
        anim.SetFloat
        ("speed", Mathf.Abs(rb.velocity.x));
    }
    public void TakeDamage(){
        if(invulnerable) return;
        if(state == State.Casting){
            currentProjectile.DestroyProjectile();
        }
        if(state == State.Grabbing){
            grabber.Release();
        }
        state = State.Normal;
        spriteRenderer.DOColor(Color.red, invulnerabilityDuration).SetEase(Ease.Flash,5).onComplete+=()=>
        {
            invulnerable = false;
            spriteRenderer.DOColor(Color.white, .1f);
        };
    }

    void Movement(){
        if(Input.GetKeyDown(KeyCode.Space) && canJump){
            releasedJump = false;
            Jump();
        }
        var hor = Input.GetAxisRaw("Horizontal");
        if(hor != 0){
            var scale = transform.localScale;
            transform.localScale = new Vector3 (hor * Mathf.Abs(scale.x) ,scale.y,scale.z);
        }       
        Walk(hor);
    }

    GuidedProjectile LaunchProjectile(){

        var projectile = Instantiate(freezeProjectile,projectileSpawn.position, transform.rotation);
        GameManager.instance.cameraController.SetTarget(projectile.transform, true);

        var projScript = projectile.GetComponent<GuidedProjectile>();
        projScript.onDestroy.AddListener(ReturnControl);

        projScript.Initialize(CheckRewindable);

        rb.velocity = Vector2.zero;
        rb.gravityScale = slowGravityScale;
        state = State.Casting;
        return projScript;
    }
    #endregion

    public void Grab(){
        state = State.Grabbing;
    }
    public void ReleaseGrabbable(){
        state = State.Normal;
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
        if(state != State.Grabbing){
            if(Input.GetKeyDown(KeyCode.E)){
                if(currentProjectile != null){
                    currentProjectile.DestroyProjectile();
                    currentProjectile = null;
                    return;
                }
                currentProjectile = LaunchProjectile();
            }
            if(Input.GetKeyDown(KeyCode.R)){
                Rewind();
            }
        }

        if(Input.GetKeyDown(KeyCode.LeftShift) && state != State.Casting){
            if(grabber.currentGrab != null){
                grabber.Throw();
            }else{
                grabber.TryGrab();
            }
            
        }
    }

    #region Utility Methods

    public void AllowJump(){
        canJump = true;
    }
    void CheckBellow(Collision2D coll){
        //collision from bellow
        if(Vector3.Dot(coll.GetContact(0).normal, Vector3.up) > 0.5){
            if(coll.gameObject.tag == "Ground" && coll.enabled){
                canJump = true;
                jumping = false;
            }else{
                var stompable = coll.gameObject.GetComponent<Stompable>();
                if(stompable != null){
                    Jump();
                    stompable.Stomp();
                }
            }
        }
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


    #endregion

    void Update()
    {
        if(state != State.Casting){
            Movement();
           
        }
        Attack();
        
    }

    void OnCollisionEnter2D(Collision2D coll){
        CheckBellow(coll);   
    }

    void OnCollisionStay2D(Collision2D coll){
        //Checks if the player has jumped and cast magic at the same time
        if(!canJump && state == State.Casting){
            CheckBellow(coll);
        }
    }

    void OnDrawGizmosSelected(){
        Gizmos.color = Color.red;
        Gizmos.DrawRay(new Ray(transform.position, transform.TransformDirection(Vector2.right) * 2f));
        Gizmos.DrawRay(new Ray(transform.position, -transform.up * 2f));
    }


    

}
