using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public abstract class GuidedProjectile : MonoBehaviour{
    

    [SerializeField] protected float duration;
    [SerializeField] protected float launchSpeed;
    [SerializeField] protected float speed;

    protected Rigidbody2D rb;
    protected UnityAction destroyAction;
    protected UnityAction<MonoBehaviour> effectAction;
    protected float timer;
    protected KeyCode selfDestroyKey;

    /// <summary>
    /// Method to initialize all the needed references and launch the projectile
    /// </summary>
    /// <param name="onEffect">Callback to when the projectile hits a target and apply some effect</param>
    /// <param name="onDestroy">Callback to when the projectile is destroyed</param>

    public void Initialize(UnityAction<MonoBehaviour> onEffect, UnityAction onDestroy, KeyCode selfDestroyKey){
        rb = GetComponent<Rigidbody2D>();
        effectAction = onEffect;
        destroyAction = onDestroy;
        this.selfDestroyKey = selfDestroyKey;
        onInitialize();
    }

    protected abstract void onInitialize();
    protected abstract void onHit(Collider2D hit);
    protected abstract void onUpdate();
    public abstract MonoBehaviour getEffect();

    private void Destruction(){
        destroyAction();
        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D coll){
        onHit(coll);
        Destruction();
    }

    private void Update(){
        Movement();
        timer += Time.deltaTime;
        onUpdate();
        if(timer >= duration){
            Destruction();
        }
        if(selfDestroyKey != KeyCode.None && Input.GetKeyDown(selfDestroyKey)){
            Destruction();
        }

    }

    protected virtual void Movement(){
        var hor = Input.GetAxisRaw("Horizontal");
        var ver = Input.GetAxisRaw("Vertical");

        //rb.velocity = new Vector2(hor, ver) * speed;
        rb.AddForce(new Vector2(hor,ver)* speed, ForceMode2D.Force);
    }

}