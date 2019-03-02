using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Enemy : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] float range;

    Tweener motion;
    Coroutine freezeAnimetion;

    // Start is called before the first frame update
    void Start()
    {   
        motion = transform.DOMoveX(range, range/speed).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnDrawGizmosSelected(){
        Gizmos.color = Color.blue;

        Gizmos.DrawLine(transform.position, transform.position + (transform.right * range));
        Gizmos.DrawCube(transform.position + (transform.right * (range/2)), Vector2.one * 0.1f);
    }


    public void Freeze(float duration){
        var sr = GetComponent<SpriteRenderer>();
        var color = sr.color;
        motion.Pause();

        GetComponent<Collider2D>().usedByEffector = true;
        sr.DOColor(Color.cyan, 0.2f).onComplete = () => sr.DOColor(color, duration -.2f);
    }

    IEnumerator FreezeAnim(){
        
    }

    public void BecomeGrabbable(){
        var grababble = gameObject.AddComponent(typeof(Grabbable));
        GetComponent<Freezable>().onUnfreeze.AddListener(()=> Destroy(grababble));
    }

    public void Unfreeze(){
        motion.Play();
        GetComponent<Collider2D>().usedByEffector = false;
    }
}
