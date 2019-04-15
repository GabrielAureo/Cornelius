using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]

public class HurtCollider : MonoBehaviour{
    public enum Side{ Player, Enemy}

    public bool active;
    public bool phasingOnCollision = true;
    public LayerMask layerMask;

    public UnityEvent onCollision;


    public void AddLayer(string name){
        layerMask |= 1 << LayerMask.NameToLayer(name);        
    }
    public void RemoveLayer(string name){
        layerMask &= ~(1<<LayerMask.NameToLayer(name));
    }

    /*void OnTriggerEnter2D(Collider2D coll){
        print(isInMask(coll.gameObject.layer).ToString() + ", " + coll.name);
        if(active && isInMask(coll.gameObject.layer)){
            var isDamageable =  coll.GetComponent<Damageable>();
           if(isDamageable){
               isDamageable.TakeDamage();
               onCollision.Invoke();
           } 
        }
    }*/

    bool isInMask(int layer){
        return layerMask == (layerMask| (1 << layer));
    }
}