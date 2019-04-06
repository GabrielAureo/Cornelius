using UnityEngine;
using UnityEngine.Events;

public class Grabber: MonoBehaviour{
    public Grabbable currentGrab{
        get; private set;
    }
    public float throwForce;
    public Transform grabSocket;

    public UnityEvent onGrab;
    public UnityEvent onThrow;
    public UnityEvent onRelease;
    public UnityEvent onFree;
    

    public void TryGrab(){
        if(CheckForGrabbables(Vector2.down)){
            //Callback to
            currentGrab.onFree.AddListener(Free);
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
        currentGrab.onFree.RemoveListener(Free);
        currentGrab = null;
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