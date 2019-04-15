using UnityEngine;
using UnityEngine.Events;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
public class Grabber: MonoBehaviour{
    public Grabbable currentGrab{
        get; private set;
    }
    public float throwForce;
    public float angleAdjustSpeed;
    public Transform grabSocket;
    public Collider2D bodyCollider;

    public UnityEvent onGrab;
    public UnityEvent onThrow;
    public UnityEvent onRelease;
    public UnityEvent onFree;

    Vector2 direction;

    
    

    public void TryGrab(){
        if(CheckForGrabbables(Vector2.down)){
            //Callback to
            currentGrab.onFree.AddListener(Free);
            //Physics2D.IgnoreCollision(bodyCollider, currentGrab.bodyCollider,true);
            onGrab.Invoke();
        }
        
    }

    /// <summary>
    /// Releases the grabbable by choice of the holder.
    /// </summary>
    public void Release(){
        currentGrab.Release();
        currentGrab.onFree.RemoveListener(Free);
        currentGrab = null;
        
    }
    public void Throw(){
        currentGrab.Throw(throwForce);
        onThrow.Invoke();
        currentGrab.onFree.RemoveListener(Free);
        currentGrab = null;
    }
    /// <summary>
    /// Releases the grabbable object by itself, alerting the holder.
    /// </summary>
    public void Free(){
        onFree.Invoke();
        currentGrab = null;
        currentGrab.onFree.RemoveListener(Free);
    }

    bool CheckForGrabbables(Vector2 direction){
        int otherLayers = ~(1 << gameObject.layer);
        var hit = Physics2D.Raycast(transform.position,transform.TransformDirection(direction),2f, otherLayers);
        if(hit.collider != null){
            var grabbable = hit.transform.GetComponent<Grabbable>();
            if(grabbable){
                if(grabbable.Grab(grabSocket)){
                    currentGrab = grabbable;
                    return true;
                }
            }
        }
        return false;
    }

}