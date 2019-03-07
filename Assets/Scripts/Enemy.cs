using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Enemy : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] float range;
    Rigidbody2D rb;

    Tweener motion;
    Coroutine freezeAnimetion;

    // Start is called before the first frame update
    void Start()
    {   
        //motion = transform.DOMoveX(range, range/speed).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine).SetRelative(true); 
        motion = transform.DOBlendableMoveBy(Vector3.right * range, range/speed).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
        rb = GetComponent<Rigidbody2D>();
    }


    public void Freeze(float duration){
        var sr = GetComponent<SpriteRenderer>();
        var color = sr.color;
        motion.Pause();

        GetComponent<Collider2D>().usedByEffector = true;
        sr.DOColor(Color.cyan, 0.2f).onComplete = () => sr.DOColor(color, duration -.2f);
    }

    public void BecomeGrabbable(){
        var grabbable = gameObject.AddComponent<Grabbable>();
        var freezable = GetComponent<Freezable>();
        
        grabbable.onGrab.AddListener(()=>{
            freezable.PauseTimer();
        });
        grabbable.onRelease.AddListener(()=>{
            freezable.ResumeTimer();
        });
        grabbable.onThrow.AddListener(()=>{
            freezable.ResumeTimer();
        });
        freezable.onUnfreeze.AddListener(()=> Destroy(grabbable));
    }
    public void Unfreeze(){
        rb.velocity = Vector2.zero;
        motion.Play();
        GetComponent<Collider2D>().usedByEffector = false;
    }

    void OnDrawGizmosSelected(){
        Gizmos.color = Color.blue;

        Gizmos.DrawLine(transform.position, transform.position + (transform.right * range));
        Gizmos.DrawCube(transform.position + (transform.right * (range/2)), Vector2.one * 0.1f);
    }

    void OnCollisionEnter2D(Collision2D coll){
        var player = coll.gameObject.GetComponent<Player>();
        if(player){
            if(coll.GetContact(0).normal == Vector2.down){
            }
        }
    }
}
