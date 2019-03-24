using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

public class Grabbable: Affectable
{
    public bool isAffectable;
    public float throwForce;
    public UnityEvent onGrab = new UnityEvent();
    public UnityEvent onRelease = new UnityEvent();
    public UnityEvent onThrow = new UnityEvent();
    
    Transform socket;
    Coroutine follow;

    void Start(){
        if(onGrab == null) onGrab = new UnityEvent();
        if(onRelease == null) onRelease = new UnityEvent();
        if(onThrow == null) onThrow = new UnityEvent();
    }

    public override void SetAffectable(bool enabled){
        isAffectable = enabled;
    }

    public bool Grab(Transform socket){
        if(!isAffectable) return false;
        var collider = GetComponent<Collider2D>();
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
        collider.enabled = false;
        transform.DOMove(socket.transform.position, 0.2f, false).OnComplete(()=>{
            this.socket = socket;
            onGrab.Invoke();
            follow = StartCoroutine(FollowHolder());
        });
        return true;
    }
    public override bool Dispel(){
        Release();
        return true;
    }
    public void Release(){
        if(follow != null)
            StopCoroutine(follow);
        GetComponent<Collider2D>().enabled = true;
        onRelease.Invoke();
    }
    public void Throw(){
        if(follow != null)
            StopCoroutine(follow);

        var rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Dynamic;
        GetComponent<Collider2D>().enabled = true;
        rb.AddForce(socket.TransformDirection(Vector2.one) * throwForce);
        onThrow.Invoke();
    }

    public void Throw(float verticalDistance, float horizontalDistance){
        if(follow != null)
            StopCoroutine(follow);
        
        //Como o objeto é jogado do alto do jogador, ele alcança o limite horizontal ainda no ar
        //Como não posso ser incomodado paa fazer os cálculos considerando a altura inicial,
        //só reduzo uma unidade, gerando uma aproximação adequada
        horizontalDistance--;
        var rb = GetComponent<Rigidbody2D>();

        float vertSpeed = Mathf.Sqrt(2f * Physics2D.gravity.magnitude * rb.gravityScale * verticalDistance) * rb.mass;
        float duration = (2 * vertSpeed) / (Physics2D.gravity.magnitude * rb.gravityScale);
        float horSpeed = horizontalDistance / duration;

        
        rb.bodyType = RigidbodyType2D.Dynamic;
        GetComponent<Collider2D>().enabled = true;
        rb.velocity = new Vector2(horSpeed * socket.TransformDirection(Vector2.right).x, vertSpeed);
        onThrow.Invoke();
    }

    void Launch(float force){
        
    }

   IEnumerator FollowHolder(){
       while(true){
           transform.position = socket.position;
           transform.localScale = socket.localScale;
           yield return null;
       }
       
       
   }
}
