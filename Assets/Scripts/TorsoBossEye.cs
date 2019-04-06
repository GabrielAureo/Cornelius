using UnityEngine;
using DG.Tweening;
using System.Collections;

public class TorsoBossEye : MonoBehaviour{
    public Collider2D coll;
    public float speed;

    SpriteRenderer sr;
    Coroutine motion;
    Rigidbody2D rb;
    Freezable freezeControl;
    Hurtable hurtControl;

    public bool paused = false;

    float timer;

    void Start(){
        timer = 0f;
        freezeControl = GetComponent<Freezable>();
        coll = GetComponent<Collider2D>();
        sr = GetComponent<SpriteRenderer>();
        hurtControl = GetComponent<Hurtable>();
    }

    public void Attach(){
        coll.enabled = false;
    }

    public void Freeze(float duration){
        paused = true;
        hurtControl.active = false;
        coll.isTrigger = false;
    }

    public void Unfreeze(){
        paused = false;
        hurtControl.active = true;
        coll.isTrigger = true;
    }

    Vector3 RecalculatePosition(Vector2 radius, Vector3 center){
        var pos = new Vector3(
                radius.x * Mathf.Cos(speed * timer) ,
                radius.y * Mathf.Sin(speed * timer)
            ) + center;
        return pos;
    }

    public void RotateAttack(Vector3 ellipseCenter, Vector2 ellipseRadius){
        coll.enabled = true;
        sr.DOFade(0, 0.2f).onComplete+= () =>{
            sr.flipX = true;
            sr.DOFade(1, .3f).onComplete+= ()=> {
                motion = StartCoroutine(RotateAround(ellipseCenter, ellipseRadius));
            };
        };
    }

    IEnumerator RotateAround(Vector3 center, Vector2 radius){
        var startPosition = transform.position;

        while(true){
            if(paused){
                yield return new WaitUntil(()=>!paused);
                var pos = RecalculatePosition(radius, center);
                yield return transform.DOMove(pos,.8f).WaitForCompletion();
            } 
            LookAt(center);
            //transform.RotateAround(center,Vector3.forward,speed * Time.smoothDeltaTime);
            transform.position = new Vector3(
                radius.x * Mathf.Cos(speed * timer) ,
                radius.y * Mathf.Sin(speed * timer)
            ) + center;
            timer += Time.deltaTime;
            yield return null;

        }   
    }

    void LookAt(Vector3 target){
        Vector3 dir = target - transform.position;
        float angle = Mathf.Atan2(dir.y,dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }
}