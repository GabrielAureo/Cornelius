using UnityEngine;
using DG.Tweening;
using System.Collections;

public class TorsoBossProjectile: MonoBehaviour{
    Rigidbody2D rb;
    SpriteRenderer spriteRenderer;
    public float speed;
    public float changeDirectionFrquency;

    void Awake(){
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    void OnEnable(){
        var col = spriteRenderer.color;
        spriteRenderer.color = new Color(col.r, col.g, col.b, 0f);
        spriteRenderer.DOFade(1f,.2f);

        rb.velocity = transform.up * speed;
        InvokeRepeating("ChangeDirection",0f, 1.5f);
        StartCoroutine(UpdateVelocity());
        StartCoroutine(DeathCounter());
    }

    void ChangeDirection(){
        transform.DORotate(Vector3.forward * Random.Range(0f, 360f), 1f, RotateMode.Fast);
    }
    IEnumerator UpdateVelocity(){
        while(true){
            rb.velocity = transform.up * speed;
            yield return null;
        }
    }

    IEnumerator DeathCounter(){
        yield return new WaitForSeconds(5f);
        yield return spriteRenderer.DOFade(1f, 0.2f);
        SimplePool.Despawn(gameObject);
    }
}