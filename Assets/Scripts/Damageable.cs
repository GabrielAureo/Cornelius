using UnityEngine;
using UnityEngine.Events;

public class Damageable : MonoBehaviour{
    public UnityEvent onDamage;

    public void TakeDamage(){
        onDamage.Invoke();
    }

}