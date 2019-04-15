using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

public class Player : MonoBehaviour
{   
    enum State{ Normal, Casting, Grabbing }
    public float walkSpeed;
    public float jumpHeight;
    public float slowGravityScale;

    public float throwForce;

    public GameObject freezeProjectile;
    public AudioClip rewindSFX;
    public Transform projectileSpawn;
    public Grabber grabber;
    public SpriteRenderer spriteRenderer;
    public float invulnerabilityDuration;
    public Collider2D footCollider;
    public ContactFilter2D groundFilter;
    public ContactFilter2D enemyFilter;

    bool invulnerable;
    bool canJump = true;
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
        jumpForce = Mathf.Sqrt(2f * Physics2D.gravity.magnitude * rb.gravityScale * jumpHeight) * rb.mass;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        source = GetComponent<AudioSource>();
        anim = GetComponentInChildren<Animator>();
        defaultGravityScale = rb.gravityScale;
        jumpForce = Mathf.Sqrt(2f * Physics2D.gravity.magnitude * rb.gravityScale * jumpHeight) * rb.mass;
    }
    #region Basic Actions
    public void Jump(){
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.AddForce(jumpForce * Vector2.up, ForceMode2D.Impulse);
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

    void CheckCollision(Collision2D coll){
        //collision from bellow
        var arr = new ContactPoint2D[8];
        int contacts = coll.GetContacts(arr);
        bool hitBellow = false;
        foreach(var contact in arr){
            if(Vector3.Dot(contact.normal, Vector3.up) > 0.5){
                //print(contact);
                hitBellow = true;
                break;
            }
        }
        if(hitBellow){
            if(coll.gameObject.tag == "Ground" && coll.enabled){
                canJump = true;
            }else{
                var stompable = coll.gameObject.GetComponent<Stompable>();
                if(stompable != null){
                    Jump();
                    stompable.Stomp();
                }
            }
        }else{
            var hurtable = coll.gameObject.GetComponent<HurtCollider>();
            if(hurtable!= null){
                TakeDamage();
                hurtable.onCollision.Invoke();
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
        CheckBellow();

        if(state != State.Casting){
            Movement();
           
        }
        Attack();
    }


    void CheckBellow(){
        if(footCollider.IsTouching(groundFilter)){
            canJump = true;
        }else{
            canJump = false;
        }

        var enemy = new Collider2D[1];
        if(footCollider.OverlapCollider(enemyFilter,enemy) > 0){
            print("a");
            var stompable = enemy[0].GetComponent<Stompable>();
            if(stompable != null){
                stompable.Stomp();
                Jump();
            }
        }
    }

    /*void OnCollisionStay2D(Collision2D coll){
        CheckCollision(coll);   
    }

    When the player presses the cast button immediatly after the jump,
    the canJump flag is consumed without leaving the ground
    void OnCollisionExit2D(Collision2D coll){
        if(coll.gameObject.tag == "Ground"){
            canJump = false;
        }
    }*/

    void OnDrawGizmosSelected(){
        Gizmos.color = Color.red;
        Gizmos.DrawRay(new Ray(transform.position, transform.TransformDirection(Vector2.right) * 2f));
        Gizmos.DrawRay(new Ray(transform.position, -transform.up * 2f));
    }


    

}
