using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Grabbable: MonoBehaviour
{
    public UnityEvent onGrab;
    public UnityEvent onRelease;
    public UnityEvent onThrow;

    Transform socket;

    public void Grab(Transform socket){
        this.socket = socket;
        onGrab.Invoke();
    }
    public void Release(){
        onRelease.Invoke();
    }
    public void Throw(){
        onThrow.Invoke();
    }

   IEnumerator FollowHolder(){
       transform.position = socket.position;
       yield return null;
   }
}
