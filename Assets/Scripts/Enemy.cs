using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;

public class Enemy : MonoBehaviour, Stompable
{
    public float speed;
    public float range;
    Rigidbody2D rb;

    Tweener motion;

    private int currentDirection = 1;

    [HideInInspector]
    public UnityAction onDeath;
    [HideInInspector]
    public Vector3 startPosition;
    Coroutine respawnRoutine;


    void Start()
    {
        //motion = transform.DOMoveX(range, range/speed).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine).SetRelative(true); 
        StartMove();
        rb = GetComponent<Rigidbody2D>();
        startPosition = transform.position;

    }
    private void StartMove()
    {
        motion = transform.DOBlendableMoveBy((Vector3.right * currentDirection) * (range / 2), range / speed).SetLoops(2, LoopType.Yoyo).OnComplete(() => StartMove());
        currentDirection = -currentDirection;
    }

    public void Freeze(float duration)
    {
        var sr = GetComponent<SpriteRenderer>();
        var color = sr.color;
        motion.Pause();

        sr.DOColor(Color.cyan, 0.2f).onComplete = () => sr.DOColor(color, duration - .2f);        
    }

    public void Stomp()
    {
        //Talvez seja melhor criar um sistema de eventos futuramente
        // var v = (EnemyArea)FindObjectOfType(typeof(EnemyArea));
        // v.RespawnEnemy(gameObject);
        Kill();
    }

    public void Respawn(){
        gameObject.SetActive(true);
    }
    public void Kill(){
        if(onDeath != null) onDeath();
        gameObject.SetActive(false);
    }

    public void ReturnToDefault(){ 
        rb.bodyType = RigidbodyType2D.Static;
        transform.position = startPosition;
        ReturnToEnemy();
    }

    int oldLayer;
    public void BecomePlatform()
    {
        oldLayer = gameObject.layer;
        //gameObject.tag = "Ground";
        gameObject.layer = LayerMask.NameToLayer("Ground");
        GetComponent<Collider2D>().usedByEffector = true;
    }

    public void ReturnToEnemy()
    {
        gameObject.layer = oldLayer;
        GetComponent<Collider2D>().usedByEffector = false;
    }


    /*public void BecomeGrabbable(){
        var grabbable = gameObject.AddComponent<Grabbable>();
        var freezable = GetComponent<Freezable>();

        grabbable.onGrab.AddListener(() => {
            freezable.PauseTimer();
        });
        grabbable.onRelease.AddListener(() => {
            freezable.ResumeTimer();
        });
        grabbable.onThrow.AddListener(() => {
            freezable.ResumeTimer();
        });
        freezable.onUnfreeze.AddListener(()=> Destroy(grabbable));
    }*/
    public void Unfreeze(){
        //rb.velocity = Vector2.zero;
        currentDirection = (transform.position.x - GameManager.instance.player.transform.position.x) > 0 ? -1 : 1;
        StartMove();
    }

    IEnumerator TrackStartPosition(){
        /*yield return new WaitWhile(()=>InsideViewport());
        print("Area Inicial Fora da Camera");*/
        yield return new WaitUntil(()=>InsideViewport());
        print("Área Inicial Dentro da Câmera");
        transform.position = startPosition;
        ReturnToDefault();

    }
    bool InsideViewport(){
        var vector = Camera.main.WorldToViewportPoint(startPosition);
        if(new Rect(0,0,1,1).Contains(vector)){
            return true;
        }
        return false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;

        Gizmos.DrawLine(new Vector2(transform.position.x - range / 2, transform.position.y)
        , new Vector2(transform.position.x + range / 2, transform.position.y));
    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        var player = coll.gameObject.GetComponent<Player>();
        if (player)
        {
            if (coll.GetContact(0).normal == Vector2.down)
            {
            }
        }
    }

    void OnBecameInvisible(){
        if(respawnRoutine != null) StopCoroutine(respawnRoutine);
        if(gameObject.activeInHierarchy) //Can't start coroutine with disabled gameobject
            respawnRoutine = StartCoroutine(TrackStartPosition());
    }

    void OnBecameVisible(){
        if(respawnRoutine != null) StopCoroutine(respawnRoutine);
    }
}