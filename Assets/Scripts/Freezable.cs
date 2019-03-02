using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using DG.Tweening;

[RequireComponent(typeof(Collider2D))]
public class Freezable : MonoBehaviour, Rewindable{
    [SerializeField] float duration;
    [SerializeField] public FreezeEvent onFreeze;
    [SerializeField] public UnityEvent onUnfreeze;
    Coroutine TimerRoutine;


    bool paused;
    float timer;

    public void Rewind(){
        Unfreeze();
    }
    public void Freeze(){
        StartCoroutine(StartFreeze());
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
        if(!paused){
            while(timer <= duration){
                timer += Time.deltaTime;
                yield return null;
            }
        }
        Unfreeze();
    }

    public void PauseTimer(){
        paused = true;
    }
    public void ResumeTimer(){
        paused = false;
    }
    
   public void Unfreeze(){
        onUnfreeze.Invoke();
    }
}

[System.Serializable]
public class FreezeEvent : UnityEvent<float>{}