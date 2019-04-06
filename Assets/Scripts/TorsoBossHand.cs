using UnityEngine;
using DG.Tweening;
using System.Collections;

public class TorsoBossHand : MonoBehaviour{
    SpriteRenderer sr;
    public TorsoBossEye eye;
    Animator anim;
    Vector3 defaultStompPosition;
    Vector3 idlePosition;

    [Header("Stomp Fields")]
    public float stompInterval;
    public float stompFallSpeed;
    public float stompRiseSpeed;
    public float handFollowSpeed;

    
    void Start(){
        idlePosition = transform.localPosition;
        sr = GetComponent<SpriteRenderer>();
        sr.color = new Color(1,1,1,0);
        eye.Attach();
        anim = GetComponent<Animator>();
    }

    public void FadeIn(float duration){
        sr.DOColor(Color.white, duration);
    }

    public void StompAttack(Vector3 startPosition){
        anim.SetTrigger("Clutch");
        defaultStompPosition = startPosition;
        transform.DOMove(defaultStompPosition,1f).onComplete+= () =>
        StartCoroutine(StompRoutine(3));
    }

    IEnumerator StompRoutine(int times){
        float followDuration = 3f;
        var player = GameManager.instance.player;
        int i = 0;
        var target = Physics2D.Raycast(transform.position, Vector3.down, Mathf.Infinity,1 << LayerMask.NameToLayer("Ground")).point;
        while (i < times){
            var playerPos = player.transform;
            var followMotion = transform.DOMove(new Vector2(playerPos.position.x, transform.position.y),.2f)
            .SetUpdate(UpdateType.Fixed).SetEase(Ease.OutSine);
            followMotion.onUpdate += ()=> followMotion.ChangeEndValue(new Vector2(playerPos.position.x, transform.position.y),true);
            yield return new WaitForSeconds(followDuration);
            followMotion.Kill();

            transform.DOMoveY(target.y, stompFallSpeed).SetSpeedBased();
            yield return new WaitForSeconds(stompInterval);
            i++;
            var returnMotion = transform.DOMoveY(defaultStompPosition.y, stompRiseSpeed).SetSpeedBased();
            while(returnMotion.IsActive()) yield return null;
        }
        ReturnToIdle();
    }

    void ReturnToIdle(){
        transform.DOLocalMove(idlePosition, 5f).SetSpeedBased().SetEase(Ease.InOutSine);
        anim.Play("Idle");
    }


    public void setAsActive(){
        sr.sortingOrder = 1;
    }

    public void setAsInactive(){
        sr.sortingOrder = -1;
    }

    void OnTriggerEnter2D(Collider2D coll){
        if(coll.tag == "Player"){
            GameManager.instance.player.TakeDamage();
        }
    }


}