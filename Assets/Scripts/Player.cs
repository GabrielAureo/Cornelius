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

    State state;
    Rewindable lastRewindable;
    float defaultGravityScale;
    Rigidbody2D rb;
    float jumpForce;
    AudioSource source;

    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        source = GetComponent<AudioSource>();
        jumpForce = Mathf.Sqrt(2f * Physics2D.gravity.magnitude * rb.gravityScale * jumpHeight) * rb.mass;
        defaultGravityScale = rb.gravityScale;
    }

    void Jump(){
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.AddForce(jumpForce * Vector2.up, ForceMode2D.Impulse);
    }
    void Walk(float direction){
        rb.velocity = new Vector2(walkSpeed * direction, rb.velocity.y);
    }
    void LaunchFreeze(){

        var projectile = Instantiate(freezeProjectile,transform.position, transform.rotation);
        GameManager.instance.cameraController.SetTarget(projectile.transform, true);

        var projScript = projectile.GetComponent<GuidedProjectile>();

        projScript.Initialize(CheckRewindable, ReturnControl);

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
        if(Input.GetKeyDown(KeyCode.W)){
            Jump();
        }
        var hor = Input.GetAxisRaw("Horizontal");
        Walk(hor);
    }

    void Grab(GameObject target){
        var grabbable = target.GetComponent<Grabbable>();
        if(grabbable == null) return;

    }

    IEnumerator Rewind(){
        if(lastRewindable != null){
            source.PlayOneShot(rewindSFX);
            yield return new WaitForSeconds(0.2f);
            lastRewindable.Rewind();
        }
        
    }

    void Attack(){
        if(Input.GetKeyDown(KeyCode.E)){
            LaunchFreeze();
        }
        if(Input.GetKeyDown(KeyCode.R)){
            StartCoroutine(Rewind());
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(state == State.Normal){
            Movement();
            Attack();
        }       
    }
}
