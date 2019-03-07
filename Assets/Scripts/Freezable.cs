using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using DG.Tweening;

[RequireComponent(typeof(Collider2D))]
public class Freezable : Affectable, Rewindable{
    public bool isAffectable;
    [SerializeField] float duration;
    [SerializeField] public FreezeEvent onFreeze;
    [SerializeField] public UnityEvent onUnfreeze;
    Coroutine TimerRoutine;
    bool paused;
    float timer;

    public override void SetAffectable(bool enabled){
        isAffectable = enabled;
    }

    public void Rewind(){
        Dispel();
    }
    public bool Freeze(){
        if(isAffectable){
            StartCoroutine(StartFreeze());
            return true;
        }
        return false;
        
    }
    public IEnumerator StartFreeze(){
        if(TimerRoutine != null){
            StopCoroutine(TimerRoutine);
        } 
        onFreeze.Invoke(duration);
        yield return TimerRoutine = StartCoroutine(UnfreezeTimer());
    }

    IEnumerator UnfreezeTimer(){
        timer = 0;
        
        while(timer <= duration){
            if(!paused) timer += Time.deltaTime;
            yield return null;
        }
        Dispel();
    }

    public void PauseTimer(){
        paused = true;
    }
    public void ResumeTimer(){
        paused = false;
    }
    
   public override bool Dispel(){
        if(TimerRoutine != null) StopCoroutine(TimerRoutine);
        onUnfreeze.Invoke();
        return true;
    }
}

[System.Serializable]
public class FreezeEvent : UnityEvent<float>{}