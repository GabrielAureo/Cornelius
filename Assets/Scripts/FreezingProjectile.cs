using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

public class FreezingProjectile : GuidedProjectile{
 
    [SerializeField] CameraSettings cameraSettings;

    protected override void onInitialize(){
        GameManager.instance.cameraController.SetTarget(transform, true, cameraSettings);
        timer = 0f;
        rb.AddForce(transform.up * launchSpeed, ForceMode2D.Impulse);
    }

    protected override void onHit(Collider2D hit)
    {
        var freezable = hit.GetComponent<Freezable>();
        if(freezable){
            freezable.Freeze();
            if(effectAction != null)
                effectAction(freezable);
        } 
        //FreezeEnemy(hit.gameObject);   
    }

    public override MonoBehaviour getEffect(){
        return this;
    }

    protected override void onUpdate(){}    

}