using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

public class Grabbable: Affectable
{
    public bool isAffectable;
    public UnityEvent onGrab = new UnityEvent();
    public UnityEvent onRelease = new UnityEvent();
    public UnityEvent onThrow = new UnityEvent();
    public UnityEvent onFree = new UnityEvent();
    public UnityEvent onGroundHit = new UnityEvent();

    Transform socket;
    Coroutine follow;
    Rigidbody2D rb;

    void Start(){
        rb = GetComponent<Rigidbody2D>();
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
        rb.bodyType = RigidbodyType2D.Static;
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
        
        rb.bodyType = RigidbodyType2D.Dynamic;
        GetComponent<Collider2D>().enabled = true;
        onRelease.Invoke();
    }
    public void Throw(float force){
        if(follow != null)
            StopCoroutine(follow);

        var rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Dynamic;
        GetComponent<Collider2D>().enabled = true;
        rb.AddForce(socket.TransformDirection(Vector2.one) * force, ForceMode2D.Impulse);
        onThrow.Invoke();
    }

    public void Free(){
        if(follow != null)
            StopCoroutine(follow);
        GetComponent<Collider2D>().enabled = true;
        onFree.Invoke();
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

   void OnCollisionEnter2D(Collision2D coll){
       if(coll.otherCollider.tag == "Ground"){
           
       }
       onGroundHit.Invoke();
   }


}
