using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

public class Grabbable: Affectable
{
    public bool isAffectable;
    public Collider2D bodyCollider;
    [Space]
    public UnityEvent onGrab = new UnityEvent();
    public UnityEvent onRelease = new UnityEvent();
    public UnityEvent onThrow = new UnityEvent();
    public UnityEvent onFree = new UnityEvent();
    public UnityEvent onGroundHit = new UnityEvent();

    [HideInInspector]
    /// <summary>
    /// Checks if the object has finihed translating to the holder
    /// </summary>
    public bool hasFinishedGrabbed = false;

    Transform socket;
    Coroutine follow;
    Rigidbody2D rb;

    void Start(){
        rb = GetComponent<Rigidbody2D>();
        bodyCollider = GetComponent<Collider2D>();
        if(onGrab == null) onGrab = new UnityEvent();
        if(onRelease == null) onRelease = new UnityEvent();
        if(onThrow == null) onThrow = new UnityEvent();
    }

    public override void SetAffectable(bool enabled){
        isAffectable = enabled;
    }

    public bool Grab(Transform socket){
        if(!isAffectable) return false;
        rb.bodyType = RigidbodyType2D.Static;
        bodyCollider.enabled = false;
        StartCoroutine(SocketTranslation(socket));
        return true;
    }

    IEnumerator SocketTranslation(Transform socket){
        hasFinishedGrabbed = false;
        var translate = transform.DOMove(socket.transform.position, 0.2f, false).OnComplete(()=>{
            this.socket = socket;
            onGrab.Invoke();
            follow = StartCoroutine(FollowHolder());
        });
        translate.SetUpdate(UpdateType.Fixed);
        transform.DORotateQuaternion(Quaternion.identity,.2f);
        yield return translate.WaitForCompletion();
        hasFinishedGrabbed = true;

    }
    public override bool Dispel(){
        Release();
        return true;
    }
    public void Release(){
        if(follow != null)
            StopCoroutine(follow);
        
        rb.bodyType = RigidbodyType2D.Dynamic;
        
        onRelease.Invoke();
    }
    public void Throw(float force){
        if(follow != null)
            StopCoroutine(follow);

        var rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Dynamic;
        bodyCollider.enabled = true;
        print(socket.TransformDirection(Vector2.one) * force);
        rb.AddForce(socket.TransformDirection(Vector2.one) * force, ForceMode2D.Impulse);
        onThrow.Invoke();
    }

    public void Free(){
        if(follow != null)
            StopCoroutine(follow);
        bodyCollider.enabled = true;
        onFree.Invoke();
    }

    void Launch(float force){
        
    }

   IEnumerator FollowHolder(){
       while(true){
           transform.position = socket.position;
           transform.localScale = socket.localScale;
           transform.rotation = Quaternion.identity;
           yield return new WaitForFixedUpdate();
       }
   }

   void OnCollisionEnter2D(Collision2D coll){
       if(coll.otherCollider.tag == "Ground"){
           
       }
       onGroundHit.Invoke();
   }


}
