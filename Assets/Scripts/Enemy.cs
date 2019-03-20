using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Enemy : MonoBehaviour, Stompable
{
    [SerializeField] float speed;
    [SerializeField] float range;
    Rigidbody2D rb;

    Tweener motion;

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

        sr.DOColor(Color.cyan, 0.2f).onComplete = () => sr.DOColor(color, duration -.2f);
    }

    public void Stomp(){
        Destroy(gameObject);
    }

    string oldTag;
    public void BecomePlatform(){
        oldTag = gameObject.tag;
        gameObject.tag = "Ground";
        GetComponent<Collider2D>().usedByEffector = true;
    }

    public void ReturnToEnemy(){
        gameObject.tag = oldTag;
        GetComponent<Collider2D>().usedByEffector = false;
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
        motion.Restart();
    }

    void OnDrawGizmosSelected(){
        Gizmos.color = Color.blue;

        Gizmos.DrawLine(new Vector2(transform.position.x - range/2, transform.position.y)
        , new Vector2(transform.position.x + range/2, transform.position.y));
    }

    void OnCollisionEnter2D(Collision2D coll){
        var player = coll.gameObject.GetComponent<Player>();
        if(player){
            if(coll.GetContact(0).normal == Vector2.down){
            }
        }
    }
}
