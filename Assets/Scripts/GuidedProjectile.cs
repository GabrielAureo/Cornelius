using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public abstract class GuidedProjectile : MonoBehaviour{
    

    [SerializeField] protected float duration;
    [SerializeField] protected float launchSpeed;
    [SerializeField] protected float speed;

    protected Rigidbody2D rb;
    public UnityEvent onDestroy;
    protected UnityAction<MonoBehaviour> effectAction;
    protected float timer;
    protected KeyCode selfDestroyKey;

    /// <summary>
    /// Method to initialize all the needed references and launch the projectile
    /// </summary>
    /// <param name="onEffect">Callback to when the projectile hits a target and apply some effect</param>
    /// <param name="onDestroy">Callback to when the projectile is destroyed</param>

    public void Initialize(UnityAction<MonoBehaviour> onEffect){
        rb = GetComponent<Rigidbody2D>();
        effectAction = onEffect;
        onInitialize();
    }

    protected abstract void onInitialize();
    protected abstract void onHit(Collider2D hit);
    protected abstract void onUpdate();

    public void DestroyProjectile(){
        onDestroy.Invoke();
        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D coll){
        onHit(coll);
        DestroyProjectile();
    }

    private void Update(){
        Movement();
        timer += Time.deltaTime;
        onUpdate();
        if(timer >= duration){
            DestroyProjectile();
        }

    }

    protected virtual void Movement(){
        var hor = Input.GetAxisRaw("Horizontal");
        var ver = Input.GetAxisRaw("Vertical");

        //rb.velocity = new Vector2(hor, ver) * speed;
        rb.AddForce(new Vector2(hor,ver)* speed, ForceMode2D.Force);
    }

}