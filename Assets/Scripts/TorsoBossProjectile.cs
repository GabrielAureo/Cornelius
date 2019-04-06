using UnityEngine;
using DG.Tweening;
using System.Collections;

public class TorsoBossProjectile: MonoBehaviour{
    Rigidbody2D rb;
    public float speed;
    public float changeDirectionFrquency;

    void Start(){
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = transform.up * speed;
        InvokeRepeating("ChangeDirection",0f, 1.5f);
        StartCoroutine(UpdateVelocity());
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
}