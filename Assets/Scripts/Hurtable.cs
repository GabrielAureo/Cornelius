using UnityEngine;

[RequireComponent(typeof(Collider2D))]

public class Hurtable : MonoBehaviour{
    public bool active;
    void OnTriggerEnter2D(Collider2D coll){
        if(active && coll.tag == "Player"){
            coll.GetComponent<Player>().TakeDamage();
        }
    }
}