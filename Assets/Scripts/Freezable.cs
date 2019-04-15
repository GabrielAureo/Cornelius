using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using DG.Tweening;
using MyBox;

[RequireComponent(typeof(Collider2D))]
public class Freezable : Affectable, Rewindable{
    public bool isAffectable;
    public bool useDefaultAnimation;
    [ConditionalField("useDefaultAnimation",true)]
    public Color frozenColor;
    [ConditionalField("useDefaultAnimation",true)]
    public SpriteRenderer spriteRenderer;
    [SerializeField] float duration;
    [SerializeField] public FreezeEvent onFreeze;
    [SerializeField] public UnityEvent onUnfreeze;

    Coroutine TimerRoutine;
    bool paused;
    float timer;
    bool active;
    Tweener colorChange;
    Color defaultColor;
    RigidbodyType2D bodyType;

    void Start(){
        if(spriteRenderer!= null) defaultColor = spriteRenderer.color;
    }


    public override void SetAffectable(bool enabled){
        isAffectable = enabled;
    }

    void FreezeAnimation(){
        if(colorChange!= null) colorChange.Kill();
        spriteRenderer.color = defaultColor;
        spriteRenderer.DOColor(frozenColor,.2f).onComplete+=()=>
        colorChange = spriteRenderer.DOColor(defaultColor, duration).SetEase(Ease.InSine);
    }

    void UnfreezeAnimation(){
        if(colorChange!= null){
            colorChange.Kill();
            spriteRenderer.color = defaultColor;
        }
    }
    /// <summary>
    /// Is the freeze effect currently active on the object?
    /// </summary>
    /// <returns>TRUE, the object is currently frozen.
    /// FALSE, the object is not currently frozen.</returns>
    public bool isActive(){
        return active;
    }

    public void Rewind(){
        Dispel();
    }

    public bool Freeze(){
        //Check if the effect is already active to not misassign the body type.
        //The object may be frozen, grabbed (turning its body in Dynamic) and frozen again.
        if(isAffectable){
            if(!active) bodyType = GetComponent<Rigidbody2D>().bodyType;
            StartCoroutine(StartFreeze());
            if(useDefaultAnimation) FreezeAnimation();
            active = true;
            return true;
        }
        return false;
        
    }
    public IEnumerator StartFreeze(){
        if(TimerRoutine != null){
            StopCoroutine(TimerRoutine);
        } 
        onFreeze.Invoke(duration);
        if(duration == 0) yield return null;
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
        colorChange.Pause();
    }
    public void ResumeTimer(){
        paused = false;
        colorChange.Play();
    }
    
   public override bool Dispel(){
        if(TimerRoutine != null) StopCoroutine(TimerRoutine);
        GetComponent<Rigidbody2D>().bodyType = bodyType;
        if(useDefaultAnimation) UnfreezeAnimation();
        onUnfreeze.Invoke();
        active = false;
        return true;
    }
}

[System.Serializable]
public class FreezeEvent : UnityEvent<float>{}